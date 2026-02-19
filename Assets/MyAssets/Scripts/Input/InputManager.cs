using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace MyAssets
{
    // ボタンの状態
    public enum ButtonState
    {
        eNone,
        ePressed,
        eReleased,
        eHeld
    };
    // キーコードの列挙型
    public enum KeyCode
    {
        eNone = -1,
        eMove,
        eLook,
        eAttack,
        eInteract,
        eCrouch,
        eJump,
        ePrevious,
        eNext,
        eSprint,
        eThrow,
        eFocusing,
        eSkill,
        eUpSelect,
        eDownSelect,
        eLeftSelect,
        eRightSelect,
        eDecide,
        eMenu,
        eTutorialMenu,
        eF1,
        Num1,
        Num2,
        Num3
    };
    // InputManagerクラス
    public class InputManager
    {
        private static InputSystem_Actions  mInputAction;

        public static InputSystem_Actions   InputActions => mInputAction;

        private static List<InputAction>    mButtonActions = new List<InputAction>();

        private static string               mCurrentControlScheme = "Keyboard&Mouse"; // デフォルト
        public static string                CurrentControlScheme => mCurrentControlScheme;

        public static bool                  IsCurrentControlSchemeKeyBoard => mCurrentControlScheme == "Keyboard&Mouse";

        public static System.Action<string> OnControlSchemeChanged;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (mInputAction != null)
            {
                return; // Already initialized
            }
            // Constructor logic if needed
            // Initialize the Input System
            mInputAction = new InputSystem_Actions();
            mInputAction.Enable();

            InputSystem.onActionChange += (obj, change) =>
            {
                if(change == InputActionChange.ActionStarted || change == InputActionChange.ActionPerformed)
                {
                    var action = (InputAction)obj;
                    if(action.activeControl != null)
                    {
                        //操作されたデバイスから、対応するスキーム名を特定
                        DetermineControlScheme(action.activeControl.device);
                    }
                }
            };

            mButtonActions.Add(mInputAction.Player.Move);
            mButtonActions.Add(mInputAction.Player.Look);
            mButtonActions.Add(mInputAction.Player.Attack);
            mButtonActions.Add(mInputAction.Player.Interact);
            mButtonActions.Add(mInputAction.Player.Crouch);
            mButtonActions.Add(mInputAction.Player.Jump);
            mButtonActions.Add(mInputAction.Player.Previous);
            mButtonActions.Add(mInputAction.Player.Next);
            mButtonActions.Add(mInputAction.Player.Sprint);
            mButtonActions.Add(mInputAction.Player.Throw);
            mButtonActions.Add(mInputAction.Player.Focusing);
            mButtonActions.Add(mInputAction.Player.Skill);

            mButtonActions.Add(mInputAction.UI.Select_Up);
            mButtonActions.Add(mInputAction.UI.Select_Down);
            mButtonActions.Add(mInputAction.UI.Select_Left);
            mButtonActions.Add(mInputAction.UI.Select_Right);
            mButtonActions.Add(mInputAction.UI.Decide);
            mButtonActions.Add(mInputAction.UI.Menu);
            mButtonActions.Add(mInputAction.UI.TutorialMenu);
            mButtonActions.Add(mInputAction.Debug.OnOff);
            mButtonActions.Add(mInputAction.Debug.CreateWoodBox);
            mButtonActions.Add(mInputAction.Debug.CreateWood);
            mButtonActions.Add(mInputAction.Debug.CreateCombustible);
        }

        // デバイスからコントロールスキーム名を特定するメソッド
        private static void DetermineControlScheme(InputDevice device)
        {
            if (device is Gamepad gamepad)
            {
                // 1. スティックが一定以上動いているかチェック
                bool isStickMoving = gamepad.rightStick.ReadValue().magnitude > 0.1f ||
                                     gamepad.leftStick.ReadValue().magnitude > 0.1f;

                // 2. いずれかのボタンが押されているかチェック
                bool isAnyButtonPressed = false;
                foreach (var control in gamepad.allControls)
                {
                    // コントロールがボタン（トリガー等を含む）であり、かつ押されている場合
                    if (control is UnityEngine.InputSystem.Controls.ButtonControl button)
                    {
                        if (button.isPressed)
                        {
                            isAnyButtonPressed = true;
                            break;
                        }
                    }
                }

                // スティックも動いておらず、ボタンも押されていない（＝微小なドリフトのみ）なら処理を中断
                if (!isStickMoving && !isAnyButtonPressed)
                {
                    return;
                }
            }
            foreach (var scheme in mInputAction.controlSchemes)
            {
                // デバイスがそのスキームの要件（Binding）に合致するかチェック
                if (scheme.SupportsDevice(device))
                {
                    if(mCurrentControlScheme != scheme.name)
                    {
                        mCurrentControlScheme = scheme.name;
                        OnControlSchemeChanged?.Invoke(mCurrentControlScheme);
                    }
                    break;
                }
            }
        }

        public static void Shutdown()
        {
            mButtonActions.Clear();
            // Destructor logic if needed
            // Disable the Input System
            mInputAction.Disable();
        }

        public static void SetLockedMouseMode()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void SetNoneMouseMode()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public static bool GetKey(KeyCode code)
        {
            if (mButtonActions.Count <= 0) { return false; }
            if (mButtonActions[(int)code] == null) { return false; }
            return mButtonActions[(int)code].IsPressed();
        }

        public static bool GetKeyDown(KeyCode code)
        {
            if (mButtonActions.Count <= 0) { return false; }
            if (mButtonActions[(int)code] == null) { return false; }
            return mButtonActions[(int)code].WasPressedThisFrame();
        }

        public static bool GetKeyUp(KeyCode code)
        {
            if (mButtonActions.Count <= 0) { return false; }
            if (mButtonActions[(int)code] == null) { return false; }
            return mButtonActions[(int)code].WasReleasedThisFrame();
        }

        public static Vector2 GetKeyValue(KeyCode code)
        {
            if (mButtonActions.Count <= 0) { return Vector2.zero; }
            if (mButtonActions[(int)code] == null) { return Vector2.zero; }
            return mButtonActions[(int)code].ReadValue<Vector2>();
        }
    }
}
