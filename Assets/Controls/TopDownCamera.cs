using System;
using ProcessControl.Tools;
using UnityEngine;


namespace ProcessControl.Controls
{
    public class TopDownCamera : MonoBehaviour
    {
        public float minZoom;
        public float maxZoom;
        public float panSpeed = 1;
        public float dragSpeed = 1;
        public float acceleration = 1;
        public float scrollSensitivity = 1;

        public bool dragging;

        private Vector3 mousePosition;
        private Vector3 dragOrigin;
        private Vector3 mouseInput;
        private Vector3 cameraOrigin;
        private Vector3 movementInput;
        private Vector3 cameraPosition;
        private readonly Vector3 cameraOffset = new Vector3(0, 0, -10);

        new private Camera camera;

        //> INITIALIZATION 
        private void Awake()
        {
            camera = GetComponent<Camera>();
            camera.transform.position = cameraOffset;
        }

        //> HANDLE INPUT
        private void Update()
        {
            // get middle mouse button
            if (Input.GetMouseButtonDown(2))
            {
                dragging = true;
                dragOrigin = camera.MouseWorldPosition2D();
                cameraOrigin = transform.position;
            }

            // zoom in and out
            var scrollWheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
            camera.orthographicSize -= scrollWheelInput * scrollSensitivity;
            camera.orthographicSize = camera.orthographicSize.Clamp(minZoom, maxZoom);
            
            // if release, stop dragging
            if (Input.GetMouseButtonUp(2)) dragging = false;

            mousePosition = camera.MouseWorldPosition2D();
            cameraPosition = transform.position;

            mouseInput.x = Input.GetAxisRaw("Mouse X");
            mouseInput.y = Input.GetAxisRaw("Mouse Y");
            
            movementInput.x = Input.GetAxisRaw("Horizontal");
            movementInput.y = Input.GetAxisRaw("Vertical");
            movementInput = movementInput.normalized * Time.deltaTime;
        }
        
        //> MOVE THE CAMERA
        private void LateUpdate()
        {
            if (dragging)
            {
                var dragOffset = mousePosition - dragOrigin;
                transform.position -= mouseInput * (dragSpeed * camera.orthographicSize);
            }
            else
            {
                var desiredPosition = cameraPosition + movementInput * panSpeed;
                transform.position = Vector3.MoveTowards(cameraPosition, desiredPosition, acceleration);
            }
        }

        //> DRAW HELPFUL GIZMOS
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            if (dragging)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(dragOrigin, 0.2f);
                Gizmos.DrawLine(dragOrigin, mousePosition);
                
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(mousePosition, 0.2f);
            }
        }
    }
}
