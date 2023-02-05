using Character;
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
        private bool isPaused = false;

        [SerializeField]
        private float dashDuration = 0.45f;

        [SerializeField]
        private float dashCooldown = 0.75f;

        public void OnMove(InputValue value)
        {
            move = value.Get<Vector2>();
        }

        public void OnQuickAction(InputValue value)
        {
            var gameController = FindObjectOfType<GameController>();

            if (gameController.IsGameOver)
            {
                gameController.RestartGame();
                return;
            }

            if (!dash)
            {
                dash = canDash && value.isPressed;

                if (dash) StartCoroutine(ResetDash());
            }
        }

        private IEnumerator ResetDash()
        {
            var playerController = FindObjectOfType<PlayerController>();
            playerController.PlaySound();
            playerController.ManageRumble();
            playerController.ShowDashUI(dashDuration, dashCooldown);

            canDash = false;
            yield return new WaitForSeconds(dashDuration + 0.1f);

            dash = false;

            playerController.ManageRumble();

            yield return new WaitForSeconds(dashCooldown + 0.2f);
            canDash = true;
        }

        public void OnMenu(InputValue value)
        {
            var gameController = FindObjectOfType<GameController>();

            if (gameController.IsGameOver) return;

            if (isPaused)
            {
                gameController.ResumeGame();
                isPaused = false;
            }
            else
            {
                gameController.PauseGame();
                isPaused = true;
            }
        }

        public void OnEscapeAction(InputValue value)
        {
            var gameController = FindObjectOfType<GameController>();

            if (isPaused || gameController.IsGameOver)
            {
                Application.Quit();
            }
        }
    }
}
