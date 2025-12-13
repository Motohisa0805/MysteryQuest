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
        eUpSelect,
        eDownSelect,
        eLeftSelect,
        eRightSelect,
        eDecide,
        //eESC,
        eF1,
        Num1,
        Num2
    };
    // InputManagerクラス
    public class InputManager
    {
        private static InputSystem_Actions mInputAction;


        private static List<InputAction> mButtonActions = new List<InputAction>();

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
            mButtonActions.Add(mInputAction.UI.Select_Up);
            mButtonActions.Add(mInputAction.UI.Select_Down);
            mButtonActions.Add(mInputAction.UI.Select_Left);
            mButtonActions.Add(mInputAction.UI.Select_Right);
            mButtonActions.Add(mInputAction.UI.Decide);
            /*
            mButtonActions.Add(mInputAction.UI.Option);
             */
            mButtonActions.Add(mInputAction.Debug.OnOff);
            mButtonActions.Add(mInputAction.Debug.CreateWoodBox);
            mButtonActions.Add(mInputAction.Debug.CreateWood);
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
