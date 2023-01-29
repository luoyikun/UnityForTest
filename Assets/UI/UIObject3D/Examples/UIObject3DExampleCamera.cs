using UnityEngine;
using System.Collections;

namespace UI.ThreeDimensional.Examples
{
    public class UIObject3DExampleCamera : MonoBehaviour
    {
        public float xRotationSpeed = 5f;
        public float yRotationSpeed = 2.5f;
        public float moveSpeed = 10f;

        private float mouseX, mouseY, mouseZ = 0;

        void Start()
        {
            mouseX = transform.rotation.eulerAngles.y;
            mouseY = transform.rotation.eulerAngles.x;
            mouseZ = transform.rotation.eulerAngles.z;
        }

        void Update()
        {
            if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                mouseX += Input.GetAxis("Mouse X") * xRotationSpeed;

                if (mouseX <= -180)
                {
                    mouseX += 360;
                }
                else if (mouseX > 180)
                {
                    mouseX -= 360;
                }

                mouseY -= Input.GetAxis("Mouse Y") * yRotationSpeed;

                if (mouseY <= -180)
                {
                    mouseY += 360;
                }
                else if (mouseY > 180)
                {
                    mouseY -= 360;
                }
            }

            transform.rotation = Quaternion.Euler(mouseY, mouseX, mouseZ);

            var _moveSpeed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _moveSpeed *= 5;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                transform.position += transform.forward * (Time.deltaTime * _moveSpeed);
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                transform.position -= transform.forward * (Time.deltaTime * _moveSpeed);
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position -= transform.right * (Time.deltaTime * _moveSpeed);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.position += transform.right * (Time.deltaTime * _moveSpeed);
            }
        }
    }
}
