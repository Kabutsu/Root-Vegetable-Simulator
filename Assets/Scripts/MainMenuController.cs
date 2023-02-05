using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;
using System.Linq;
using Assets.Scripts.Extensions;

public class MainMenuController : MonoBehaviour
{
    public Image MoveImage;
    public Image DashImage;
    public Image PauseImage;

    // Start is called before the first frame update
    void Start()
    {
        if (InputSystem.devices.Where(x => x is DualShock4GamepadHID).Any())
        {
            MoveImage.sprite = Resources.Load<Sprite>("PS4/PS4_Left_Stick");
            DashImage.sprite = Resources.Load<Sprite>("PS4/PS4_Cross");
            PauseImage.sprite = Resources.Load<Sprite>("PS4/PS4_Options");
        }
        else if (Gamepad.all.Any())
        {
            MoveImage.sprite = Resources.Load<Sprite>("Xbox/Xbox_Left_Stick");
            DashImage.sprite = Resources.Load<Sprite>("Xbox/Xbox_A");
            PauseImage.sprite = Resources.Load<Sprite>("Xbox/Xbox_Menu");
        }
        else
        {
            MoveImage.sprite = Resources.Load<Sprite>("Keyboard/WASD_Key_Light");
            DashImage.sprite = Resources.Load<Sprite>("Keyboard/Space_Key_Light");
            PauseImage.sprite = Resources.Load<Sprite>("Keyboard/Enter_Alt_Key_Light");
        }

        foreach (var obj in GameObject.FindGameObjectsWithTag("PlatformSpecific"))
        {
            if (obj.TryGetComponent<Text>(out var text))
            {
                text.ReplacePlatformText();
            }
        }

        enabled = false;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
