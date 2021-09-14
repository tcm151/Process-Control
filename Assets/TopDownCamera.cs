using System;
using UnityEngine;


namespace ProcessControl.Control
{
    public class TopDownCamera : MonoBehaviour
    {
        public float speed;
        public float dragSpeed = 2;
        public float acceleration;

        public bool dragging;

        private Vector3 mousePosition;
        private Vector3 dragOrigin;
        private Vector3 mouseInput;
        private Vector3 cameraOrigin;
        private Vector3 movementInput;
        private Vector3 currentPosition;
        private readonly Vector3 cameraOffset = new Vector3(0, 0, -10);

        new private Camera camera;

        private void Awake()
        {
            camera = GetComponent<Camera>();
            camera.transform.position = cameraOffset;
        }

        private void Update()
        {

            if (Input.GetMouseButtonDown(2))
            {
                dragging = true;
                dragOrigin = camera.ScreenToWorldPoint(Input.mousePosition);
                dragOrigin.z = 0f;
                
                cameraOrigin = transform.position;
            }
            
            if (Input.GetMouseButtonUp(2)) dragging = false;

            mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            currentPosition = transform.position;

            mouseInput.x = Input.GetAxisRaw("Mouse X");
            mouseInput.y = Input.GetAxisRaw("Mouse Y");
            
            movementInput.x = Input.GetAxisRaw("Horizontal");
            movementInput.y = Input.GetAxisRaw("Vertical");
            movementInput.z = 0f;
            movementInput.Normalize();

            movementInput *= Time.deltaTime;
        }

        private void LateUpdate()
        {
            if (dragging)
            {
                var dragOffset = mousePosition - dragOrigin;
                transform.position -= mouseInput * (dragSpeed * Time.deltaTime);
            }
            else
            {
                var desiredPosition = currentPosition + movementInput * speed;
                transform.position = Vector3.MoveTowards(currentPosition, desiredPosition, acceleration);
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(dragOrigin, 0.2f);
            
            Gizmos.color = (dragging) ? Color.red : Color.green;
            Gizmos.DrawLine(dragOrigin, mousePosition);
            
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(mousePosition, 0.2f);
        }
    }
}
