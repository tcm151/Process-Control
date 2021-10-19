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

        [Header("Dragging")]
        public float dragSpeed = 1;

        [Header("WASD")]
        public float maxPanSpeed = 1;
        public float maxAcceleration = 1;
        public float maxDeceleration = 1;

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
            camera.orthographicSize -= ((scrollWheelInput == 0f) ? zoomKeysInput : scrollWheelInput) * zoomSensitivity * Time.deltaTime;
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
            //> USING WASD/ARROW KEYS
            else
            {

                var currentVelocity = rigidbody.velocity;
                Vector2 desiredVelocity = (movementInput) switch
                {
                    { x: 0f, y: 0f } => Vector2.zero,
                    _ => movementInput * (maxPanSpeed * camera.orthographicSize),
                };
                float acceleration = (movementInput) switch
                {
                    { x: 0f, y: 0f } => maxDeceleration,
                    _ => maxAcceleration,
                };

                if (Vector2.Dot(movementInput, currentVelocity) < 0)
                {
                    acceleration = maxAcceleration;
                }

                currentVelocity.MoveTowardsR(desiredVelocity, acceleration * Time.deltaTime);
                rigidbody.velocity = currentVelocity.ClampMagnitude(maxPanSpeed);
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
