using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

// NOTE: This is a **temporary stub** for CI / headless builds where the Input
// System's code-generation step does not run.  It exposes just enough surface
// to satisfy existing scripts at compile-time.  Replace with the real
// generated class (via .inputactions "Generate C# Class") when working in the
// Unity Editor.

public class FrostRealmInputActions : IEnumerable<InputAction>, IDisposable
{
    // Empty asset – runtime input will not work, but compilation succeeds.
    private readonly InputActionAsset _asset = ScriptableObject.CreateInstance<InputActionAsset>();

    // ----------------- nested action maps -----------------
    public struct UIActions
    {
        public readonly InputAction Navigate  => new InputAction();
        public readonly InputAction Submit    => new InputAction();
        public readonly InputAction Cancel    => new InputAction();
        public readonly InputAction Point     => new InputAction();
        public readonly InputAction Click     => new InputAction();
        public readonly InputAction RightClick=> new InputAction();
    }

    public struct PlayerActions
    {
        public readonly InputAction Pause => new InputAction();
    }

    // Public accessors expected by game code
    public UIActions UI => new UIActions();
    public PlayerActions Player => new PlayerActions();

    // ----------------- interface plumbing -----------------
    public void Dispose() => UnityEngine.Object.Destroy(_asset);

    public InputBinding? bindingMask { get => null; set { } }
    public ReadOnlyArray<InputDevice>? devices { get => null; set { } }
    public ReadOnlyArray<InputControlScheme> controlSchemes => _asset.controlSchemes;

    // IEnumerable implementation
    public IEnumerator<InputAction> GetEnumerator() => _asset.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Enable()  { }
    public void Disable() { }

    public InputActionAsset Get() => _asset;

    public InputAction FindAction(string nameOrId, bool throwIfNotFound = false) => null;
    public int FindBinding(InputBinding bindingMask, out InputAction action) { action = null; return -1; }

    public InputActionMap FindActionMap(string name, bool throwIfNotFound = false) => null;
} 