using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace FrostRealm.Core
{
    /// <summary>
    /// Generates terrain for FrostRealm Chronicles maps with Warcraft III-style features.
    /// Creates ground plane, cliffs, water, and texture mapping.
    /// </summary>
    public class TerrainGenerator : MonoBehaviour
    {
        [Header("Terrain Settings")]
        [SerializeField] private int mapWidth = 128;
        [SerializeField] private int mapHeight = 128;
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private int textureResolution = 2048;
        
        [Header("Terrain Materials")]
        [SerializeField] private Material grassMaterial;
        [SerializeField] private Material dirtMaterial;
        [SerializeField] private Material snowMaterial;
        [SerializeField] private Material waterMaterial;
        
        [Header("Map Features")]
        [SerializeField] private bool generateCliffs = true;
        [SerializeField] private bool generateWater = true;
        [SerializeField] private bool generateTrees = true;
        [SerializeField] private float cliffHeight = 3f;
        
        private GameObject terrainObject;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        
        private void Start()
        {
            GenerateTerrain();
        }
        
        public void GenerateTerrain()
        {
            CreateTerrainObject();
            GenerateMesh();
            ApplyMaterials();
            GenerateMapFeatures();
        }
        
        private void CreateTerrainObject()
        {
            if (terrainObject != null)
            {
                DestroyImmediate(terrainObject);
            }
            
            terrainObject = new GameObject("Generated Terrain");
            terrainObject.layer = LayerMask.NameToLayer("Terrain");
            
            meshFilter = terrainObject.AddComponent<MeshFilter>();
            meshRenderer = terrainObject.AddComponent<MeshRenderer>();
            meshCollider = terrainObject.AddComponent<MeshCollider>();
            
            // Add HDRP decal projector support
            terrainObject.AddComponent<DecalProjector>();
        }
        
        private void GenerateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = "Terrain Mesh";
            
            // Calculate vertices
            int vertexCount = (mapWidth + 1) * (mapHeight + 1);
            Vector3[] vertices = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            
            // Generate height map with Perlin noise for variation
            float[,] heightMap = GenerateHeightMap();
            
            // Create vertices
            for (int z = 0; z <= mapHeight; z++)
            {
                for (int x = 0; x <= mapWidth; x++)
                {
                    int index = z * (mapWidth + 1) + x;
                    float height = heightMap[x, z];
                    
                    vertices[index] = new Vector3(x * tileSize, height, z * tileSize);
                    uvs[index] = new Vector2((float)x / mapWidth, (float)z / mapHeight);
                }
            }
            
            // Create triangles
            int triangleCount = mapWidth * mapHeight * 2;
            int[] triangles = new int[triangleCount * 3];
            int triIndex = 0;
            
            for (int z = 0; z < mapHeight; z++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    int topLeft = z * (mapWidth + 1) + x;
                    int topRight = topLeft + 1;
                    int bottomLeft = (z + 1) * (mapWidth + 1) + x;
                    int bottomRight = bottomLeft + 1;
                    
                    // First triangle
                    triangles[triIndex++] = topLeft;
                    triangles[triIndex++] = bottomLeft;
                    triangles[triIndex++] = topRight;
                    
                    // Second triangle
                    triangles[triIndex++] = topRight;
                    triangles[triIndex++] = bottomLeft;
                    triangles[triIndex++] = bottomRight;
                }
            }
            
            // Apply to mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
        
        private float[,] GenerateHeightMap()
        {
            float[,] heightMap = new float[mapWidth + 1, mapHeight + 1];
            
            // Base terrain with Perlin noise
            float noiseScale = 0.1f;
            float heightScale = 0.5f;
            
            for (int z = 0; z <= mapHeight; z++)
            {
                for (int x = 0; x <= mapWidth; x++)
                {
                    float noiseValue = Mathf.PerlinNoise(x * noiseScale, z * noiseScale);
                    heightMap[x, z] = noiseValue * heightScale;
                    
                    // Add cliff plateaus
                    if (generateCliffs && noiseValue > 0.6f)
                    {
                        heightMap[x, z] = cliffHeight;
                    }
                    
                    // Create water depressions
                    if (generateWater && noiseValue < 0.3f)
                    {
                        heightMap[x, z] = -0.5f;
                    }
                }
            }
            
            return heightMap;
        }
        
        private void ApplyMaterials()
        {
            // Create default materials if not assigned
            if (grassMaterial == null)
            {
                grassMaterial = CreateDefaultMaterial("Grass", new Color(0.2f, 0.6f, 0.2f));
            }
            if (dirtMaterial == null)
            {
                dirtMaterial = CreateDefaultMaterial("Dirt", new Color(0.5f, 0.3f, 0.1f));
            }
            if (snowMaterial == null)
            {
                snowMaterial = CreateDefaultMaterial("Snow", Color.white);
            }
            if (waterMaterial == null)
            {
                waterMaterial = CreateDefaultMaterial("Water", new Color(0.1f, 0.3f, 0.6f, 0.8f));
                waterMaterial.SetFloat("_Surface", 1); // Transparent
            }
            
            // Apply main material
            meshRenderer.material = grassMaterial;
        }
        
        private Material CreateDefaultMaterial(string name, Color color)
        {
            Material mat = new Material(Shader.Find("HDRP/Lit"));
            mat.name = name;
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.2f);
            return mat;
        }
        
        private void GenerateMapFeatures()
        {
            // Generate trees
            if (generateTrees)
            {
                GenerateTrees();
            }
            
            // Generate water planes
            if (generateWater)
            {
                GenerateWaterPlanes();
            }
            
            // Generate resource points (gold mines, lumber)
            GenerateResourcePoints();
            
            // Generate creep camps
            GenerateCreepCamps();
        }
        
        private void GenerateTrees()
        {
            GameObject treeContainer = new GameObject("Trees");
            treeContainer.transform.parent = terrainObject.transform;
            
            int treeCount = Random.Range(50, 100);
            
            for (int i = 0; i < treeCount; i++)
            {
                Vector3 position = new Vector3(
                    Random.Range(10f, (mapWidth - 10) * tileSize),
                    0,
                    Random.Range(10f, (mapHeight - 10) * tileSize)
                );
                
                // Sample height at position
                if (Physics.Raycast(position + Vector3.up * 10, Vector3.down, out RaycastHit hit, 20f))
                {
                    position.y = hit.point.y;
                    
                    // Don't place trees on water or cliffs
                    if (position.y > 0.1f && position.y < cliffHeight - 0.5f)
                    {
                        CreateTree(position, treeContainer.transform);
                    }
                }
            }
        }
        
        private void CreateTree(Vector3 position, Transform parent)
        {
            GameObject tree = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tree.name = "Tree";
            tree.transform.position = position;
            tree.transform.localScale = new Vector3(1f, 3f, 1f);
            tree.transform.parent = parent;
            
            // Add green material
            Material treeMat = CreateDefaultMaterial("Tree", new Color(0.1f, 0.4f, 0.1f));
            tree.GetComponent<Renderer>().material = treeMat;
            
            // Add tree component for lumber harvesting
            tree.AddComponent<TreeResource>();
        }
        
        private void GenerateWaterPlanes()
        {
            GameObject water = GameObject.CreatePrimitive(PrimitiveType.Cube);
            water.name = "Water";
            water.transform.position = new Vector3(mapWidth * tileSize * 0.5f, -0.25f, mapHeight * tileSize * 0.5f);
            water.transform.localScale = new Vector3(mapWidth * tileSize, 0.5f, mapHeight * tileSize);
            water.transform.parent = terrainObject.transform;
            
            if (waterMaterial != null)
            {
                water.GetComponent<Renderer>().material = waterMaterial;
            }
            
            // Remove collider for water
            Destroy(water.GetComponent<Collider>());
        }
        
        private void GenerateResourcePoints()
        {
            GameObject resourceContainer = new GameObject("Resource Points");
            resourceContainer.transform.parent = terrainObject.transform;
            
            // Generate gold mines
            int goldMineCount = Random.Range(4, 8);
            for (int i = 0; i < goldMineCount; i++)
            {
                Vector3 position = GetRandomGroundPosition();
                CreateGoldMine(position, resourceContainer.transform);
            }
        }
        
        private Vector3 GetRandomGroundPosition()
        {
            Vector3 position = new Vector3(
                Random.Range(20f, (mapWidth - 20) * tileSize),
                10f,
                Random.Range(20f, (mapHeight - 20) * tileSize)
            );
            
            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 20f))
            {
                position.y = hit.point.y;
            }
            
            return position;
        }
        
        private void CreateGoldMine(Vector3 position, Transform parent)
        {
            GameObject goldMine = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goldMine.name = "Gold Mine";
            goldMine.transform.position = position;
            goldMine.transform.localScale = new Vector3(4f, 2f, 4f);
            goldMine.transform.parent = parent;
            
            // Gold material
            Material goldMat = CreateDefaultMaterial("Gold", new Color(1f, 0.8f, 0f));
            goldMat.SetFloat("_Metallic", 0.8f);
            goldMat.SetFloat("_Smoothness", 0.7f);
            goldMine.GetComponent<Renderer>().material = goldMat;
            
            // Add gold mine component
            goldMine.AddComponent<GoldMineResource>();
        }
        
        private void GenerateCreepCamps()
        {
            GameObject creepContainer = new GameObject("Creep Camps");
            creepContainer.transform.parent = terrainObject.transform;
            
            // Generate creep spawn points
            int creepCampCount = Random.Range(6, 12);
            for (int i = 0; i < creepCampCount; i++)
            {
                Vector3 position = GetRandomGroundPosition();
                CreateCreepCamp(position, creepContainer.transform, i + 1);
            }
        }
        
        private void CreateCreepCamp(Vector3 position, Transform parent, int level)
        {
            GameObject camp = new GameObject($"Creep Camp Level {level}");
            camp.transform.position = position;
            camp.transform.parent = parent;
            
            // Add creep camp component
            CreepCamp creepCamp = camp.AddComponent<CreepCamp>();
            creepCamp.SetLevel(level);
            
            // Visual marker
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "Camp Marker";
            marker.transform.position = position + Vector3.up;
            marker.transform.localScale = Vector3.one * 0.5f;
            marker.transform.parent = camp.transform;
            
            Material markerMat = CreateDefaultMaterial("Creep", Color.red);
            marker.GetComponent<Renderer>().material = markerMat;
        }
        
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("FrostRealm/Generate Terrain")]
        public static void GenerateTerrainEditor()
        {
            TerrainGenerator generator = FindFirstObjectByType<TerrainGenerator>();
            if (generator == null)
            {
                GameObject go = new GameObject("Terrain Generator");
                generator = go.AddComponent<TerrainGenerator>();
            }
            
            generator.GenerateTerrain();
            Debug.Log("Terrain generated successfully!");
        }
        #endif
    }
    
    // Resource components
    public class TreeResource : MonoBehaviour
    {
        public int lumberAmount = 200;
        public float regrowTime = 120f;
    }
    
    public class GoldMineResource : MonoBehaviour
    {
        public int goldAmount = 12500;
        public int workersMax = 5;
    }
    
    public class CreepCamp : MonoBehaviour
    {
        public int level = 1;
        public int experienceReward = 100;
        
        public void SetLevel(int newLevel)
        {
            level = Mathf.Clamp(newLevel, 1, 10);
            experienceReward = level * 100;
        }
    }
}