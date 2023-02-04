using UnityEngine;
using Assets.Scripts.Extensions;

namespace Assets.Scripts.GameWorld
{
    public class MapTileController : MonoBehaviour
    {
        public float MinSpawnTime = 0.5f;
        public float MaxSpawnTime = 2.5f;

        private EnemyManager _enemyManager;
        private BoxCollider2D _spawnArea;
        private float _cooldown;
        private bool _canCooldown = true;

        [SerializeField]
        private GameObject EnemyObject;

        private void Start()
        {
            _enemyManager = FindObjectOfType<EnemyManager>();
            _spawnArea = GetComponent<BoxCollider2D>();

            ResetCooldown();
        }

        private void Update()
        {
            if (_canCooldown)
            {
                _cooldown -= Time.deltaTime;

                if (_cooldown <= 0f) Spawn();
            }
        }

        private void ResetCooldown()
        {
            _cooldown = Random.Range(MinSpawnTime, MaxSpawnTime);
        }

        private void Spawn()
        {
            Vector2 point = _spawnArea.GetRandomPointInsideCollider();
            Vector3 pointOnCamera = Camera.main.WorldToViewportPoint(new Vector3(point.x, point.y, 0.0f));

            if ((pointOnCamera.x < 0 || pointOnCamera.x > 1 || pointOnCamera.y < 0 || pointOnCamera.y > 1)
                && _enemyManager.Register())
            {
                Instantiate(EnemyObject, point, transform.rotation);
            }

            ResetCooldown();
        }
    }
}
