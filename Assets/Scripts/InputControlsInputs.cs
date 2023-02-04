using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class InputControlsInputs : MonoBehaviour
    {
        public Vector2 move;
        public bool dash;

        private bool canDash = true;

        [SerializeField]
        private float dashDuration = 0.45f;

        [SerializeField]
        private float dashCooldown = 0.75f;

        public void OnMove(InputValue value)
        {
            move = value.Get<Vector2>();
        }

        public void OnDash(InputValue value)
        {
            if (!dash)
            {
                dash = canDash && value.isPressed;

                if (dash) StartCoroutine(ResetDash());
            }
        }

        private IEnumerator ResetDash()
        {
            canDash = false;

            yield return new WaitForSeconds(dashDuration);
            dash = false;

            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }
}
