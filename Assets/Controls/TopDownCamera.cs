using ProcessControl.Industry;
using ProcessControl.Procedural;
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
        public float zoomKeySensitivity = 5;
        public float scrollWheelSensitivity = 5;

        [Header("Dragging")]
        public float dragSpeed = 1;

        [Header("WASD")]
        public float maxPanSpeed = 1;
        public float maxAcceleration = 1;
        public float maxDeceleration = 1;

        // PRIVATE VARIABLES
        private bool dragging;
        private Camera camera;
        private Vector3 mouseInput;
        private Vector3 movementInput;
        private Vector3 cameraPosition;
        public Transform initialTarget;
        private readonly Vector3 cameraOffset = new Vector3(0, 0, -10);

        //> INITIALIZATION 
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            camera = GetComponent<Camera>();
            camera.transform.position = initialTarget.position + cameraOffset;

            Spawner.onStartLocationDetermined += (coords) =>
            {
                camera.transform.position = coords.ToVector3() + cameraOffset;
                initialTarget.position = coords.ToVector3();
            };
        }

        //> HANDLE INPUT
        private void Update()
        {
            if (Time.timeScale == 0f) return;

            // get middle mouse button
            if (Input.GetKeyDown(KeyCode.Mouse2)) dragging = true;

            var zoomKeysInput = 0f;
            if (Input.GetKey(KeyCode.Period)) zoomKeysInput += 1f;
            if (Input.GetKey(KeyCode.Comma)) zoomKeysInput -= 1f;

            // zoom in and out
            var scrollWheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
            camera.orthographicSize -= ((scrollWheelInput == 0f) ? zoomKeysInput * zoomKeySensitivity : scrollWheelInput * scrollWheelSensitivity) * camera.orthographicSize * Time.deltaTime;
            camera.orthographicSize = camera.orthographicSize.Clamp(minZoom, maxZoom);

            // if release, stop dragging
            if (Input.GetMouseButtonUp(2)) dragging = false;

            // cache camera position for later
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
            //> USING MOUSE DRAGGING
            if (dragging)
            {
                transform.position -= mouseInput * (dragSpeed * camera.orthographicSize);
            }
            else //> USING WASD/ARROW KEYS
            {
                var currentVelocity = rigidbody.velocity;
                
                // calculate velocity based on movement input
                Vector2 desiredVelocity = (movementInput) switch
                {
                    { x: 0f, y: 0f } => Vector2.zero,
                    _ => movementInput * (maxPanSpeed * camera.orthographicSize),
                };
                
                // calculate acceleration based on movement input
                float acceleration = (movementInput) switch
                {
                    { x: 0f, y: 0f } => maxDeceleration,
                    _ => maxAcceleration,
                };

                // if moving away from current velocity use maximum acceleration
                if (Vector2.Dot(movementInput, currentVelocity) < 0)
                {
                    acceleration = maxAcceleration;
                }

                // apply the maths
                currentVelocity.MoveTowardsR(desiredVelocity, acceleration * Time.deltaTime);
                rigidbody.velocity = currentVelocity.ClampMagnitudeR(maxPanSpeed);
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
