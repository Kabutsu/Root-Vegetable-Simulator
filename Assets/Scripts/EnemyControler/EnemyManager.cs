using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int MaxEnemies { get; private set; } = 10;
    private int _enemies;

    // Start is called before the first frame update
    void Start()
    {
        _enemies = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }
}
