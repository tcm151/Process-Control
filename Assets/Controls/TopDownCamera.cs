using UnityEngine;
using ProcessControl.Tools;
#pragma warning disable 108,114


namespace ProcessControl.Controls
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TopDownCamera : MonoBehaviour
    {
        private Rigidbody2D rigidbody;
        
        [Header("Zoom")]
        public float minZoom;
        public float maxZoom;
        public float zoomSensitivity = 5;
        
        [Header("Movement")]
        public float panSpeed = 1;
        public float dragSpeed = 1;
        public float acceleration = 1;
        public float deceleration = 1;

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
            rigidbody = GetComponent<Rigidbody2D>();
            
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
        private void FixedUpdate()
        {
            if (dragging)
            {
                transform.position -= mouseInput * (dragSpeed * camera.orthographicSize);
            }
            else
            {
                
                var currentVelocity = rigidbody.velocity;
                
                Vector2 desiredVelocity = (movementInput) switch
                {
                    {x: 0f, y: 0f} => Vector2.zero,
                    _ => movementInput * (panSpeed * camera.orthographicSize),
                };

                float maxAcceleration = (movementInput) switch
                {
                    {x: 0f, y: 0f} => deceleration,
                    _ => acceleration,
                };
                
                currentVelocity.MoveTowardsR(desiredVelocity, maxAcceleration * Time.deltaTime);
                
                // transform.position = Vector3.MoveTowards(cameraPosition, desiredPosition, acceleration);
                rigidbody.velocity = currentVelocity;
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
