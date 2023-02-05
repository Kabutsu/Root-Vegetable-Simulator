using UnityEngine;
using UnityEngine.InputSystem;

public class MenuControlsInputs : MonoBehaviour
{
    public void OnQuickAction(InputValue value)
    {
        FindObjectOfType<MainMenuController>().StartGame();
    }
}
