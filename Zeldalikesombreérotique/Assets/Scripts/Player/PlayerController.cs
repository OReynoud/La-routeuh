using NaughtyAttributes;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;
        [Foldout("Références")]public Rigidbody rb;
        [Header("Mouvements")]
        [Space(10)]
        [BoxGroup][Tooltip("Vitesse du joueur")]public float groundSpeed;

        [Foldout("Débug")][Tooltip("Direction du déplacement du joueur")] public Vector3 playerDir;
        [Foldout("Débug")][Tooltip("Est-ce que le joueur touche le sol?")] public bool isGrounded;
    
        private InputManager controls;
        private float _baseOffset = -2.5f;
        void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            instance = this;
            controls = new InputManager();
            controls.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
        }

        // Update is called once per frame
        void Update()
        {
            CameraController.instance.offset = new Vector3(playerDir.x, CameraController.instance.offset.y,playerDir.z + _baseOffset);
            if (!controls.Player.Move.IsPressed() && isGrounded)
            {
                playerDir = new Vector3(playerDir.x * 0.1f,playerDir.y,playerDir.z * 0.1f);
                rb.velocity = playerDir;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                RotateModel();
                rb.velocity = playerDir * groundSpeed;
            }
        }

        private void Move(Vector2 dir)
        {
            playerDir = new Vector3(dir.x,playerDir.y, dir.y);
        }

        private void RotateModel()
        {
            var angle = Mathf.Atan2(playerDir.x, playerDir.z)* Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle,Vector3.up);
            Debug.Log(angle);
        }

        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }
    }
}
