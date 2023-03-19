using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;
using Utilities;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;
        [Foldout("Références")]public Rigidbody rb;
        [Foldout("Références")]public SpringJoint joint;
        [Foldout("Références")] public Animator rig;
        [HorizontalLine(color: EColor.Black)]
        
        [BoxGroup("Mouvements")][Tooltip("Accélération du joueur")]public float groundSpeed;
        [BoxGroup("Mouvements")][Tooltip("Accélération du joueur quand il manipule un objet")]public float grabbedSpeed;
        [BoxGroup("Mouvements")] [Tooltip("Vitesse minimale du joueur")] public float minSpeed;
        [BoxGroup("Mouvements")] [Tooltip("Vitesse maximale du joueur")] public float maxSpeed;
        [Range(0,1)][BoxGroup("Mouvements")] [Tooltip("Maniabilitée du perso: ( 0 c'est un robot et a 1 il a des briques de savon a la place des pieds)")] public float allowedDrift;
        [BoxGroup("Mouvements")] [Tooltip("Le temps que le joueur met à ramasser/poser un objet")] public float pickUpTime;
        [HorizontalLine(color: EColor.Red)]
        
        [Foldout("Débug")][Tooltip("Direction du déplacement du joueur")] public Vector3 playerDir;
        [Foldout("Débug")][Tooltip("Est-ce que le joueur touche le sol?")] public bool isGrounded;
        [Foldout("Débug")][Tooltip("Est-ce que le jeu fait des trucs de gros shlag pour la PoC?")] public bool proofOfConcept;
        [Foldout("Débug")][Tooltip("Est-ce que je joueur manipule un objet?")] public bool isGrabbing;
        [Foldout("Débug")][Tooltip("Quel est l'objet à grab")] public Rigidbody objectToGrab;
        [Foldout("Débug")][Tooltip("Le script de l'objet à grab")] public DynamicObject objectType;
        [Foldout("Débug")][Tooltip("Le joueur a t-il le droit de bouger?")] public bool canMove;
        [Foldout("Débug")] [Tooltip("Ou est-ce que le joueur porte son objet?")] private Vector3 carrySpot;
        
        private InputManager controls;
        [Foldout("Autre")] [SerializeField] private float xOffset = 1f;
        [Foldout("Autre")] [SerializeField] private float yOffset = 2f;
        

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            instance = this;
            canMove = true;
            controls = new InputManager();
            controls.Enable();
            controls.Player.Enable();
            controls.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
            controls.Player.Interact.performed += _ => Interact();
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            var speedFactor = rb.velocity.magnitude / maxSpeed;
            if (!proofOfConcept) rig.SetFloat("Speed",speedFactor);
            if (!canMove)
            {
                
                if (!proofOfConcept)rig.SetBool("isWalking", false);
                return;
            }
            if (!controls.Player.Move.IsPressed() && isGrounded)
            {
                
                if (!proofOfConcept)rig.SetBool("isWalking", false);
                playerDir = new Vector3(playerDir.x * 0.1f,playerDir.y,playerDir.z * 0.1f);
                rb.velocity *= 0.9f;
                rb.angularVelocity = Vector3.zero;
            }
            else if (isGrabbing && objectType.mobilityType == DynamicObject.MobilityType.CanMove)
            {
                ApplyForce(grabbedSpeed);
            }
            else
            {
                ApplyForce(groundSpeed);
            }

            /*if (!objectType)
            {
                return;
            }
            if (objectType.currentType == ObjectType.canCarry && isGrabing)
            {
                objectToGrab.position = carrySpot;
            }*/
        }



        /*private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<DynamicObject>() && !isGrabing)
            {
                canGrab = true;
                objectToGrab = other.attachedRigidbody;
                objectType = other.GetComponent<DynamicObject>();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Moveable") && !isGrabing)
            {
                canGrab = false;
                objectToGrab = null;
                objectType = null;
            }
        }*/

        #region Actions
        
        private void Move(Vector2 dir)
        {
            playerDir = new Vector3(dir.x,playerDir.y, dir.y);
        }

        private void Interact()
        {
            if (!isGrabbing)
            {
                objectToGrab = GetClosestObject();
                if (!objectToGrab)
                {
                    return;
                }
                objectType = objectToGrab.GetComponent<DynamicObject>();

                switch (objectType.mobilityType)
                {
                    case DynamicObject.MobilityType.CanCarry:
                        PickupObject();
                        break;
                    case DynamicObject.MobilityType.CanMove:
                        var dir = objectToGrab.position - transform.position;
                        dir.y = 0;
                        objectToGrab.transform.Translate(dir.normalized * 0.2f);
                        joint.gameObject.SetActive(true);
                        joint.connectedBody = objectToGrab;
                        RotateModel();
                        break;
                }
            
                isGrabbing = true;
            }
            else
            {
                switch (objectType.mobilityType)
                {
                    case DynamicObject.MobilityType.CanCarry:
                        PickupObject();
                        break;
                    case DynamicObject.MobilityType.CanMove:
                        joint.connectedBody = null;
                        joint.gameObject.SetActive(false);
                        break;
                }
                
                isGrabbing = false;
            }
        }

        private void ApplyForce(float appliedModifier)
        {
            RotateModel();
            if (!proofOfConcept)rig.SetBool("isWalking", true);
            var dx = playerDir - rb.velocity.normalized;
            if (Mathf.Abs(dx.x) > allowedDrift)
            {
                var velocity = rb.velocity;
                rb.velocity = new Vector3(velocity.x * 0.9f, velocity.y, velocity.z);
            }

            if (Mathf.Abs(dx.z) > allowedDrift)
            {
                var velocity = rb.velocity;
                rb.velocity = new Vector3(velocity.x, velocity.y, velocity.z * 0.9f);
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
        
        private void RotateModel()
        {
            if (!isGrabbing || objectType.mobilityType == DynamicObject.MobilityType.CanCarry)
            {
                var angle = Mathf.Atan2(playerDir.x, playerDir.z)* Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle,Vector3.up);
                return;
            }

            Vector3 dir = objectToGrab.transform.position - transform.position;
            var dirNormed = dir.normalized;
            var angle2 = Mathf.Atan2(dirNormed.x, dirNormed.z) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.AngleAxis(angle2,Vector3.up);
            transform.rotation = Quaternion.AngleAxis(angle2,Vector3.up);
        }

        private void PickupObject()
        {
            if (isGrabbing)
            {
                rb.velocity = Vector3.zero;
                canMove = false;
                var rot = Quaternion.AngleAxis(transform.localRotation.eulerAngles.y, Vector3.up);
                var currentDir = rot * Vector3.forward;
                carrySpot = transform.position + xOffset * 1.2f * currentDir.normalized + Vector3.up;
                objectToGrab.transform.DOJump(carrySpot, 1f, 1, pickUpTime).AppendCallback(() =>
                {
                    canMove = true;
                    objectToGrab.isKinematic = true;
                    objectToGrab.transform.SetParent(null);
                    objectToGrab.useGravity = true;
                    objectToGrab.isKinematic = false;
                });
            }
            else
            {
                var rot = Quaternion.AngleAxis(transform.localRotation.eulerAngles.y, Vector3.up);
                var currentDir = rot * Vector3.forward;
                var transform1 = transform;
                carrySpot = transform1.position + xOffset * currentDir.normalized + yOffset * Vector3.up;
                rb.velocity = Vector3.zero;
                canMove = false;
                objectToGrab.transform.SetParent(transform1);
                objectToGrab.useGravity = false;
                objectToGrab.transform.DOJump(carrySpot, 2.5f, 1, pickUpTime).AppendCallback(() =>
                {
                    canMove = true;
                    objectToGrab.isKinematic = true;
                });
            }
        }
        
        #endregion
        
        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }

        private Rigidbody GetClosestObject()
        {
            // ReSharper disable once Unity.PreferNonAllocApi
            var nearbyObjects = Physics.OverlapSphere(transform.position,2).ToList().Select(x => x.gameObject).ToList();
            
            nearbyObjects.Sort((x, y) =>
            {
                var position = transform.position;
                return Vector3.Distance(x.transform.position, position)
                        .CompareTo(Vector3.Distance(y.transform.position, position));
            });

            return nearbyObjects.Where(obj => obj.GetComponent<DynamicObject>())
                .Select(obj => obj.GetComponent<Rigidbody>()).FirstOrDefault();
        }
    }
}
