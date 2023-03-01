using System;
using NaughtyAttributes;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;
        [Foldout("Références")]public Rigidbody rb;
        [Foldout("Références")]public SpringJoint joint;
        [Header("Mouvements")]
        [Space(10)]
        [BoxGroup][Tooltip("Vitesse du joueur")]public float groundSpeed;

        [BoxGroup] public float grabedRotateSpeed;

        [Foldout("Débug")][Tooltip("Direction du déplacement du joueur")] public Vector3 playerDir;
        [Foldout("Débug")][Tooltip("Est-ce que le joueur touche le sol?")] public bool isGrounded;
        [Foldout("Débug")][Tooltip("Est-ce que le jeu fait des trucs de gros shlag pour la PoC?")] public bool proofOfConcept;
        [Foldout("Débug")][Tooltip("Est-ce que je joueur manipule un objet?")] public bool isGrabing;
        [Foldout("Débug")] public bool canGrab;
        [Foldout("Débug")] public Rigidbody objectToGrab;
    
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
            controls.Player.Interact.performed += _ => Interact();
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
            else if(!isGrabing)
            {
                RotateModel();
                rb.velocity = playerDir * groundSpeed;
            }
            else if (isGrabing)
            {
                
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Moveable"))
            {
                canGrab = true;
                objectToGrab = other.attachedRigidbody;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Moveable"))
            {
                canGrab = false;
                objectToGrab = null;
            }
        }

        private void Move(Vector2 dir)
        {
            playerDir = new Vector3(dir.x,playerDir.y, dir.y);
        }

        private void Interact()
        {
            if (isGrabing)
            {
                joint.connectedBody = null;
                joint.gameObject.SetActive(false);
                isGrabing = false;
                return;
            }
            if (canGrab && objectToGrab != null)
            {
                joint.connectedBody = objectToGrab;
                joint.gameObject.SetActive(true);
                isGrabing = true;
            }
        }

        private void RotateModel()
        {
            if (!isGrabing)
            {
                var angle = Mathf.Atan2(playerDir.x, playerDir.z)* Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle,Vector3.up);
                return;
            }

            Vector3 dir = objectToGrab.transform.position - transform.position;
            var dirNormed = dir.normalized;
            joint.connectedAnchor = new Vector3(dirNormed.x, 0, dirNormed.z);
            joint.anchor = transform.position;
            var angle2 = Mathf.Atan2(dirNormed.x, dirNormed.z) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.AngleAxis(angle2,Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.AngleAxis(angle2,Vector3.up),grabedRotateSpeed);
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
