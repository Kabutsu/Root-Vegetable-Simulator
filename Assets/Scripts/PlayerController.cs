using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace Character
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public float _speed = 5f;
        private CharacterController _controller;


        // Start is called before the first frame update
        void Start()
        {
            _controller = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            float vTranslate = Input.GetAxis("Vertical") * _speed * Time.deltaTime;
            float hTranslate = Input.GetAxis("Horizontal") * _speed * Time.deltaTime;

            _controller.Move(new Vector3(hTranslate, vTranslate, 0));
        }
    }
}
