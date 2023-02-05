using Assets.Scripts.GameWorld;
using Assets.Scripts.Extensions;
using Character;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameController : MonoBehaviour
{
    public bool IsGameOver { get; private set; } = false;
    private AudioSource _audio;
    private Vector3 _savedPlayerVelocity;

    [SerializeField]
    private PlayerController _player;

    [SerializeField]
    private GameObject GameOverUI;

    [SerializeField]
    private GameObject PausedUI;

    [SerializeField]
    private AudioClip PlayMusic;

    [SerializeField]
    private AudioClip GameOverMusic;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        _audio.clip = PlayMusic;
        _audio.volume = 0.5f;
        _audio.loop = true;
        _audio.Play();

        foreach (var obj in GameObject.FindGameObjectsWithTag("PlatformSpecific"))
        {
            if (obj.TryGetComponent<UnityEngine.UI.Text>(out var text))
            {
                text.ReplacePlatformText();
            }
        }

        foreach (var btn in GameObject.FindGameObjectsWithTag("QuitImage"))
        {
            if (InputSystem.devices.Where(x => x is DualShock4GamepadHID).Any())
            {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("PS4/PS4_Triangle");
            }
            else if (Gamepad.all.Any())
            {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Xbox/Xbox_Y");
            }
            else
            {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Keyboard/Esc_Key_Light");
            }
        }


        GameOverUI.SetActive(false);
        PausedUI.SetActive(false);
        enabled = false;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void OnApplicationQuit()
    {
        InputSystem.ResetHaptics();
        if (Gamepad.current is DualShock4GamepadHID) DualShockGamepad.current.SetLightBarColor(Color.clear);
    }

    public void PauseGame()
    {
        InputSystem.PauseHaptics();

        if (!IsGameOver)
        {
            _audio.volume = 0.33f;
            PausedUI.SetActive(true);
        }

        var playerRB = _player.GetComponent<Rigidbody2D>();
        _savedPlayerVelocity = playerRB.velocity;
        playerRB.velocity = Vector3.zero;
        
        _player.enabled = false;

        foreach(var enemy in FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
        {
            enemy.StopAllCoroutines();
            enemy.enabled = false;
        }

        foreach(var tile in FindObjectsByType<MapTileController>(FindObjectsSortMode.None))
        {
            tile.enabled = false;
        }
    }

    public void ResumeGame()
    {
        PausedUI.SetActive(false);

        _audio.volume = 0.5f;
        _player.enabled = true;
        _player.GetComponent<Rigidbody2D>().velocity = _savedPlayerVelocity;

        foreach (var enemy in FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
        {
            enemy.enabled = true;
        }

        foreach (var tile in FindObjectsByType<MapTileController>(FindObjectsSortMode.None))
        {
            tile.enabled = true;
        }

        InputSystem.ResumeHaptics();
    }

    public void GameOver()
    {
        IsGameOver = true;

        PauseGame();

        _audio.clip = GameOverMusic;
        _audio.Play();

        GameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        InputSystem.ResetHaptics();
        if (Gamepad.current is DualShock4GamepadHID) DualShockGamepad.current.SetLightBarColor(Color.clear);

        IsGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
