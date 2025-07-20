using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace FrostRealm.Core
{
    /// <summary>
    /// Central audio management system for FrostRealm Chronicles.
    /// Handles music, sound effects, and voice lines with dynamic mixing and 3D spatial audio.
    /// Inspired by Warcraft III: The Frozen Throne audio system.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Mixer Groups")]
        [SerializeField] private AudioMixerGroup masterMixerGroup;
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        [SerializeField] private AudioMixerGroup voiceMixerGroup;
        [SerializeField] private AudioMixerGroup ambientMixerGroup;
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource ambientSource;
        [SerializeField] private AudioSource uiSfxSource;
        
        [Header("Audio Settings")]
        [SerializeField] private float masterVolume = 1.0f;
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 0.8f;
        [SerializeField] private float voiceVolume = 0.9f;
        [SerializeField] private float ambientVolume = 0.5f;
        
        [Header("Music Transition")]
        [SerializeField] private float musicFadeTime = 2.0f;
        [SerializeField] private AnimationCurve musicFadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("3D Audio Settings")]
        [SerializeField] private float maxAudioDistance = 50f;
        [SerializeField] private AnimationCurve spatialFalloff = AnimationCurve.Linear(0, 1, 1, 0);
        
        // Audio pools for efficiency
        private Queue<AudioSource> sfxSourcePool = new Queue<AudioSource>();
        private Queue<AudioSource> voiceSourcePool = new Queue<AudioSource>();
        private List<AudioSource> activeAudioSources = new List<AudioSource>();
        
        // Music system
        private AudioClip currentMusic;
        private AudioClip queuedMusic;
        private Coroutine musicTransitionCoroutine;
        
        // Sound library references
        private Dictionary<string, AudioClip> musicLibrary = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> sfxLibrary = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> voiceLibrary = new Dictionary<string, AudioClip>();
        
        // Events
        public System.Action<float> OnMasterVolumeChanged;
        public System.Action<float> OnMusicVolumeChanged;
        public System.Action<float> OnSfxVolumeChanged;
        public System.Action<float> OnVoiceVolumeChanged;
        public System.Action<AudioClip> OnMusicTrackChanged;
        
        // Singleton instance
        private static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AudioManager>();
                    if (instance == null)
                    {
                        var go = new GameObject("Audio Manager");
                        instance = go.AddComponent<AudioManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        // Properties
        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SfxVolume => sfxVolume;
        public float VoiceVolume => voiceVolume;
        public float AmbientVolume => ambientVolume;
        public bool IsPlayingMusic => musicSource != null && musicSource.isPlaying;
        public AudioClip CurrentMusic => currentMusic;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioManager();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            LoadAudioLibraries();
            ApplyVolumeSettings();
        }
        
        /// <summary>
        /// Initializes the audio manager components.
        /// </summary>
        private void InitializeAudioManager()
        {
            // Create audio sources if not assigned
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.outputAudioMixerGroup = musicMixerGroup;
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            
            if (ambientSource == null)
            {
                ambientSource = gameObject.AddComponent<AudioSource>();
                ambientSource.outputAudioMixerGroup = ambientMixerGroup;
                ambientSource.loop = true;
                ambientSource.playOnAwake = false;
            }
            
            if (uiSfxSource == null)
            {
                uiSfxSource = gameObject.AddComponent<AudioSource>();
                uiSfxSource.outputAudioMixerGroup = sfxMixerGroup;
                uiSfxSource.loop = false;
                uiSfxSource.playOnAwake = false;
            }
            
            // Initialize audio source pools
            InitializeAudioSourcePools();
        }
        
        /// <summary>
        /// Initializes object pools for audio sources.
        /// </summary>
        private void InitializeAudioSourcePools()
        {
            // Create SFX source pool
            for (int i = 0; i < 10; i++)
            {
                var sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.outputAudioMixerGroup = sfxMixerGroup;
                sfxSource.playOnAwake = false;
                sfxSource.enabled = false;
                sfxSourcePool.Enqueue(sfxSource);
            }
            
            // Create voice source pool
            for (int i = 0; i < 5; i++)
            {
                var voiceSource = gameObject.AddComponent<AudioSource>();
                voiceSource.outputAudioMixerGroup = voiceMixerGroup;
                voiceSource.playOnAwake = false;
                voiceSource.enabled = false;
                voiceSourcePool.Enqueue(voiceSource);
            }
        }
        
        /// <summary>
        /// Loads audio clips from Resources folders into libraries.
        /// </summary>
        private void LoadAudioLibraries()
        {
            // Load music
            var musicClips = Resources.LoadAll<AudioClip>("Audio/Music");
            foreach (var clip in musicClips)
            {
                musicLibrary[clip.name] = clip;
            }
            
            // Load SFX
            var sfxClips = Resources.LoadAll<AudioClip>("Audio/SFX");
            foreach (var clip in sfxClips)
            {
                sfxLibrary[clip.name] = clip;
            }
            
            // Load voice lines
            var voiceClips = Resources.LoadAll<AudioClip>("Audio/Voice");
            foreach (var clip in voiceClips)
            {
                voiceLibrary[clip.name] = clip;
            }
            
            Debug.Log($"Audio libraries loaded: {musicLibrary.Count} music, {sfxLibrary.Count} SFX, {voiceLibrary.Count} voice clips");
        }
        
        /// <summary>
        /// Plays a music track with optional crossfading.
        /// </summary>
        /// <param name="musicName">Name of the music track</param>
        /// <param name="fadeIn">Whether to fade in the track</param>
        /// <param name="loop">Whether to loop the track</param>
        public void PlayMusic(string musicName, bool fadeIn = true, bool loop = true)
        {
            if (!musicLibrary.TryGetValue(musicName, out AudioClip clip))
            {
                Debug.LogWarning($"Music track '{musicName}' not found in library!");
                return;
            }
            
            PlayMusic(clip, fadeIn, loop);
        }
        
        /// <summary>
        /// Plays a music track with optional crossfading.
        /// </summary>
        public void PlayMusic(AudioClip musicClip, bool fadeIn = true, bool loop = true)
        {
            if (musicClip == null)
            {
                Debug.LogWarning("Attempted to play null music clip!");
                return;
            }
            
            if (musicTransitionCoroutine != null)
            {
                StopCoroutine(musicTransitionCoroutine);
            }
            
            queuedMusic = musicClip;
            
            if (fadeIn && IsPlayingMusic)
            {
                musicTransitionCoroutine = StartCoroutine(CrossfadeMusicCoroutine(musicClip, loop));
            }
            else
            {
                SetMusicTrack(musicClip, loop);
                if (fadeIn)
                {
                    musicTransitionCoroutine = StartCoroutine(FadeInMusicCoroutine());
                }
            }
        }
        
        /// <summary>
        /// Stops the current music with optional fade out.
        /// </summary>
        public void StopMusic(bool fadeOut = true)
        {
            if (musicTransitionCoroutine != null)
            {
                StopCoroutine(musicTransitionCoroutine);
            }
            
            if (fadeOut && IsPlayingMusic)
            {
                musicTransitionCoroutine = StartCoroutine(FadeOutMusicCoroutine());
            }
            else
            {
                musicSource.Stop();
                currentMusic = null;
            }
        }
        
        /// <summary>
        /// Plays a sound effect at the specified position.
        /// </summary>
        public void PlaySfx(string sfxName, Vector3 position = default, float volume = 1.0f, float pitch = 1.0f)
        {
            if (!sfxLibrary.TryGetValue(sfxName, out AudioClip clip))
            {
                Debug.LogWarning($"SFX '{sfxName}' not found in library!");
                return;
            }
            
            PlaySfx(clip, position, volume, pitch);
        }
        
        /// <summary>
        /// Plays a sound effect at the specified position.
        /// </summary>
        public void PlaySfx(AudioClip sfxClip, Vector3 position = default, float volume = 1.0f, float pitch = 1.0f)
        {
            if (sfxClip == null) return;
            
            AudioSource source = GetPooledAudioSource(sfxSourcePool);
            if (source == null) return;
            
            ConfigureAudioSource(source, sfxClip, position, volume, pitch, false);
            source.Play();
            
            StartCoroutine(ReturnAudioSourceToPool(source, sfxSourcePool, sfxClip.length / pitch));
        }
        
        /// <summary>
        /// Plays a UI sound effect (2D, no spatialization).
        /// </summary>
        public void PlayUiSfx(string sfxName, float volume = 1.0f, float pitch = 1.0f)
        {
            if (!sfxLibrary.TryGetValue(sfxName, out AudioClip clip))
            {
                Debug.LogWarning($"UI SFX '{sfxName}' not found in library!");
                return;
            }
            
            PlayUiSfx(clip, volume, pitch);
        }
        
        /// <summary>
        /// Plays a UI sound effect (2D, no spatialization).
        /// </summary>
        public void PlayUiSfx(AudioClip sfxClip, float volume = 1.0f, float pitch = 1.0f)
        {
            if (sfxClip == null || uiSfxSource == null) return;
            
            uiSfxSource.pitch = pitch;
            uiSfxSource.PlayOneShot(sfxClip, volume);
        }
        
        /// <summary>
        /// Plays a voice line at the specified position.
        /// </summary>
        public void PlayVoice(string voiceName, Vector3 position = default, float volume = 1.0f)
        {
            if (!voiceLibrary.TryGetValue(voiceName, out AudioClip clip))
            {
                Debug.LogWarning($"Voice line '{voiceName}' not found in library!");
                return;
            }
            
            PlayVoice(clip, position, volume);
        }
        
        /// <summary>
        /// Plays a voice line at the specified position.
        /// </summary>
        public void PlayVoice(AudioClip voiceClip, Vector3 position = default, float volume = 1.0f)
        {
            if (voiceClip == null) return;
            
            AudioSource source = GetPooledAudioSource(voiceSourcePool);
            if (source == null) return;
            
            ConfigureAudioSource(source, voiceClip, position, volume, 1.0f, false);
            source.Play();
            
            StartCoroutine(ReturnAudioSourceToPool(source, voiceSourcePool, voiceClip.length));
        }
        
        /// <summary>
        /// Sets master volume and applies to all audio.
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            OnMasterVolumeChanged?.Invoke(masterVolume);
        }
        
        /// <summary>
        /// Sets music volume.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            OnMusicVolumeChanged?.Invoke(musicVolume);
        }
        
        /// <summary>
        /// Sets SFX volume.
        /// </summary>
        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            OnSfxVolumeChanged?.Invoke(sfxVolume);
        }
        
        /// <summary>
        /// Sets voice volume.
        /// </summary>
        public void SetVoiceVolume(float volume)
        {
            voiceVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            OnVoiceVolumeChanged?.Invoke(voiceVolume);
        }
        
        /// <summary>
        /// Applies current volume settings to all audio mixer groups.
        /// </summary>
        private void ApplyVolumeSettings()
        {
            if (masterMixerGroup != null)
            {
                float masterDb = masterVolume > 0 ? Mathf.Log10(masterVolume) * 20 : -80f;
                masterMixerGroup.audioMixer.SetFloat("MasterVolume", masterDb);
            }
            
            if (musicMixerGroup != null)
            {
                float musicDb = musicVolume > 0 ? Mathf.Log10(musicVolume) * 20 : -80f;
                musicMixerGroup.audioMixer.SetFloat("MusicVolume", musicDb);
            }
            
            if (sfxMixerGroup != null)
            {
                float sfxDb = sfxVolume > 0 ? Mathf.Log10(sfxVolume) * 20 : -80f;
                sfxMixerGroup.audioMixer.SetFloat("SfxVolume", sfxDb);
            }
            
            if (voiceMixerGroup != null)
            {
                float voiceDb = voiceVolume > 0 ? Mathf.Log10(voiceVolume) * 20 : -80f;
                voiceMixerGroup.audioMixer.SetFloat("VoiceVolume", voiceDb);
            }
        }
        
        /// <summary>
        /// Gets a pooled audio source or creates a new one if none available.
        /// </summary>
        private AudioSource GetPooledAudioSource(Queue<AudioSource> pool)
        {
            AudioSource source = null;
            
            if (pool.Count > 0)
            {
                source = pool.Dequeue();
            }
            else
            {
                // Create new source if pool is empty
                source = gameObject.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = pool == sfxSourcePool ? sfxMixerGroup : voiceMixerGroup;
                source.playOnAwake = false;
            }
            
            source.enabled = true;
            activeAudioSources.Add(source);
            return source;
        }
        
        /// <summary>
        /// Configures an audio source for playback.
        /// </summary>
        private void ConfigureAudioSource(AudioSource source, AudioClip clip, Vector3 position, float volume, float pitch, bool loop)
        {
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = loop;
            
            if (position != default)
            {
                // 3D spatial audio
                source.spatialBlend = 1.0f;
                source.maxDistance = maxAudioDistance;
                source.rolloffMode = AudioRolloffMode.Custom;
                source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, spatialFalloff);
                source.transform.position = position;
            }
            else
            {
                // 2D audio
                source.spatialBlend = 0.0f;
            }
        }
        
        /// <summary>
        /// Returns an audio source to its pool after the clip finishes.
        /// </summary>
        private IEnumerator ReturnAudioSourceToPool(AudioSource source, Queue<AudioSource> pool, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (source != null && activeAudioSources.Contains(source))
            {
                source.Stop();
                source.clip = null;
                source.enabled = false;
                activeAudioSources.Remove(source);
                pool.Enqueue(source);
            }
        }
        
        /// <summary>
        /// Sets the current music track without transitions.
        /// </summary>
        private void SetMusicTrack(AudioClip musicClip, bool loop)
        {
            if (musicSource == null) return;
            
            musicSource.clip = musicClip;
            musicSource.loop = loop;
            musicSource.volume = musicVolume;
            musicSource.Play();
            
            currentMusic = musicClip;
            OnMusicTrackChanged?.Invoke(currentMusic);
        }
        
        /// <summary>
        /// Coroutine for crossfading between music tracks.
        /// </summary>
        private IEnumerator CrossfadeMusicCoroutine(AudioClip newTrack, bool loop)
        {
            float elapsedTime = 0f;
            float initialVolume = musicSource.volume;
            
            // Fade out current track
            while (elapsedTime < musicFadeTime / 2)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / (musicFadeTime / 2);
                musicSource.volume = Mathf.Lerp(initialVolume, 0f, musicFadeCurve.Evaluate(progress));
                yield return null;
            }
            
            // Switch tracks
            SetMusicTrack(newTrack, loop);
            musicSource.volume = 0f;
            
            // Fade in new track
            elapsedTime = 0f;
            while (elapsedTime < musicFadeTime / 2)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / (musicFadeTime / 2);
                musicSource.volume = Mathf.Lerp(0f, musicVolume, musicFadeCurve.Evaluate(progress));
                yield return null;
            }
            
            musicSource.volume = musicVolume;
        }
        
        /// <summary>
        /// Coroutine for fading in music.
        /// </summary>
        private IEnumerator FadeInMusicCoroutine()
        {
            musicSource.volume = 0f;
            float elapsedTime = 0f;
            
            while (elapsedTime < musicFadeTime)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / musicFadeTime;
                musicSource.volume = Mathf.Lerp(0f, musicVolume, musicFadeCurve.Evaluate(progress));
                yield return null;
            }
            
            musicSource.volume = musicVolume;
        }
        
        /// <summary>
        /// Coroutine for fading out music.
        /// </summary>
        private IEnumerator FadeOutMusicCoroutine()
        {
            float initialVolume = musicSource.volume;
            float elapsedTime = 0f;
            
            while (elapsedTime < musicFadeTime)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / musicFadeTime;
                musicSource.volume = Mathf.Lerp(initialVolume, 0f, musicFadeCurve.Evaluate(progress));
                yield return null;
            }
            
            musicSource.Stop();
            currentMusic = null;
        }
        
        void OnDestroy()
        {
            if (musicTransitionCoroutine != null)
            {
                StopCoroutine(musicTransitionCoroutine);
            }
        }
    }
}