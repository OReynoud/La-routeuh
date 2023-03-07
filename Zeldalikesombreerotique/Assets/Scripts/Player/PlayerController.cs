using System;
using System.Dynamic;
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
        [BoxGroup][Tooltip("Accélération du joueur")]public float groundSpeed;
        [BoxGroup][Tooltip("Accélération du joueur quand il manipule un objet")]public float grabbedSpeed;

        [BoxGroup] [Tooltip("Vitesse minimale du joueur")] public float minSpeed;
        [BoxGroup] [Tooltip("Vitesse maximale du joueur")] public float maxSpeed;
        [Range(0,1)][BoxGroup] [Tooltip("Maniabilitée du perso: ( 0 c'est un robot et a 1 il a des briques de savon a la place des pieds)")] public float allowedDrift;

        [Foldout("Débug")][Tooltip("Direction du déplacement du joueur")] public Vector3 playerDir;
        [Foldout("Débug")][Tooltip("Est-ce que le joueur touche le sol?")] public bool isGrounded;
        [Foldout("Débug")][Tooltip("Est-ce que le jeu fait des trucs de gros shlag pour la PoC?")] public bool proofOfConcept;
        [Foldout("Débug")][Tooltip("Est-ce que je joueur manipule un objet?")] public bool isGrabing;
        [Foldout("Débug")][Tooltip("Y a t-il un objet à porté que le joueur peut grab?")] public bool canGrab;
        [Foldout("Débug")][Tooltip("Quel est l'objet à grab")] public Rigidbody objectToGrab;
        public enum ObjectType
        {
            canCarry,
            canMove,
            canRotateOnly
        }
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
        void FixedUpdate()
        {
            // CameraController.instance.offset = new Vector3(playerDir.x, CameraController.instance.offset.y,playerDir.z + _baseOffset);
            
            if (!controls.Player.Move.IsPressed() && isGrounded)
            {

                playerDir = new Vector3(playerDir.x * 0.1f,playerDir.y,playerDir.z * 0.1f);
                rb.velocity *= 0.9f;
                rb.angularVelocity = Vector3.zero;
            }
            else if(!isGrabing)
            {
                ApplyForce(groundSpeed);
            }
            else if (isGrabing)
            {
                ApplyForce(grabbedSpeed);
            }
        }

        void ApplyForce(float appliedModifier)
        {
            RotateModel();
            var dx = playerDir - rb.velocity.normalized;
            if (Mathf.Abs(dx.x) > allowedDrift)
            {
                rb.velocity = new Vector3(rb.velocity.x * 0.9f, rb.velocity.y, rb.velocity.z);
            }

            if (Mathf.Abs(dx.z) > allowedDrift)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z * 0.9f);
            }

            if (rb.velocity.magnitude < minSpeed)
            {
                rb.velocity = minSpeed * playerDir;
            }
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = maxSpeed * playerDir;
                return;
            }
            rb.AddForce(playerDir * (appliedModifier),ForceMode.Force);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<DynamicObject>())
            {
                canGrab = true;
                objectToGrab = other.attachedRigidbody;
            }

            switch (other.GetComponent<DynamicObject>().currentType)
            {
                case ObjectType.canCarry:
                    break;
                case ObjectType.canMove:
                    break;
                case ObjectType.canRotateOnly:
                    break;
            }
            
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Moveable") && !isGrabing)
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
                RotateModel();
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
            //joint.connectedAnchor = new Vector3(dirNormed.x, 0, dirNormed.z);
            //joint.anchor = transform.position;
            var angle2 = Mathf.Atan2(dirNormed.x, dirNormed.z) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.AngleAxis(angle2,Vector3.up);
            transform.rotation = Quaternion.AngleAxis(angle2,Vector3.up);
        }

        /*private void ResetVelocity()
        {
            if (rb.velocity.normalized - playerDir >= Vector3.one * 0.3f)
            {
                
            }
        }*/

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
