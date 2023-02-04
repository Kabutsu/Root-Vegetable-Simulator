using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using Assets.Scripts.Extensions;
using System.Linq;

namespace Character
{
    public class PlayerController : MonoBehaviour
    {
        public float MoveSpeed = 5f;
        public float DashSpeed = 15f;
        public float SpeedChangeRate = 12.5f;
        public float Health = 100f;

        public float BGSlowDown = 100f;

        private Rigidbody2D _rigidBody;
        private InputControlsInputs _input;
        private ScrollBackground _background;
        private Image _healthSliderImage;
        private float _speed;

        private bool _isPaused = false;

        [SerializeField]
        private float DamageThreshold = 7.5f;

        [SerializeField]
        private Slider HealthSlider;

        // Start is called before the first frame update
        void Start()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _input = GetComponent<InputControlsInputs>();
            _background = GetComponentInChildren<ScrollBackground>();

            _healthSliderImage = HealthSlider
                .GetComponentsInChildren<Image>()
                .Where(x => x.name.Contains("Fill"))
                .FirstOrDefault();
        }

        private void FixedUpdate()
        {
            Move();
        }

        public void Pause() => _isPaused = true;
        public void Resume() => _isPaused = false;

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.dash ? DashSpeed : MoveSpeed;

            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }
            // if there is an input, move the background
            else
            {
                
            }

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = _rigidBody.velocity.magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.dash ? 1 : _input.move.magnitude;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector2 offset = _input.move * (_speed * Time.deltaTime / BGSlowDown);

            if (_rigidBody.velocity.x == 0) offset.x = 0;
            if (_rigidBody.velocity.y == 0) offset.y = 0;

            _background.Move(offset);

            // set player's velocity
            _rigidBody.AddForce(_input.move * _speed);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if(!_isPaused)
            {
                var enemy = collision.gameObject.GetComponent<EnemyController>();

                if (enemy != null)
                {
                    var myMomentum = _rigidBody.velocity.magnitude * _rigidBody.mass;
            
                    if (_rigidBody.velocity.magnitude >= DamageThreshold)
                    {
                        enemy.WasHit(myMomentum, _rigidBody.velocity);
                    }
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!_isPaused)
            {
                var enemy = collision.gameObject.GetComponent<EnemyController>();
                if (enemy != null && !enemy.Staggered && _rigidBody.velocity.magnitude < DamageThreshold)
                {
                    Health -= enemy.DmgPerFrame;
                    HealthSlider.value = Mathf.Max(Health, 0f);
                    _healthSliderImage.LerpColor3(Color.green, Color.yellow, Color.red, 0.5f, Health / 100f);

                    if (Health <= 0f)
                    {
                        FindObjectOfType<GameController>().GameOver();
                    }
                }
            }
        }
    }
}
