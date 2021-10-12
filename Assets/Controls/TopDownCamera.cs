using UnityEngine;
using ProcessControl.Tools;
#pragma warning disable 108,114


namespace ProcessControl.Controls
{
    public class TopDownCamera : MonoBehaviour
    {
        [Header("Zoom")]
        public float minZoom;
        public float maxZoom;
        public float zoomSensitivity = 5;
        
        [Header("Movement")]
        public float panSpeed = 1;
        public float dragSpeed = 1;
        public float acceleration = 1;

        private bool dragging;
        private Camera camera;
        // private Vector3 dragOrigin;
        private Vector3 mouseInput;
        private Vector3 movementInput;
        // private Vector3 mousePosition;
        private Vector3 cameraPosition;
        private readonly Vector3 cameraOffset = new Vector3(0, 0, -10);

        public Transform initialTarget;

        //> INITIALIZATION 
        private void Awake()
        {
            camera = GetComponent<Camera>();
            camera.transform.position = initialTarget.position + cameraOffset;
        }

        //> HANDLE INPUT
        private void Update()
        {
            if (Time.timeScale == 0f) return;
            
            // get middle mouse button
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                dragging = true;
                // dragOrigin = camera.MousePosition2D();
            }

            // zoom in and out
            var scrollWheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
            camera.orthographicSize -= scrollWheelInput * zoomSensitivity;
            camera.orthographicSize = camera.orthographicSize.Clamp(minZoom, maxZoom);
            
            // if release, stop dragging
            if (Input.GetMouseButtonUp(2)) dragging = false;

            // mousePosition = camera.MousePosition2D();
            cameraPosition = transform.position;

            // mouse input
            mouseInput.x = Input.GetAxisRaw("Mouse X");
            mouseInput.y = Input.GetAxisRaw("Mouse Y");
            
            // wasd/arrows input
            movementInput.x = Input.GetAxisRaw("Horizontal");
            movementInput.y = Input.GetAxisRaw("Vertical");
            movementInput = movementInput.normalized * Time.deltaTime;
        }
        
        //> MOVE THE CAMERA
        private void LateUpdate()
        {
            if (dragging)
            {
                transform.position -= mouseInput * (dragSpeed * camera.orthographicSize);
            }
            else
            {
                var desiredPosition = cameraPosition + movementInput * (panSpeed * camera.orthographicSize);
                transform.position = Vector3.MoveTowards(cameraPosition, desiredPosition, acceleration);
            }
        }

        //> DRAW HELPFUL GIZMOS
        private void OnDrawGizmos()
        {
            // if (!Application.isPlaying) return;
            //
            // if (dragging)
            // {
            //     Gizmos.color = Color.red;
            //     Gizmos.DrawSphere(dragOrigin, 0.2f);
            //     Gizmos.DrawLine(dragOrigin, mousePosition);
            //     
            //     Gizmos.color = Color.green;
            //     Gizmos.DrawSphere(mousePosition, 0.2f);
            // }
        }
    }
}
