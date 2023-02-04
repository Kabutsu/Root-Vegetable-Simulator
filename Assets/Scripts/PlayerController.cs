using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Scripts;

namespace Character
{
    public class PlayerController : MonoBehaviour
    {
        public float MoveSpeed = 5f;
        public float DashSpeed = 15f;
        public float SpeedChangeRate = 12.5f;

        private Rigidbody2D _rigidBody;
        private InputControlsInputs _input;
        private float _speed;


        // Start is called before the first frame update
        void Start()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _input = GetComponent<InputControlsInputs>();
        }

        // Update is called once per frame
        void Update()
        {
            Move();
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.dash ? DashSpeed : MoveSpeed;

            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

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

            // set player's velocity
            _rigidBody.AddForce(_input.move * _speed);
            //transform.Translate(_input.move * _speed * Time.deltaTime);
            //_rigidBody.velocity = _input.move * _speed;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            var myMomentum = _rigidBody.velocity.magnitude * _rigidBody.mass;
            var enMomentum = collision.attachedRigidbody.velocity.magnitude * collision.attachedRigidbody.velocity.magnitude;

            Debug.Log($"MyMomentum: ${myMomentum}");
            Debug.Log($"EnMomentum: ${enMomentum}");

            if (myMomentum >= enMomentum)
            {
                collision.attachedRigidbody.gameObject.GetComponent<EnemyController>().WasHit(myMomentum);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            //ToDo: Deal Damage
        }
    }
}
