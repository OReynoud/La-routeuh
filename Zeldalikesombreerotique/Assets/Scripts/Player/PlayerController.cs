using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Utilities;
using Vector3 = UnityEngine.Vector3;
// ReSharper disable Unity.InefficientPropertyAccess
// ReSharper disable Unity.PreferAddressByIdToGraphicsParams
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;
        [Foldout("Références")]public Rigidbody rb;
        [Foldout("Références")]public ConfigurableJoint joint;
        [Foldout("Références")] public Animator rig;
        [Foldout("Références")] public Collider playerColl;
        [HorizontalLine(color: EColor.Black)]
        
        [BoxGroup("Mouvements")][Tooltip("Accélération du joueur")]public float groundSpeed;
        [BoxGroup("Mouvements")] [Tooltip("Multiplie la groundSpeed")]public float sprintSpeed;
        [BoxGroup("Mouvements")][Tooltip("Accélération du joueur quand il manipule un objet")]public float grabbedSpeed;
        [BoxGroup("Mouvements")] [Tooltip("Vitesse minimale du joueur")] public float minSpeed;
        [BoxGroup("Mouvements")] [Tooltip("Vitesse maximale du joueur")] public float maxSpeed;
        
        [BoxGroup("Mouvements")] [Tooltip("Vitesse de rotation")] public float rotationSpeed;

        [BoxGroup("Mouvements")] [Tooltip("Vitesse minimale du joueur quand il a grab un objet")]
        public float grabbedMinFactor;
        [Range(0,1)][BoxGroup("Mouvements")] [Tooltip("Maniabilitée du perso: ( 0 c'est un robot et a 1 il a des briques de savon a la place des pieds)")] public float allowedDrift;
        [BoxGroup("Mouvements")] [Tooltip("Le temps que le joueur met à ramasser/poser un objet")] public float pickUpTime;
        [HorizontalLine(color: EColor.Red)]
        
        [Foldout("Débug")][Tooltip("Direction du déplacement du joueur")] public Vector3 playerDir;
        [Foldout("Débug")][Tooltip("Est-ce que le joueur touche le sol?")] public bool isGrounded;
        [Foldout("Débug")][Tooltip("Est-ce que le jeu fait des trucs de gros shlag pour la PoC?")] public bool proofOfConcept;
        [Foldout("Débug")][Tooltip("Est-ce que je joueur manipule un objet?")] public bool isGrabbing;
        [Foldout("Débug")] [Tooltip("Pousser tirer quand c'est faux et rotate quand c'est vrai")] public bool pushingPullingRotate;
        [Foldout("Débug")][Tooltip("Quel est l'objet à grab")] public Rigidbody objectToGrab;
        [Foldout("Débug")][Tooltip("Le script de l'objet à grab")] public DynamicObject objectType;
        [Foldout("Débug")][Tooltip("Le joueur a t-il le droit de bouger?")] public bool canMove;
        [Foldout("Débug")] [Tooltip("Ou est-ce que le joueur porte son objet?")] private Vector3 carrySpot;
        
        [Foldout("Débug")] [Tooltip("Double la vitesse max du joueur")]public bool isSprinting;
        
        [Foldout("Débug")] [Tooltip("")]public bool isProtected;
        
        public InputManager controls;
        [Foldout("Autre")] [SerializeField] private float xOffset = 1f;
        [Foldout("Autre")] [SerializeField] private float yOffset = 2f;
        [Foldout("Autre")] public float rumbleIntensity;
        private float inputLag = 0.2f;

        private const RigidbodyConstraints BaseConstraints = RigidbodyConstraints.FreezeRotation;
        public Gamepad gamepad;
        public bool canRotateClockwise = true;
        public bool canRotateCounterClockwise = true;
        [Foldout("Autre")] public LayerMask mask;


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
            gamepad = Gamepad.current;
            
        }
        
        // Update is called once per frame
        private void FixedUpdate()
        {
            //incrémentation du inputBuffer pour éviter le spam de bouttons
            if (inputLag > 0)
            {
                inputLag -= Time.deltaTime;
            }
            var speedFactor = rb.velocity.magnitude / maxSpeed;
            if (!proofOfConcept) rig.SetFloat("Speed",speedFactor);
            if (!canMove)
            {
                if (!proofOfConcept)rig.SetBool("isWalking", false);
                return;
            }
            if (!controls.Player.Move.IsPressed() && isGrounded)    //Si ya aucune input du joueur
            {
                if (!proofOfConcept)rig.SetBool("isWalking", false);
                rig.SetBool("IsPushing",false);
                playerDir = new Vector3(playerDir.x * 0.1f,playerDir.y,playerDir.z * 0.1f);
                rb.velocity *= 0.9f;
                rb.angularVelocity = Vector3.zero;
                gamepad?.SetMotorSpeeds(0f,0f);
                return;
            }
            if (isGrabbing && objectType.mobilityType is DynamicObject.MobilityType.CanMove or DynamicObject.MobilityType.MoveWithHandle)    //Si le joueur bouge en grabbant un truc
            {
                
                gamepad?.SetMotorSpeeds(0f,0f);
                ApplyForce(grabbedSpeed);
            }
            else
            {
                ApplyForce(groundSpeed);    //Si le joueur bouge
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
                canMove = false;
                switch (objectType.mobilityType)
                {
                    case DynamicObject.MobilityType.None:
                        controls.Enable();
                        return;
                    case DynamicObject.MobilityType.CanCarry:
                        PickupObject();
                        break;
                    case DynamicObject.MobilityType.CanMove:
                        rb.velocity = Vector3.zero;
                        var dir = objectToGrab.position - transform.position;
                        dir.y = 0;
                        if (Vector3.Distance(
                                objectToGrab.GetComponent<BoxCollider>().ClosestPoint(transform.position), 
                                transform.position) > 1f)       dir *= -1;
                        
                            objectToGrab.transform.DOMove(objectToGrab.transform.position + dir.normalized * 0.3f,0.1f).
                                OnComplete(() => SetJoint(true));
                        break;
                    case DynamicObject.MobilityType.MoveWithHandle:
                        var checkForProximity = Physics.OverlapCapsule(
                            objectType.handlePos.position + Vector3.down,
                            objectType.handlePos.position + Vector3.up, 
                            1);
                        
                        bool isPlayerNear = false;
                        foreach (var coll in checkForProximity)
                        {
                            if(coll != playerColl)continue;
                            isPlayerNear = true;
                            transform.DOMove(new Vector3(objectType.handlePos.position.x,transform.position.y,objectType.handlePos.position.z), 0.5f).
                                OnComplete(() => SetJoint(true));
                        }

                        if (!isPlayerNear)
                        {
                            Debug.Log("Player is too far from handle");
                            controls.Enable();
                            canMove = true;
                            return;
                        }
                        break;
                }

                rig.SetBool("IsGrabbing",true);
                transform.DOMove(transform.position, 0.3f).OnComplete((() =>
                {
                    canMove = true;
                    controls.Enable();
                    isGrabbing = true;
                    RotateModel();
                }));
            }
            else
            {
                switch (objectType.mobilityType)
                {
                    case DynamicObject.MobilityType.CanCarry:
                        PickupObject();
                        break;
                    case DynamicObject.MobilityType.CanMove:
                        SetJoint(false);
                        break;
                    case DynamicObject.MobilityType.MoveWithHandle:
                        SetJoint(false);
                        break;
                }

                rig.SetBool("IsGrabbing",false);
                pushingPullingRotate = false;
                isGrabbing = false;
            }
        }
        
        /// <summary>
        /// Switch entre le rotate et le pousser tirer
        /// </summary>
        void SecondaryInteract()
        {
            if (isGrabbing)
            {
                pushingPullingRotate = !pushingPullingRotate;
            }

            if (pushingPullingRotate)
            {
                transform.position = new Vector3(
                    objectType.handlePos.position.x, 
                    transform.position.y,
                    objectType.handlePos.position.z);
                joint.autoConfigureConnectedAnchor = false;
                if (!objectType.isColliding)
                {
                    objectToGrab.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePosition;
                }
                //rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                joint.autoConfigureConnectedAnchor = true;
                objectToGrab.constraints = BaseConstraints;
                //rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="appliedModifier"> modifcateur de force</param>
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
            
                //Pousser/tirrer_______________________________________________________________________________________________________________________________________
            
            if (isGrabbing && !pushingPullingRotate)
            {
                var fwrd = transform.forward;
                var differential = playerDir - fwrd;
                var absDiff = Mathf.Abs(differential.x) + Mathf.Abs(differential.z);
                var ctxMax = maxSpeed / objectToGrab.mass;
                //Debug.Log(absDiff);
                if (absDiff < 2f)
                {
                    playerDir = new Vector3(fwrd.x, playerDir.y, fwrd.z);
                    var delta = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                        new Vector2(objectType.handlePos.position.x, objectType.handlePos.position.z));
                    if (delta > 0.1f)
                    {
                        gamepad?.SetMotorSpeeds(rumbleIntensity,rumbleIntensity);
                    }
                    if (rb.velocity.magnitude < minSpeed * grabbedMinFactor)
                    {
                        rb.velocity = minSpeed * playerDir;
                    }
                    if (rb.velocity.magnitude > ctxMax)
                    {
                        rb.velocity = ctxMax * playerDir;
                        return;
                    }
                    rig.SetBool("IsPushing",true);
                    
                    rb.AddForce(playerDir * appliedModifier);
                }
                else if(absDiff >= 2f)
                { 
                    
                    playerDir = new Vector3(-fwrd.x, playerDir.y, -fwrd.z);
                    var delta = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                        new Vector2(objectType.handlePos.position.x, objectType.handlePos.position.z));
                    if (delta > 0.3f)
                    {
                        rb.velocity = Vector3.zero;
                        gamepad?.SetMotorSpeeds(rumbleIntensity,rumbleIntensity);
                        //rb.AddForce(playerDir * (-2 * appliedModifier));
                        return;
                    }
                    if (rb.velocity.magnitude < minSpeed * grabbedMinFactor)
                    {
                        rb.velocity = minSpeed * playerDir;
                    }
                    if (rb.velocity.magnitude > ctxMax)
                    {
                        rb.velocity = (ctxMax) * playerDir;
                        return;
                    }
                    rig.SetBool("IsPushing",false);
                    rb.AddForce(playerDir * appliedModifier);
                }
                return;
            }
            
                //Rotate______________________________________________________________________________________________________________________________________
            
            if (isGrabbing && pushingPullingRotate && playerDir.magnitude > 0.1f)
            {
                rb.velocity = Vector3.zero;
                var avatarOrientation = -transform.forward;
                var diff = playerDir - avatarOrientation;
                var absDiff = Mathf.Abs(diff.x) + Mathf.Abs(diff.z);

                var dirModifier = 1f;
                // 1 is clockwise
                // -1 is counter-clockwise
                
                
                if (Vector3.Distance(transform.right,playerDir) < Vector3.Distance(-transform.right,playerDir))
                {
                    if (canRotateCounterClockwise)
                    {
                        dirModifier = -1;
                    }
                    else
                    {
                        dirModifier = 0;
                    }
                }
                else
                {
                    if (canRotateClockwise)
                    {
                        dirModifier = 1;
                    }
                    else
                    {
                        dirModifier = 0;
                    }
                }
                if (absDiff > 1f)
                {
                    transform.RotateAround(objectToGrab.position,transform.up, rotationSpeed * dirModifier);
                }
                else
                {
                    transform.RotateAround(objectToGrab.position,transform.up, rotationSpeed * absDiff * dirModifier);
                }
                
                var delta = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                    new Vector2(objectType.handlePos.position.x, objectType.handlePos.position.z));
                if (delta > 0.1f)
                {
                    rb.velocity =  Vector3.zero;
                }
                return;
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
                controls.Disable();
                rb.velocity = Vector3.zero;
                canMove = false;
                var rot = Quaternion.AngleAxis(transform.localRotation.eulerAngles.y, Vector3.up);
                var currentDir = rot * Vector3.forward;
                carrySpot = transform.position + xOffset * 1.2f * currentDir.normalized;
                objectToGrab.transform.DOJump(carrySpot, 0.5f, 1, pickUpTime).AppendCallback(() =>
                {
                    canMove = true;
                    objectToGrab.isKinematic = true;
                    objectToGrab.transform.SetParent(null);
                    objectToGrab.useGravity = true;
                    controls.Enable();
                });
            }
            else
            {
                controls.Disable();
                var rot = Quaternion.AngleAxis(transform.localRotation.eulerAngles.y, Vector3.up);
                var currentDir = rot * Vector3.forward;
                var transform1 = transform;
                carrySpot = transform1.position + xOffset * currentDir.normalized + yOffset * Vector3.up;
                rb.velocity = Vector3.zero;
                canMove = false;
                objectToGrab.transform.SetParent(transform1);
                objectToGrab.useGravity = false;
                objectToGrab.transform.DOJump(carrySpot, 1f, 1, pickUpTime).AppendCallback(() =>
                {
                    canMove = true;
                    objectToGrab.isKinematic = true;
                    controls.Enable();
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
        /// <summary>
        /// Méthode pour trouver le Dynamic Object le plus proche du joueur
        /// </summary>
        /// <returns></returns>
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
        private void SetJoint(bool grabbing)
        {
            if (grabbing)
            {
                joint.gameObject.SetActive(true);
                joint.connectedBody = objectToGrab;
                joint.connectedAnchor = objectToGrab.transform.position;
                objectToGrab.isKinematic = false;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                objectToGrab.constraints = BaseConstraints;
                joint.connectedBody = null;
                joint.gameObject.SetActive(false);
                objectToGrab.isKinematic = true;
                joint.autoConfigureConnectedAnchor = true;
                objectToGrab = null;
            }
        }

        private void OnCollisionStay(Collision collisionInfo)
        {
            if (!objectToGrab)return;
            if (collisionInfo.gameObject.CompareTag("Untagged")) return;
            if (collisionInfo.gameObject == objectToGrab.gameObject && !pushingPullingRotate) return;
            var leftSide = -transform.right;
            var rightSide = transform.right;
            var delta = Vector3.Distance(leftSide, collisionInfo.collider.ClosestPoint(leftSide))- Vector3.Distance(rightSide, collisionInfo.collider.ClosestPoint(rightSide));
            if (delta > 0)
            {
                //right side
                canRotateCounterClockwise = false;
            }
            else
            {
                //left side
                canRotateClockwise = false;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (!objectToGrab) return;
            if (other.gameObject == objectToGrab.gameObject && !pushingPullingRotate) return;
            canRotateClockwise = true;
            canRotateCounterClockwise = true;
        }
    }
}

/*if (willTriggerCinematic)   //Cinématique pour la voiture
            {
                controls.Disable();
                objectToGrab = GetClosestObject();
                var dir = tpLocation.position - objectToGrab.position;
                var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                var rotationValue = Quaternion.AngleAxis(angle, Vector3.up);
                objectToGrab.transform.DOLocalRotate(rotationValue.eulerAngles, 1).OnComplete(()=>
                {
                    objectToGrab.transform.DOJump(tpLocation.position, 2, 1, 3).AppendCallback((() =>
                    {
                        controls.Enable();
                        willTriggerCinematic = false;
                        objectToGrab.GetComponent<RollbackCar>().flyToHub = false;
                    }));
                }); 
                return;
            }*/