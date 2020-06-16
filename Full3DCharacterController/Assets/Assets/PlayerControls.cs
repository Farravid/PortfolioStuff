// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerControls : IInputActionCollection
{
    private InputActionAsset asset;
    public PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""1a6317ad-679a-49dc-9299-30c00f20867b"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""id"": ""c742a001-f0b8-4bb3-b2cb-7d403ec3b483"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""MovePlayer"",
                    ""id"": ""e8da132b-3e42-4f05-ada1-6cfbefa7be8c"",
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
                    ""id"": ""d2003b06-c2c4-4f15-b9e5-3b0f95b29996"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""RunButton"",
                    ""id"": ""39d8d438-c8c7-4c2d-8b03-94253da8f619"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Agacharse"",
                    ""id"": ""d6e4ae2b-a4cb-4773-88ce-1b77d71dd4bd"",
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
                    ""id"": ""ee3a4219-95ed-403b-8899-27ec6acead09"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""b41384df-0fec-48e8-8d18-cf0f42ee7a5c"",
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
                    ""id"": ""f44d2c5b-539a-41ca-91bf-00a249409ec6"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RunButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""7dd8f250-b012-4e3f-826a-bef16038cfd1"",
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
                    ""id"": ""5ac03d39-19a2-489a-9435-14668d10bf5e"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Agacharse"",
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
        m_Gameplay_Jump = m_Gameplay.GetAction("Jump");
        m_Gameplay_MovePlayer = m_Gameplay.GetAction("MovePlayer");
        m_Gameplay_MoveCamera = m_Gameplay.GetAction("MoveCamera");
        m_Gameplay_RunButton = m_Gameplay.GetAction("RunButton");
        m_Gameplay_Agacharse = m_Gameplay.GetAction("Agacharse");
    }

    ~PlayerControls()
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
    private InputAction m_Gameplay_Jump;
    private InputAction m_Gameplay_MovePlayer;
    private InputAction m_Gameplay_MoveCamera;
    private InputAction m_Gameplay_RunButton;
    private InputAction m_Gameplay_Agacharse;
    public struct GameplayActions
    {
        private PlayerControls m_Wrapper;
        public GameplayActions(PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump { get { return m_Wrapper.m_Gameplay_Jump; } }
        public InputAction @MovePlayer { get { return m_Wrapper.m_Gameplay_MovePlayer; } }
        public InputAction @MoveCamera { get { return m_Wrapper.m_Gameplay_MoveCamera; } }
        public InputAction @RunButton { get { return m_Wrapper.m_Gameplay_RunButton; } }
        public InputAction @Agacharse { get { return m_Wrapper.m_Gameplay_Agacharse; } }
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
                Jump.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                Jump.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                Jump.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                MovePlayer.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovePlayer;
                MovePlayer.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovePlayer;
                MovePlayer.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovePlayer;
                MoveCamera.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCamera;
                MoveCamera.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCamera;
                MoveCamera.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveCamera;
                RunButton.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRunButton;
                RunButton.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRunButton;
                RunButton.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRunButton;
                Agacharse.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAgacharse;
                Agacharse.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAgacharse;
                Agacharse.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAgacharse;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                Jump.started += instance.OnJump;
                Jump.performed += instance.OnJump;
                Jump.canceled += instance.OnJump;
                MovePlayer.started += instance.OnMovePlayer;
                MovePlayer.performed += instance.OnMovePlayer;
                MovePlayer.canceled += instance.OnMovePlayer;
                MoveCamera.started += instance.OnMoveCamera;
                MoveCamera.performed += instance.OnMoveCamera;
                MoveCamera.canceled += instance.OnMoveCamera;
                RunButton.started += instance.OnRunButton;
                RunButton.performed += instance.OnRunButton;
                RunButton.canceled += instance.OnRunButton;
                Agacharse.started += instance.OnAgacharse;
                Agacharse.performed += instance.OnAgacharse;
                Agacharse.canceled += instance.OnAgacharse;
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
        void OnJump(InputAction.CallbackContext context);
        void OnMovePlayer(InputAction.CallbackContext context);
        void OnMoveCamera(InputAction.CallbackContext context);
        void OnRunButton(InputAction.CallbackContext context);
        void OnAgacharse(InputAction.CallbackContext context);
    }
}
