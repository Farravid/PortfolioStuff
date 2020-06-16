// GENERATED AUTOMATICALLY FROM 'Assets/GamepadController.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class GamepadController : IInputActionCollection
{
    private InputActionAsset asset;
    public GamepadController()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GamepadController"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""152290e5-ee4e-458e-a4fe-5ce6acbd8028"",
            ""actions"": [
                {
                    ""name"": ""MovePlayer"",
                    ""id"": ""2464dc1c-376c-44d1-bb0d-acf5f184d71b"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""MoveCamera"",
                    ""id"": ""636ff9e0-7ff4-45d7-a166-22dcd7ddf7e4"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""FlyDash"",
                    ""id"": ""1a31b095-964f-441a-8c89-e4e4ad0ad60f"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6e58dc44-6c75-4416-83e4-42e35dead360"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovePlayer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""d695833a-fe5c-4cdd-b428-850f0631fca8"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""81967ddf-2b93-440e-8bd1-dca8a60d4b59"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""FlyDash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.GetActionMap("Gameplay");
        m_Gameplay_MovePlayer = m_Gameplay.GetAction("MovePlayer");
        m_Gameplay_MoveCamera = m_Gameplay.GetAction("MoveCamera");
        m_Gameplay_FlyDash = m_Gameplay.GetAction("FlyDash");
    }

    ~GamepadController()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes
    {
        get => asset.controlSchemes;
    }

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Gameplay
    private InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private InputAction m_Gameplay_MovePlayer;
    private InputAction m_Gameplay_MoveCamera;
    private InputAction m_Gameplay_FlyDash;
    public struct GameplayActions
    {
        private GamepadController m_Wrapper;
        public GameplayActions(GamepadController wrapper) { m_Wrapper = wrapper; }
        public InputAction @MovePlayer { get { return m_Wrapper.m_Gameplay_MovePlayer; } }
        public InputAction @MoveCamera { get { return m_Wrapper.m_Gameplay_MoveCamera; } }
        public InputAction @FlyDash { get { return m_Wrapper.m_Gameplay_FlyDash; } }
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                MovePlayer.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovePlayer;
                MovePlayer.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovePlayer;
                MovePlayer.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovePlayer;
                MoveCamera.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCamera;
                MoveCamera.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCamera;
                MoveCamera.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCamera;
                FlyDash.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnFlyDash;
                FlyDash.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnFlyDash;
                FlyDash.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnFlyDash;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                MovePlayer.started += instance.OnMovePlayer;
                MovePlayer.performed += instance.OnMovePlayer;
                MovePlayer.canceled += instance.OnMovePlayer;
                MoveCamera.started += instance.OnMoveCamera;
                MoveCamera.performed += instance.OnMoveCamera;
                MoveCamera.canceled += instance.OnMoveCamera;
                FlyDash.started += instance.OnFlyDash;
                FlyDash.performed += instance.OnFlyDash;
                FlyDash.canceled += instance.OnFlyDash;
            }
        }
    }
    public GameplayActions @Gameplay
    {
        get
        {
            return new GameplayActions(this);
        }
    }
    public interface IGameplayActions
    {
        void OnMovePlayer(InputAction.CallbackContext context);
        void OnMoveCamera(InputAction.CallbackContext context);
        void OnFlyDash(InputAction.CallbackContext context);
    }
}
