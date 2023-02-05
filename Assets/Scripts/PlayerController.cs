using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using Assets.Scripts;
using Assets.Scripts.Extensions;
using System.Linq;

namespace Character
{
    public class PlayerController : MonoBehaviour
    {
        public Vector2 DashRumble = new Vector2(0.75f, 0.25f);
        public Vector2 HurtingRumble = new Vector2(0.25f, 0.75f);
        public float MoveSpeed = 5f;
        public float DashSpeed = 15f;
        public float SpeedChangeRate = 12.5f;
        public float Health = 100f;

        public float BGSlowDown = 100f;

        private Rigidbody2D _rigidBody;
        private InputControlsInputs _input;
        private ScrollBackground _background;
        private Image _healthSliderImage;
        private AudioSource _audio;
        private float _speed;
        private int _currentAttackers = 0;

        private bool _isPaused = false;
        private bool _isDualshock;

        [SerializeField]
        private float DamageThreshold = 7.5f;

        [SerializeField]
        private Slider HealthSlider;

        [SerializeField]
        private AudioClip[] DashSound = new AudioClip[3];

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }

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

            _isDualshock = Gamepad.current is DualShock4GamepadHID;

            if (_isDualshock) DualShock4GamepadHID.current.SetLightBarColor(Color.green);

            ManageRumble();
        }

        private void FixedUpdate()
        {
            Move();
        }

        public void Pause() => _isPaused = true;
        public void Resume() => _isPaused = false;

        public void ManageRumble()
        {
            if (_isPaused || Gamepad.current == null) return;

            if(_input.dash)
            {
                SetRumble(DashRumble.x, DashRumble.y);
            }
            else if (_currentAttackers > 0)
            {
                SetRumble(HurtingRumble.x, HurtingRumble.y);
            }
            else
            {
                SetRumble(0f, 0f);
            }
        }

        public void PlaySound()
        {
            _audio.PlayOneShot(DashSound[Random.Range(0, 2)]);
        }

        private void SetRumble(float lowFrequency, float highFrequency)
        {
            if (_isDualshock)
            {
                DualShock4GamepadHID.current.SetMotorSpeeds(lowFrequency, highFrequency);
            }
            else
            {
                Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);
            }
        }

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
            if(!_isPaused && collision.gameObject.TryGetComponent<EnemyController>(out var enemy))
            {
                _currentAttackers++;

                ManageRumble();

                var myMomentum = _rigidBody.velocity.magnitude * _rigidBody.mass;
            
                if (_rigidBody.velocity.magnitude >= DamageThreshold)
                {
                    enemy.WasHit(myMomentum, _rigidBody.velocity);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!_isPaused
                && collision.gameObject.TryGetComponent<EnemyController>(out var enemy)
                && !enemy.Staggered
                && _rigidBody.velocity.magnitude < DamageThreshold)
            {
                Health -= enemy.DmgPerFrame;
                HealthSlider.value = Mathf.Max(Health, 0f);

                Color healthColor = DataExtensions.LerpColor3(Color.green, Color.yellow, Color.red, 0.5f, Health / 100f);
                _healthSliderImage.color = healthColor;
                if (_isDualshock) DualShock4GamepadHID.current.SetLightBarColor(healthColor);

                if (Health <= 0f)
                {
                    FindObjectOfType<GameController>().GameOver();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.gameObject.TryGetComponent<EnemyController>(out var enemy))
            {
                _currentAttackers--;

                ManageRumble();
            }
        }
    }
}
