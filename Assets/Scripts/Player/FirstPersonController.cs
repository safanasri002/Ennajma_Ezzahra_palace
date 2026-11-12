using UnityEngine;

namespace Ennejma
{
    /// <summary>
    /// Visiteur à la première personne : la caméra est posée sur sa tête (hauteur des yeux),
    /// il marche, court, regarde autour de lui avec la souris. Léger balancement de la tête.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Références")]
        [Tooltip("Caméra placée au niveau des yeux (enfant du joueur).")]
        public Transform cameraRoot;

        [Header("Déplacement")]
        public float walkSpeed = 2.2f;
        public float runSpeed = 4.5f;
        public float gravity = -9.81f;

        [Header("Regard (souris)")]
        public float mouseSensitivity = 2f;
        public float minPitch = -80f;
        public float maxPitch = 80f;
        public bool invertY = false;

        [Header("Balancement de la tête")]
        public bool headBob = true;
        public float bobFrequency = 1.6f;
        public float bobAmplitude = 0.035f;

        [Header("État (utilisé par l'UI en phase 2)")]
        public bool canMove = true;
        public bool canLook = true;

        CharacterController controller;
        float pitch;
        float verticalVelocity;
        float bobTimer;
        Vector3 cameraBasePos;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (cameraRoot == null)
            {
                var cam = GetComponentInChildren<Camera>();
                if (cam != null) cameraRoot = cam.transform;
            }
            if (cameraRoot != null) cameraBasePos = cameraRoot.localPosition;
        }

        void Start()
        {
            SpawnPoint.PlacePlayer(this);
            SetCursorLocked(true);
        }

        void Update()
        {
            if (InputBridge.EscapePressed()) SetCursorLocked(false);
            else if (InputBridge.LeftClickPressed() && Cursor.lockState != CursorLockMode.Locked && canLook)
                SetCursorLocked(true);

            if (canLook && Cursor.lockState == CursorLockMode.Locked) Look();
            Move();
        }

        void Look()
        {
            Vector2 d = InputBridge.Look() * mouseSensitivity;
            transform.Rotate(0f, d.x, 0f);
            pitch += invertY ? d.y : -d.y;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            if (cameraRoot != null) cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        void Move()
        {
            Vector2 input = canMove ? InputBridge.Move() : Vector2.zero;
            float speed = InputBridge.Run() ? runSpeed : walkSpeed;
            Vector3 horizontal = (transform.right * input.x + transform.forward * input.y) * speed;

            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;

            controller.Move((horizontal + Vector3.up * verticalVelocity) * Time.deltaTime);
            UpdateHeadBob(horizontal.magnitude);
        }

        void UpdateHeadBob(float horizontalSpeed)
        {
            if (!headBob || cameraRoot == null) return;
            Vector3 target = cameraBasePos;
            if (controller.isGrounded && horizontalSpeed > 0.1f)
            {
                bobTimer += Time.deltaTime * bobFrequency * (horizontalSpeed / walkSpeed);
                target.y += Mathf.Sin(bobTimer * Mathf.PI * 2f) * bobAmplitude;
                target.x += Mathf.Cos(bobTimer * Mathf.PI) * bobAmplitude * 0.5f;
            }
            else bobTimer = 0f;
            cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, target, Time.deltaTime * 12f);
        }

        public void SetCursorLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        /// <summary>Place le visiteur à une position (utilisé par les points d'apparition).</summary>
        public void Teleport(Vector3 position, float yaw)
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(position, Quaternion.Euler(0f, yaw, 0f));
            controller.enabled = true;
            pitch = 0f;
            if (cameraRoot != null) cameraRoot.localRotation = Quaternion.identity;
        }
    }
}
