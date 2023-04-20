using System.Linq;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;
using Utilities;
using Vector3 = UnityEngine.Vector3;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;
        [Foldout("Références")]public Rigidbody rb;
        [Foldout("Références")]public FixedJoint joint;
        [Foldout("Références")] public Animator rig;
        [Foldout("Références")] public Collider playerColl;
        [HorizontalLine(color: EColor.Black)]
        
        [BoxGroup("Mouvements")][Tooltip("Accélération du joueur")]public float groundSpeed;
        [BoxGroup("Mouvements")] [Tooltip("Multiplie la groundSpeed")]public float sprintSpeed;
        [BoxGroup("Mouvements")][Tooltip("Accélération du joueur quand il manipule un objet")]public float grabbedSpeed;
        [BoxGroup("Mouvements")] [Tooltip("Vitesse minimale du joueur")] public float minSpeed;
        [BoxGroup("Mouvements")] [Tooltip("Vitesse maximale du joueur")] public float maxSpeed;
        [BoxGroup("Mouvements")] [Tooltip("Vitesse minimale du joueur quand il a grab un objet")]
        public float grabbedMinFactor;
        [Range(0,1)][BoxGroup("Mouvements")] [Tooltip("Maniabilitée du perso: ( 0 c'est un robot et a 1 il a des briques de savon a la place des pieds)")] public float allowedDrift;
        [BoxGroup("Mouvements")] [Tooltip("Le temps que le joueur met à ramasser/poser un objet")] public float pickUpTime;
        [HorizontalLine(color: EColor.Red)]
        
        [Foldout("Débug")][Tooltip("Direction du déplacement du joueur")] public Vector3 playerDir;
        [Foldout("Débug")][Tooltip("Est-ce que le joueur touche le sol?")] public bool isGrounded;
        [Foldout("Débug")][Tooltip("Est-ce que le jeu fait des trucs de gros shlag pour la PoC?")] public bool proofOfConcept;
        [Foldout("Débug")][Tooltip("Est-ce que je joueur manipule un objet?")] public bool isGrabbing;
        [Foldout("Débug")] [Tooltip("Pousser tirer quand c'est faux et rotate quand c'est vrai")] public bool pushingPulling_Rotate;
        [Foldout("Débug")][Tooltip("Quel est l'objet à grab")] public Rigidbody objectToGrab;
        [Foldout("Débug")][Tooltip("Le script de l'objet à grab")] public DynamicObject objectType;
        [Foldout("Débug")][Tooltip("Le joueur a t-il le droit de bouger?")] public bool canMove;
        [Foldout("Débug")] [Tooltip("Ou est-ce que le joueur porte son objet?")] private Vector3 carrySpot;
        
        [Foldout("Débug")] [Tooltip("Double la vitesse max du joueur")]public bool isSprinting;
        [Foldout("Débug")] [Tooltip("")]public bool willTriggerCinematic;
        [Foldout("Débug")] [Tooltip("")]public Transform tpLocation;
        
        [Foldout("Débug")] [Tooltip("")]public bool isProtected;
        
        public InputManager controls;
        [Foldout("Autre")] [SerializeField] private float xOffset = 1f;
        [Foldout("Autre")] [SerializeField] private float yOffset = 2f;
        private float inputLag = 0.2f;

        private RigidbodyConstraints _baseConstraints =
            RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        
        public CinemachineStateDrivenCamera cinemachineCamera;

        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.forward);
        }

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Physics.reuseCollisionCallbacks = true;
            instance = this;
            canMove = true;
            controls = new InputManager();
            controls.Enable();
            controls.Player.Enable();
            controls.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
            controls.Player.Interact.performed += _ => Interact();
            controls.Player.Sprint.performed += _ => TogleSprint();
            controls.Player.SecondaryInput.performed += _ => SecondaryInteract();
            cinemachineCamera.Follow = transform;
        }
        
        // Update is called once per frame
        private void FixedUpdate()
        {
            if (inputLag > 0)
            {
                inputLag -= Time.deltaTime;
            }
            var speedFactor = rb.velocity.magnitude / maxSpeed / 1.3f;
            if (!proofOfConcept) rig.SetFloat("Speed",speedFactor);
            if (!canMove)
            {
                if (!proofOfConcept)rig.SetBool("isWalking", false);
                return;
            }
            if (!controls.Player.Move.IsPressed() && isGrounded && !pushingPulling_Rotate)
            {
                
                if (!proofOfConcept)rig.SetBool("isWalking", false);
                playerDir = new Vector3(playerDir.x * 0.1f,playerDir.y,playerDir.z * 0.1f);
                rb.velocity *= 0.9f;
                rb.angularVelocity = Vector3.zero;
                return;
            }
            if (isGrabbing && objectType.mobilityType == DynamicObject.MobilityType.CanMove)
            {
                ApplyForce(grabbedSpeed);
            }
            else
            {
                ApplyForce(groundSpeed);
            }
        }

        #region Actions
        private void TogleSprint() 
        {
            isSprinting = !isSprinting;
        }
        private void Move(Vector2 dir)
        {
            playerDir = new Vector3(dir.x,playerDir.y, dir.y);
        }

        private void Interact()
        {
            if (inputLag > 0) return;
            inputLag = 0.1f;
            if (willTriggerCinematic)
            {
                controls.Disable();
                objectToGrab = GetClosestObject();
                var dir = tpLocation.position - objectToGrab.position;
                var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                var rotationValue = Quaternion.AngleAxis(angle, Vector3.up);
                cinemachineCamera.Follow = objectToGrab.transform;
                objectToGrab.transform.DOLocalRotate(rotationValue.eulerAngles, 1).OnComplete(()=>
                {
                    objectToGrab.transform.DOJump(tpLocation.position, 2, 1, 3).AppendCallback((() =>
                    {
                        controls.Enable();
                        cinemachineCamera.Follow = transform;
                        willTriggerCinematic = false;
                        objectToGrab.GetComponent<RollbackCar>().flyToHub = false;
                    }));
                }); 
                return;
            }
            if (!isGrabbing)
            {
                objectToGrab = GetClosestObject();
                if (!objectToGrab)
                {
                    return;
                }
                objectType = objectToGrab.GetComponent<DynamicObject>();
                if (objectToGrab.GetComponent<ObjectReseter>())
                {
                    objectToGrab.GetComponent<ObjectReseter>().ResetObjects();
                    return;
                }
                controls.Disable();
                switch (objectType.mobilityType)
                {
                    case DynamicObject.MobilityType.None:
                        controls.Enable();
                        return;
                    case DynamicObject.MobilityType.CanCarry:
                        PickupObject();
                        break;
                    case DynamicObject.MobilityType.CanMove:
                        var dir = objectToGrab.position - transform.position;
                        dir.y = 0;
                        rb.velocity = Vector3.zero;
                        objectToGrab.transform.DOMove(objectToGrab.transform.position + dir.normalized * 0.2f,0.2f).OnComplete(
                            () =>
                            {
                                RotateModel();
                                joint.gameObject.SetActive(true);
                                joint.connectedBody = objectToGrab;
                                objectToGrab.isKinematic = false;
                            });
                        break;
                    case DynamicObject.MobilityType.MoveWithHandle:
                        var checkForProximity = Physics.OverlapCapsule(objectType.handlePos.position + Vector3.down,
                            objectType.handlePos.position + Vector3.up, 1);
                        bool isPlayerNear = false;
                        foreach (var coll in checkForProximity)
                        {
                            if(coll != playerColl)continue;
                            isPlayerNear = true;
                            transform.DOMove(new Vector3(objectType.handlePos.position.x,transform.position.y,objectType.handlePos.position.z), 0.5f).OnComplete((() =>
                            {
                                RotateModel();
                                joint.gameObject.SetActive(true);
                                joint.connectedBody = objectToGrab;
                                objectToGrab.isKinematic = false;
                            }));
                            break;
                        }

                        if (!isPlayerNear)
                        {
                            Debug.Log("Player is too far from handle");
                            return;
                        }
                        break;
                }
                controls.Enable();
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
                        objectToGrab.isKinematic = true;
                        break;
                    case DynamicObject.MobilityType.MoveWithHandle:
                        joint.connectedBody = null;
                        joint.gameObject.SetActive(false);
                        objectToGrab.isKinematic = true;
                        break;
                }

                pushingPulling_Rotate = false;
                isGrabbing = false;
            }
        }

        void SecondaryInteract()
        {
            if (isGrabbing)
            {
                pushingPulling_Rotate = !pushingPulling_Rotate;
            }

            objectToGrab.constraints = _baseConstraints | RigidbodyConstraints.FreezePosition;
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
                        
            if (rb.velocity.magnitude < minSpeed && !isGrabbing)
            {
                rb.velocity = minSpeed * playerDir;
            }

            if (!isSprinting)
            {
                if (rb.velocity.magnitude > maxSpeed)
                {
                    rb.velocity = maxSpeed * playerDir;
                    return;
                }
            }
            else
            {
                if (rb.velocity.magnitude > maxSpeed * 2)
                {
                    rb.velocity = playerDir * (maxSpeed * 2);
                    return;
                }
            }
            
                // Pousser/tirrer
            if (isGrabbing && !pushingPulling_Rotate)
            {
                var fwrd = transform.forward;
                var differential = playerDir - fwrd;
                var absDiff = Mathf.Abs(differential.x) + Mathf.Abs(differential.z);
                var ctxMax = maxSpeed / objectToGrab.mass;
                if (absDiff < 2f)
                {
                    playerDir = new Vector3(fwrd.x, playerDir.y, fwrd.z);
                    if (rb.velocity.magnitude < minSpeed * grabbedMinFactor)
                    {
                        rb.velocity = minSpeed * playerDir;
                    }
                    if (rb.velocity.magnitude > ctxMax)
                    {
                        rb.velocity = ctxMax * playerDir;
                        return;
                    }
                    rb.AddForce(playerDir * appliedModifier);
                }
                else if(absDiff > 2f)
                { 
                    playerDir = new Vector3(-fwrd.x, playerDir.y, -fwrd.z);
                    if (rb.velocity.magnitude < minSpeed * grabbedMinFactor)
                    {
                        rb.velocity = minSpeed * playerDir;
                    }
                    if (rb.velocity.magnitude > ctxMax)
                    {
                        rb.velocity = (ctxMax) * playerDir;
                        return;
                    }
                    rb.AddForce(playerDir * appliedModifier);
                }
                return;
            }
            if (!isSprinting)
            {
                rb.AddForce(playerDir * (appliedModifier),ForceMode.Force);
                return;
            }
            
            rb.AddForce(playerDir * ((appliedModifier) * sprintSpeed),ForceMode.Force);


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
            transform.rotation = Quaternion.AngleAxis(angle2,Vector3.up);
        }
        
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

            return nearbyObjects.Where(obj =>
                    {
                        if (!obj.TryGetComponent<DynamicObject>(out var dynObj)) return false;
                        return dynObj.mobilityType != DynamicObject.MobilityType.None;
                    })
                .Select(obj => obj.GetComponent<Rigidbody>()).FirstOrDefault();
        }
    }
}
