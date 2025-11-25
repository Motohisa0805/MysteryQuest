using UnityEngine;

namespace MyAssets
{
    [RequireComponent(typeof(PlayableChracterController))]
    public class PlayableInput : MonoBehaviour
    {
        private Vector2 mInputMove; //“ü—Í•ûŒü

        public Vector2  InputMove => mInputMove;


        private bool    mInteract;
        public bool     Interact => mInteract;

        private bool    mSprit;

        public bool     Sprit => mSprit;

        private bool    mInputJump; //ƒWƒƒƒ“ƒv“ü—Í

        public bool     InputJump => mInputJump;


        private bool    mInputCrouch;

        public bool     InputCrouch => mInputCrouch;

        private void Start()
        {
            mInputMove = Vector2.zero;
            mSprit = false;
            mInputJump = false;
            mInputCrouch = false;
        }

        // Update is called once per frame
        private void Update()
        {
            mInputMove = InputManager.GetKeyValue(KeyCode.eMove);

            mInteract = InputManager.GetKeyDown(KeyCode.eInteract);

            mSprit = InputManager.GetKey(KeyCode.eSprint);

            mInputJump = InputManager.GetKeyDown(KeyCode.eJump);

            mInputCrouch = InputManager.GetKeyDown(KeyCode.eCrouch);
        }
    }
}
