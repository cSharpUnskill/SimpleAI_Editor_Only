using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ziggurat
{
    public class PlayerMoving : MonoBehaviour
    {
        [SerializeField]
        private float _movingSpeed;

        [SerializeField]
        private float lookSpeedH = 2f;

        [SerializeField]
        private float lookSpeedV = 2f;

        [SerializeField]
        private float zoomSpeed = 2f;

        [SerializeField]
        private float dragSpeed = 6f;

        [SerializeField]
        private TopMenuManager topMenuManager;

        private float _horizontal = 0f;
        private float _vertical = 0f;

        public ZigguratClass _currentZiggurat;


        void Update()
        {
            Click();

            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * _movingSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(-Vector3.right * _movingSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(-Vector3.forward * _movingSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * _movingSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                _horizontal += lookSpeedH * Input.GetAxis("Mouse X");
                _vertical -= lookSpeedV * Input.GetAxis("Mouse Y");

                _vertical = Mathf.Clamp(_vertical, -90f, 90f);

                transform.eulerAngles = new Vector3(_vertical, _horizontal, 0f);
            }

            if (Input.GetMouseButton(2))
            {
                transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
            }

            transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Space.Self);
        }

        public void Click()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    var ziggurat = hit.transform.GetComponent<ZigguratClass>();

                    if (ziggurat)
                    {
                        topMenuManager.ZigguratClick(ziggurat);
                    }
                }
            }
        }
    }
}
