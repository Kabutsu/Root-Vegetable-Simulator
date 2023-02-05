using Assets.Scripts.GameWorld;
using Character;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public bool IsGameOver { get; private set; } = false;
    private Vector3 _savedPlayerVelocity;

    [SerializeField]
    private PlayerController _player;

    [SerializeField]
    private GameObject GameOverUI;

    [SerializeField]
    private AudioSource Audio;

    [SerializeField]
    private AudioClip PlayMusic;

    [SerializeField]
    private AudioClip GameOverMusic;

    private void Awake()
    {
        Audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        Audio.clip = PlayMusic;
        Audio.volume = 0.5f;
        Audio.loop = true;
        Audio.Play();

        GameOverUI.SetActive(false);
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
        Audio.volume = 0.33f;

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
        Audio.volume = 0.5f;
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

        Audio.clip = GameOverMusic;
        Audio.volume = 0.5f;
        Audio.Play();

        foreach(var obj in GameObject.FindGameObjectsWithTag("PlatformSpecific"))
        {
            if (obj.TryGetComponent<UnityEngine.UI.Text>(out var text))
            {
                text.text = text.text.Replace("$", GetPlatformSpecificQuickActionButton());
            }
        }

        GameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        InputSystem.ResetHaptics();
        if (Gamepad.current is DualShock4GamepadHID) DualShockGamepad.current.SetLightBarColor(Color.clear);

        IsGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private string GetPlatformSpecificQuickActionButton()
    {
        if (InputSystem.devices.Where(x => x is DualShock4GamepadHID).Any())
        {
            return "X";
        }

        if (Gamepad.all.Any())
        {
            return "A";
        }

        return "Space";
    }
}
