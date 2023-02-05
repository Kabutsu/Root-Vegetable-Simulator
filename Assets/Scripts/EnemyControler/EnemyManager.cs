using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public int MaxEnemies { get; private set; } = 10;

    private int _enemies;
    private int _score;

    [SerializeField]
    private Text _scoreText;

    // Start is called before the first frame update
    void Start()
    {
        _enemies = 0;
        _score = 0;
    }

    public int SetMaxEnemies(int newMax)
    {
        MaxEnemies = newMax;
        return MaxEnemies;
    }

    public bool Register()
    {
        if (_enemies < MaxEnemies)
        {
            _enemies++;
            return true;
        }

        return false;
    }

    public void DeRegister()
    {
        _enemies--;
        _score++;

        _scoreText.text = _score.ToString();

        if (_score % 3 == 0) MaxEnemies += 3;
    }
}
