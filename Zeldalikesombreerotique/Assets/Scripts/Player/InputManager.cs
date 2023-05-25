//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.0
//     from Assets/Scripts/Player/InputManager.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputManager: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputManager()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputManager"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""7a6bd78c-c3f7-4807-8ac8-af51f48e0dab"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""87dcbd9f-f708-429e-bb79-71345933718e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""InteractEnter"",
                    ""type"": ""Button"",
                    ""id"": ""bd65ee18-55d4-4b39-95c6-32c03a6d1aae"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""InteractLeave"",
                    ""type"": ""Button"",
                    ""id"": ""0f86f696-d031-4f41-9775-f7671741c8a4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""0d36d52a-3fad-4b77-9fe8-fa7fafa936ce"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SecondaryEnter"",
                    ""type"": ""Button"",
                    ""id"": ""e402e382-1e21-427a-87f6-8a40ef13665f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Oui"",
                    ""type"": ""Button"",
                    ""id"": ""fc24154e-dda8-422c-9c9a-ff4b357a1893"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(pressPoint=1.401298E-45,behavior=2)"",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""zqsd keys"",
                    ""id"": ""d62d4584-9268-4062-911f-5f277d6432d3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a36a2c64-c834-4474-91d0-ca6e1b1114c5"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard layout"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""19baa0b0-b376-46f1-bc39-bbc81c6a9811"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard layout"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""816fb4e4-3564-4b0a-80dc-44720b4ff050"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard layout"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""01e2fecf-a734-4809-b822-a7cc2cc0353c"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard layout"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""df9036a4-bc1a-4a57-8a96-2eea8ccc7732"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(min=0.2,max=0.8),AxisDeadzone(min=0.2,max=0.8),Clamp(min=-1,max=1)"",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d37d3a61-bb7b-4e70-bfa2-c105d8f6f612"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1ded8fc6-94f1-405d-a2ba-2ece0e11249d"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2704e173-7f2e-4fb2-9575-bef93003d31c"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""bb9605e7-166f-4eb8-86e8-19c360bd1cb2"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""c8c962ca-9e33-4fc3-8cf0-60a11a8c18c0"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""03f99133-dff4-4c4a-adb1-f547afcb9b29"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard layout"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5659e169-c3b2-4851-b519-a4411608678a"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard layout"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c3322aec-49c0-408e-b44c-2f28d8d76280"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard layout"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0df732ef-2b13-4598-8a01-a760d03adcba"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard layout"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""3663a1a6-926e-49e9-9f26-8f5202c4d5ee"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InteractEnter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7f0b9cea-08b0-4028-aae1-deb0972fb5ce"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InteractEnter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cd041990-854f-452c-931d-e5be9097cb74"",
                    ""path"": ""<XInputController>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InteractEnter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""df54f071-a062-49d7-bcc0-9484a37e16d7"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""04598657-7868-4d2b-a9ee-e72fc7dd84ce"",
                    ""path"": ""<XInputController>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SecondaryEnter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""71adaef3-e9e8-44fc-9868-bfbbc9f2576c"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SecondaryEnter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2f1b6172-2a4f-4950-bcaa-c5b61c4e3b73"",
                    ""path"": ""<XInputController>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SecondaryEnter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""99c63483-0679-4c70-b922-c0067b020361"",
                    ""path"": ""<Keyboard>/u"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard layout;Joystick"",
                    ""action"": ""Oui"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7a9b9cef-1321-4b7c-8cb1-64589c0affb5"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InteractLeave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5eff87a9-10bb-439e-b6e2-d9afb55ba2a0"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InteractLeave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f505f1e3-d6e0-4717-97fc-56d426f6aff6"",
                    ""path"": ""<XInputController>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InteractLeave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UIControls"",
            ""id"": ""44791381-c224-43ef-bb2b-cdfe82673f97"",
            ""actions"": [
                {
                    ""name"": ""Selection"",
                    ""type"": ""Value"",
                    ""id"": ""2982ba4a-65d7-4dfb-979e-0d580b3be8c0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Confirm"",
                    ""type"": ""Button"",
                    ""id"": ""9ab8c876-ce48-4b91-b01c-3845fae8d493"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""87c7970e-b5e9-4c4a-92b1-4b752ae4915c"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Selection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f8372ac2-1dff-4762-a1ab-bb6ec3851922"",
                    ""path"": ""<XInputController>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Selection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7194878-2658-46aa-9ebc-3a01adadfcf9"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard layout"",
            ""bindingGroup"": ""Keyboard layout"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Joystick"",
            ""bindingGroup"": ""Joystick"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_InteractEnter = m_Player.FindAction("InteractEnter", throwIfNotFound: true);
        m_Player_InteractLeave = m_Player.FindAction("InteractLeave", throwIfNotFound: true);
        m_Player_Sprint = m_Player.FindAction("Sprint", throwIfNotFound: true);
        m_Player_SecondaryEnter = m_Player.FindAction("SecondaryEnter", throwIfNotFound: true);
        m_Player_Oui = m_Player.FindAction("Oui", throwIfNotFound: true);
        // UIControls
        m_UIControls = asset.FindActionMap("UIControls", throwIfNotFound: true);
        m_UIControls_Selection = m_UIControls.FindAction("Selection", throwIfNotFound: true);
        m_UIControls_Confirm = m_UIControls.FindAction("Confirm", throwIfNotFound: true);
    }

    public void Dispose()
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

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_InteractEnter;
    private readonly InputAction m_Player_InteractLeave;
    private readonly InputAction m_Player_Sprint;
    private readonly InputAction m_Player_SecondaryEnter;
    private readonly InputAction m_Player_Oui;
    public struct PlayerActions
    {
        private @InputManager m_Wrapper;
        public PlayerActions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @InteractEnter => m_Wrapper.m_Player_InteractEnter;
        public InputAction @InteractLeave => m_Wrapper.m_Player_InteractLeave;
        public InputAction @Sprint => m_Wrapper.m_Player_Sprint;
        public InputAction @SecondaryEnter => m_Wrapper.m_Player_SecondaryEnter;
        public InputAction @Oui => m_Wrapper.m_Player_Oui;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @InteractEnter.started += instance.OnInteractEnter;
            @InteractEnter.performed += instance.OnInteractEnter;
            @InteractEnter.canceled += instance.OnInteractEnter;
            @InteractLeave.started += instance.OnInteractLeave;
            @InteractLeave.performed += instance.OnInteractLeave;
            @InteractLeave.canceled += instance.OnInteractLeave;
            @Sprint.started += instance.OnSprint;
            @Sprint.performed += instance.OnSprint;
            @Sprint.canceled += instance.OnSprint;
            @SecondaryEnter.started += instance.OnSecondaryEnter;
            @SecondaryEnter.performed += instance.OnSecondaryEnter;
            @SecondaryEnter.canceled += instance.OnSecondaryEnter;
            @Oui.started += instance.OnOui;
            @Oui.performed += instance.OnOui;
            @Oui.canceled += instance.OnOui;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @InteractEnter.started -= instance.OnInteractEnter;
            @InteractEnter.performed -= instance.OnInteractEnter;
            @InteractEnter.canceled -= instance.OnInteractEnter;
            @InteractLeave.started -= instance.OnInteractLeave;
            @InteractLeave.performed -= instance.OnInteractLeave;
            @InteractLeave.canceled -= instance.OnInteractLeave;
            @Sprint.started -= instance.OnSprint;
            @Sprint.performed -= instance.OnSprint;
            @Sprint.canceled -= instance.OnSprint;
            @SecondaryEnter.started -= instance.OnSecondaryEnter;
            @SecondaryEnter.performed -= instance.OnSecondaryEnter;
            @SecondaryEnter.canceled -= instance.OnSecondaryEnter;
            @Oui.started -= instance.OnOui;
            @Oui.performed -= instance.OnOui;
            @Oui.canceled -= instance.OnOui;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // UIControls
    private readonly InputActionMap m_UIControls;
    private List<IUIControlsActions> m_UIControlsActionsCallbackInterfaces = new List<IUIControlsActions>();
    private readonly InputAction m_UIControls_Selection;
    private readonly InputAction m_UIControls_Confirm;
    public struct UIControlsActions
    {
        private @InputManager m_Wrapper;
        public UIControlsActions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @Selection => m_Wrapper.m_UIControls_Selection;
        public InputAction @Confirm => m_Wrapper.m_UIControls_Confirm;
        public InputActionMap Get() { return m_Wrapper.m_UIControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIControlsActions set) { return set.Get(); }
        public void AddCallbacks(IUIControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_UIControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIControlsActionsCallbackInterfaces.Add(instance);
            @Selection.started += instance.OnSelection;
            @Selection.performed += instance.OnSelection;
            @Selection.canceled += instance.OnSelection;
            @Confirm.started += instance.OnConfirm;
            @Confirm.performed += instance.OnConfirm;
            @Confirm.canceled += instance.OnConfirm;
        }

        private void UnregisterCallbacks(IUIControlsActions instance)
        {
            @Selection.started -= instance.OnSelection;
            @Selection.performed -= instance.OnSelection;
            @Selection.canceled -= instance.OnSelection;
            @Confirm.started -= instance.OnConfirm;
            @Confirm.performed -= instance.OnConfirm;
            @Confirm.canceled -= instance.OnConfirm;
        }

        public void RemoveCallbacks(IUIControlsActions instance)
        {
            if (m_Wrapper.m_UIControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_UIControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIControlsActions @UIControls => new UIControlsActions(this);
    private int m_KeyboardlayoutSchemeIndex = -1;
    public InputControlScheme KeyboardlayoutScheme
    {
        get
        {
            if (m_KeyboardlayoutSchemeIndex == -1) m_KeyboardlayoutSchemeIndex = asset.FindControlSchemeIndex("Keyboard layout");
            return asset.controlSchemes[m_KeyboardlayoutSchemeIndex];
        }
    }
    private int m_JoystickSchemeIndex = -1;
    public InputControlScheme JoystickScheme
    {
        get
        {
            if (m_JoystickSchemeIndex == -1) m_JoystickSchemeIndex = asset.FindControlSchemeIndex("Joystick");
            return asset.controlSchemes[m_JoystickSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnInteractEnter(InputAction.CallbackContext context);
        void OnInteractLeave(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnSecondaryEnter(InputAction.CallbackContext context);
        void OnOui(InputAction.CallbackContext context);
    }
    public interface IUIControlsActions
    {
        void OnSelection(InputAction.CallbackContext context);
        void OnConfirm(InputAction.CallbackContext context);
    }
}
