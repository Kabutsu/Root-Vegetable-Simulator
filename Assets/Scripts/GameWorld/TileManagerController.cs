using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManagerController : MonoBehaviour
{
    private CircleCollider2D _safeArea;
    private Vector3 _directionToPlayer;

    void Start()
    {
        _safeArea = GetComponent<CircleCollider2D>();
    }


    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out var player))
        {
            _directionToPlayer = (player.transform.position - transform.position).normalized;
            transform.Translate(_directionToPlayer * 60);
        }
    }
}
