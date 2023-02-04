using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float DmgPerFrame = 0.1f;
    public bool Staggered = false;

    private PlayerController _goal;
    private EnemyManager _enemyManager;
    private Rigidbody2D _rigidBody;
    private Vector3 _directionToPlayer;
    private Vector3 _localScale;
    private Vector3 _fluctuation;
    private float _fluctuationBound;

    [SerializeField]
    private float Health = 2.5f;

    [SerializeField]
    private float StaggerCooldown = 0.75f;

    [SerializeField]
    private float MovementSpeed = 1.5f;


    // Start is called before the first frame update
    void Start()
    {
        _goal = FindObjectOfType<PlayerController>();
        _enemyManager = FindObjectOfType<EnemyManager>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _localScale = transform.localScale;

        _fluctuationBound = _goal.GetComponent<CircleCollider2D>().radius;

        _fluctuation = new Vector3(
            Random.Range(-_fluctuationBound, _fluctuationBound),
            Random.Range(-_fluctuationBound, _fluctuationBound),
            0.0f
        );
    }

    private void FixedUpdate()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        Vector3 goalPosition = _goal.transform.position + _fluctuation;

        _directionToPlayer = (goalPosition - transform.position).normalized;
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
        _enemyManager.DeRegister();
        Destroy(gameObject);
    }

    private IEnumerator Stagger()
    {
        Staggered = true;

        yield return new WaitForSeconds(StaggerCooldown);

        Staggered = false;
    }
}
