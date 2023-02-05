using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

namespace Assets.Scripts.Extensions
{
    public static class UIExtensions
    {
        public static void ReplacePlatformText(this UnityEngine.UI.Text text)
        {
            text.text = text.text
                .Replace("%", GetPlatformSpecificButton(ButtonType.Start))
                .Replace("$", GetPlatformSpecificButton(ButtonType.QuickAction));
        }

        private static string GetPlatformSpecificButton(ButtonType button)
        {
            if (InputSystem.devices.Where(x => x is DualShock4GamepadHID).Any())
            {
                return button switch
                {
                    ButtonType.QuickAction => "X",
                    ButtonType.Start => "Options"
                };
            }
            else if (Gamepad.all.Any())
            {
                return button switch
                {
                    ButtonType.QuickAction => "A",
                    ButtonType.Start => "Menu"
                };
            }
            else
            {
                return button switch
                {
                    ButtonType.QuickAction => "Space",
                    ButtonType.Start => "Enter"
                };
            }
        }
    }

    public enum ButtonType
    {
        Start = 0,
        QuickAction = 1
    }
}
