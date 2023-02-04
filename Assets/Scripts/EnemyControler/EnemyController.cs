using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float DmgPerFrame = 0.1f;
    public bool Staggered = false;

    private PlayerController _goal;
    private Rigidbody2D _rigidBody;
    private Vector3 _directionToPlayer;
    private Vector3 _localScale;

    [SerializeField]
    private float Health = 2.5f;

    [SerializeField]
    private float StaggerCooldown = 0.75f;

    [SerializeField]
    private float MovementSpeed = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _goal = FindObjectOfType<PlayerController>();
        _localScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        _directionToPlayer = (_goal.transform.position - transform.position).normalized;
        transform.Translate(new Vector2(_directionToPlayer.x, _directionToPlayer.y) * MovementSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (_rigidBody.velocity.x > 0)
        {
            transform.localScale = new Vector3(_localScale.x, _localScale.y, _localScale.z);
        }
        else if (_rigidBody.velocity.x < 0)
        {
            transform.localScale = new Vector3(-_localScale.x, _localScale.y, _localScale.z);
        }
    }

    public void WasHit(float momentum)
    {
        Health -= momentum;

        if (Health <= 0.0f)
        {
            Despawn();
            return;
        }

        StartCoroutine(Stagger());
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

    private IEnumerator Stagger()
    {
        Staggered = true;

        yield return new WaitForSeconds(StaggerCooldown);

        Staggered = false;
    }
}
