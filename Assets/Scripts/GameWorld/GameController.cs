using Assets.Scripts.GameWorld;
using Character;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private PlayerController _player;

    [SerializeField]
    private GameObject GameOverUI;

    private Vector3 _savedPlayerVelocity;

    // Start is called before the first frame update
    void Start()
    {
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

    public void PauseGame()
    {
        InputSystem.PauseHaptics();

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
        PauseGame();
        GameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        InputSystem.ResetHaptics();
        if (Gamepad.current is DualShock4GamepadHID) DualShockGamepad.current.SetLightBarColor(Color.clear);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
