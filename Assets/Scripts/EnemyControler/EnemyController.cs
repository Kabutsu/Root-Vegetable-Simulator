using Character;
using System.Collections;
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

        var playerSpriteSize = _goal.GetComponent<SpriteRenderer>().size / 2f;
        _fluctuationBound = Mathf.Max(playerSpriteSize.x, playerSpriteSize.y);

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
        if (!Staggered)
        {
            Vector3 goalPosition = _goal.transform.position + _fluctuation;

            _directionToPlayer = (goalPosition - transform.position).normalized;
            transform.Translate(new Vector2(_directionToPlayer.x, _directionToPlayer.y) * MovementSpeed * Time.deltaTime);
        }
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

    public void WasHit(float momentum, Vector2 velocity)
    {
        Health -= momentum;

        var direction = velocity.normalized;
        var distance = (direction * momentum * StaggerCooldown) / 2f;
        StartCoroutine(Stagger(distance));
    }

    private void Despawn()
    {
        _enemyManager.DeRegister();
        Destroy(gameObject);
    }

    private IEnumerator Stagger(Vector2 byDistance)
    {
        Staggered = true;

        Vector3 startingPos = transform.position;
        Vector3 finalPos = transform.position + new Vector3(byDistance.x, byDistance.y, transform.position.z);

        float elapsed = 0;

        while (elapsed < (StaggerCooldown / 2f))
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (2f * elapsed) / StaggerCooldown);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (Health <= 0.0f)
        {
            Despawn();
        }
        else
        {
            yield return new WaitForSeconds(StaggerCooldown / 2f);
            Staggered = false;
        }
    }
}
