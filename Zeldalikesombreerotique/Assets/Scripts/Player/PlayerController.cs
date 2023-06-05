using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Managers;
using Utilities;
using Random = UnityEngine.Random;
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
        [Foldout("Références")] public Animator[] rig;
        [Foldout("Références")] public Collider playerColl;

        [HorizontalLine(color: EColor.Black)] [BoxGroup("Mouvements")] [Range(0,1)] public float modelRotate;
        [BoxGroup("Mouvements")][Tooltip("Accélération du joueur")]public float groundSpeed;
        [BoxGroup("Mouvements")] [Tooltip("Multiplie la groundSpeed")]public float sprintSpeed;
        [BoxGroup("Mouvements")][Tooltip("Accélération du joueur quand il manipule un objet")]public float grabbedSpeed;
        [BoxGroup("Mouvements")] [Tooltip("Vitesse minimale du joueur")] public float minSpeed;
        [BoxGroup("Mouvements")] [Tooltip("Vitesse maximale du joueur")] public float maxSpeed;
        [BoxGroup("Mouvements")]public float savedMaxSpeed;
        
        [BoxGroup("Mouvements")] [Tooltip("Vitesse de rotation quand le joueur grab")] public float rotationSpeed;

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
        [Foldout("Variables pour Enzo")][Tooltip("La taille de la marge de sécurité pour la collision des spots")]public float overlapBoxSize = 2;

        private float inputLag = 0.2f;

        private const RigidbodyConstraints BaseConstraints = RigidbodyConstraints.FreezeRotation;
        public Gamepad gamepad;
        [Foldout("Débug")]public bool canRotateClockwise = true;
        [Foldout("Débug")]public bool canRotateCounterClockwise = true;
        [Foldout("Débug")]public bool canPush = true;
        [Foldout("Débug")]public bool canPull = true;
        [Foldout("Variables pour Enzo")] [Tooltip("Le temps que le perso mets pour ajuster sa position en grabbant un objet")]public float grabAdjustTimer;
        [Foldout("Variables pour Enzo")] [Tooltip("Plus c'est bas, plus la décélerration est forte")][Range(0,1)]public float decelerationFactor;
        [Foldout("Variables pour Enzo")] [Tooltip("Le temps maximal avant avant que le joueur pousse/tire à pleine vitesse")]public float grabAccelerationTime;
        [Foldout("Variables pour Enzo")][ShowNonSerializedField]private float accelerationTimer;
        private float grabTimer = 0;
        private bool oui;
        [Foldout("Autre")] public LayerMask mask;
        [HorizontalLine(2,EColor.Blue)]
        [BoxGroup("Anim d'intro")]public bool introCinematic;
        [BoxGroup("Anim d'intro")] [ShowIf("introCinematic")] public Transform pointToMove;
        [BoxGroup("Anim d'intro")] [ShowIf("introCinematic")] public float timeToStandUp;
        [BoxGroup("Anim d'intro")] [ShowIf("introCinematic")] public float timeToPickUpCone;
        [BoxGroup("Anim d'intro")] [ShowIf("introCinematic")] public float timeToPutOnCone;
        [BoxGroup("Anim d'intro")] [ShowIf("introCinematic")] public Transform characterHand;
        [BoxGroup("Anim d'intro")] [ShowIf("introCinematic")] public Transform characterHead;
        [BoxGroup("Anim d'intro")] [ShowIf("introCinematic")] public Transform cone;
        [BoxGroup("Anim d'intro")] [ShowIf("introCinematic")] public Vector3 finalConePosition;
        [BoxGroup("Anim d'intro")] [ShowIf("introCinematic")] public Vector3 finalConeRotation;
        [BoxGroup("Anim d'intro")] [ShowIf("introCinematic")] public float conePutOnDuration;
        
        [BoxGroup("Anim d'intro")] [ShowIf("introCinematic")] public float walkAnimationSpeed;
        [BoxGroup("Anim d'intro")] [ShowIf("introCinematic")] public float walkAnimationDuration;
        [BoxGroup("Cinématique de fin")] public bool cinematiqueDeFin;
        [BoxGroup("Cinématique de fin")][ShowIf("cinematiqueDeFin")] public Transform[] playerDestinations;
        [BoxGroup("Cinématique de fin")][ShowIf("cinematiqueDeFin")] public Transform[] filleDestinations;
        [BoxGroup("Cinématique de fin")][ShowIf("cinematiqueDeFin")] public float[] playerTTR;
        [BoxGroup("Cinématique de fin")][ShowIf("cinematiqueDeFin")] public float[] filleTTR;
        [BoxGroup("Cinématique de fin")][ShowIf("cinematiqueDeFin")] public float[] playerWaitingTime;
        [BoxGroup("Cinématique de fin")][ShowIf("cinematiqueDeFin")] public float[] filleWaitingTime;
        [BoxGroup("Cinématique de fin")][ShowIf("cinematiqueDeFin")] public bool lookAtFille;
        [BoxGroup("Cinématique de fin")] [ShowIf("cinematiqueDeFin")] public float timeToPickChapo;
        [BoxGroup("Cinématique de fin")] [ShowIf("cinematiqueDeFin")] public float timeToPutChapo;
        [BoxGroup("Cinématique de fin")] [ShowIf("cinematiqueDeFin")] public Transform girlHand;
        [BoxGroup("Cinématique de fin")] [ShowIf("cinematiqueDeFin")] public Transform girlHead;
        [BoxGroup("Cinématique de fin")] [ShowIf("cinematiqueDeFin")] public Transform chapo;
        [BoxGroup("Cinématique de fin")] [ShowIf("cinematiqueDeFin")] public GameObject fireHead;
        public Transform laPetite;
        private Vector3 savedLeft;
        private Vector3 savedRight;
        [Foldout("Références")] public PauseMenu pauseMenu;
        [Foldout("Références")] public CameraManager cameraManager;
        [HideInInspector]public bool isDead;
        
        [SerializeField] private GameObject musicBoxGameObject;
        private Coroutine _musicBoxCoroutine;
        [SerializeField] private GameObject scribblingGameObject;
        private Coroutine _scribblingCoroutine;
        [SerializeField] private float scribblingFadeDuration;
        [SerializeField] private float scribblingPitchInterval;

        public float jumpPower = 0.3f;

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
            controls.Player.Move.performed += Move;
            controls.Player.InteractEnter.performed += PushPullEnter;
            controls.Player.InteractEnter.canceled += PushPullEnter;
            controls.Player.InteractEnter.canceled += SecondaryInteract;
            controls.Player.Sprint.performed += _ => TogleSprint();
            controls.Player.SecondaryEnter.performed += SecondaryInteract;
            controls.Player.SecondaryEnter.canceled +=  SecondaryInteract;
            controls.Player.SecondaryEnter.canceled +=  PushPullEnter;
            if (pauseMenu)controls.Player.ShowUI.performed += pauseMenu.ShowOption;
            savedMaxSpeed = maxSpeed;
                gamepad = Gamepad.current;
                if (introCinematic)
                {
                    StartCoroutine(IntroCinematic());
                }
                else
                {
                    if (cone)
                    {
                        var aga = Instantiate(cone.gameObject, characterHead.position,Quaternion.identity, characterHead);
                        aga.transform.localPosition = finalConePosition;
                        aga.transform.DOLocalRotate(finalConeRotation, 0);
                        Debug.Log("Le cône tête de cul !");
                    }
                    rig[0].gameObject.SetActive(false);
                    rig[1].gameObject.SetActive(true);
                    controls.Enable();
                    controls.Player.Enable();
                }
                DOTween.SetTweensCapacity(2000,1000);
        }
        
        // Update is called once per frame
        IEnumerator IntroCinematic()
        {
            //Debug.Log("Starting cinematic");
            rb.isKinematic = true;
            rig[0].SetBool("isBullshit",true);
            yield return new WaitForSeconds(timeToStandUp);
            //Debug.Log("Walking to cone");
            rig[0].SetFloat("Speed", walkAnimationSpeed);
            transform.DOJump(pointToMove.position,0,0, walkAnimationDuration);
            yield return new WaitForSeconds(walkAnimationDuration);
            //Debug.Log("Picking up cone");
            rig[0].SetFloat("Speed", -0.1f);
            yield return new WaitForSeconds(timeToPickUpCone);
            //Debug.Log("Picked up cone");
            cone.SetParent(characterHand);
            yield return new WaitForSeconds(timeToPutOnCone);
            //Debug.Log("Putting cone on head");
            cone.SetParent(characterHead);
            yield return new WaitForSeconds(0.2f);
            cone.DOLocalJump(finalConePosition,0.1f,1, conePutOnDuration);
            cone.DOLocalRotate(finalConeRotation, conePutOnDuration);
            yield return new WaitForSeconds(conePutOnDuration);
            //cone.position = finalConePosition;
            //Debug.Log("Completed");
            introCinematic = false;
            rig[0].SetBool("isBullshit",false);
            controls.Enable();
            controls.Player.Enable();
            rb.isKinematic = false;
            CinematicBands.instance.CloseBands();
        }
        private void FixedUpdate()
        {
            if (introCinematic)return;
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
            //incrémentation du inputBuffer pour éviter le spam de bouttons
            if (inputLag > 0)
            {
                inputLag -= Time.deltaTime;
            }

            if (accelerationTimer < grabAccelerationTime)
            {
                accelerationTimer += Time.fixedDeltaTime;
            }
            if (isGrabbing && grabTimer < grabAdjustTimer)
            {
                oui = false;
                grabTimer += Time.fixedDeltaTime;
                transform.position = Vector3.Lerp(transform.position, new Vector3(objectType.handlePos.position.x, transform.position.y, objectType.handlePos.position.z),grabTimer/grabAdjustTimer);
                RotateModel();
                return;
            }
            if(!oui && isGrabbing)
            {
                oui = true;
                if (!pushingPullingRotate)
                {
                    
                    SetJoint(true);
                    rig[0].SetBool("IsGrabbing", true);
                    canMove = true;
                    joint.autoConfigureConnectedAnchor = true;
                    objectToGrab.constraints = BaseConstraints;
                    
                }
                else
                {
                    savedLeft = -transform.right;
                    savedRight= transform.right;
                    SetJoint(true);
                    canMove = true;
                    rig[0].SetBool("IsGrabbing", true);
                    joint.autoConfigureConnectedAnchor = false;
                    if (!objectType.isColliding)
                    {
                        objectToGrab.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePosition;
                    }
                }
            }
            var speedFactor = rb.velocity.magnitude / maxSpeed;
            if (speedFactor > 1)
            {
                speedFactor = 1;
            }

            if (speedFactor < 0.1f)
            {
                speedFactor = 0.1f;
            }
            if (!proofOfConcept) rig[0].SetFloat("Speed",speedFactor);
            if (!canMove)
            {
                if (!proofOfConcept)
                {
                    rig[0].SetBool("isWalking", false);
                    rig[1].SetBool("isWalking", false);
                }
                return;
            }
            if (!controls.Player.Move.IsPressed() && isGrounded)    //Si ya aucune input du joueur
            {
                if (!proofOfConcept)
                {
                    rig[0].SetBool("isWalking", false);
                    rig[1].SetBool("isWalking", false);
                }

                if(isGrabbing && pushingPullingRotate)
                {
                    savedLeft = -transform.right;
                    savedRight= transform.right;
                }
                rig[0].SetBool("IsPushing",false);
                Decelerate();
                gamepad?.SetMotorSpeeds(0f,0f);
                return;
            }
            if (isGrabbing && objectType.mobilityType is DynamicObject.MobilityType.CanMove or DynamicObject.MobilityType.MoveWithHandle)    //Si le joueur bouge en grabbant un truc
            {
                if (PlayerPrefs.GetInt("Vibrations",1) == 1)
                {
                    gamepad?.SetMotorSpeeds(0f,0f);
                }
                ApplyForce(grabbedSpeed);
            }
            else
            {
                ApplyForce(groundSpeed);    //Si le joueur bouge
            }
        }

        void Decelerate()
        {
            if (!canMove)return;
            playerDir = new Vector3(playerDir.x * 0.1f,playerDir.y,playerDir.z * 0.1f);
            rb.velocity *= decelerationFactor;
            rb.angularVelocity = Vector3.zero;
        }

                                                                    #region Actions
        private void TogleSprint() 
        {
            isSprinting = !isSprinting;
        }
        public void Move(InputAction.CallbackContext context)
        {
            var dir = context.ReadValue<Vector2>();
            playerDir = new Vector3(dir.x,playerDir.y, dir.y);
        }
        
                                                                            #region ManipulationDobjets
        public void PushPullEnter(InputAction.CallbackContext context)
        {
            if (!controls.Player.SecondaryEnter.IsPressed() && !controls.Player.InteractEnter.IsPressed())
            {
                rig[0].SetBool("IsGrabbing",false);
                pushingPullingRotate = false;
                isGrabbing = false;
                canMove = true;
            }
            if (context.ReadValue<float>() == 0 && !controls.Player.InteractEnter.IsPressed())
            {
                if (!objectToGrab || !objectType)return;
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
            }
            else
            {
                objectToGrab = GetClosestObject();
                if (!objectToGrab) return;
                objectType = objectToGrab.GetComponent<DynamicObject>();
                if (objectToGrab.GetComponent<ObjectReseter>())
                {
                    objectToGrab.GetComponent<ObjectReseter>().ResetObjects();
                    return;
                }

                pushingPullingRotate = false;
                rb.velocity = Vector3.zero;
                grabTimer = 0;
                canMove = false;
                switch (objectType.mobilityType)
                {
                    case DynamicObject.MobilityType.None:
                        return;
                    case DynamicObject.MobilityType.MoveWithHandle:
                        var checkForProximity = Physics.OverlapCapsule(
                            objectType.handlePos.position + Vector3.down,
                            objectType.handlePos.position + Vector3.up,
                            1);

                        bool isPlayerNear = false;
                        foreach (var coll in checkForProximity)
                        {
                            if (coll != playerColl) continue;
                            isPlayerNear = true;
                            /*StartCoroutine(SetPlayerPos(0.3f,
                                    new Vector3(objectType.handlePos.position.x, transform.position.y,
                                        objectType.handlePos.position.z)));*/
                            break;
                        }

                        if (!isPlayerNear)
                        {
                            Debug.Log("Player is too far from handle");
                            canMove = true;
                            return;
                        }
                        isGrabbing = true;
                        rig[0].SetBool("IsGrabbing", true);
                        break;
                }
            }
            //transform.DOMove(transform.position, 0.3f).OnComplete((() => { }));
        }

        /// <summary>
        /// Switch entre le rotate et le pousser tirer
        /// </summary>
        public void SecondaryInteract(InputAction.CallbackContext context)
        {
            if (!controls.Player.SecondaryEnter.IsPressed() && !controls.Player.InteractEnter.IsPressed())
            {
                rig[0].SetBool("IsGrabbing",false);
                pushingPullingRotate = false;
                isGrabbing = false;
                canMove = true;
            }
            if (context.ReadValue<float>() == 0 && !controls.Player.SecondaryEnter.IsPressed())
            {
                if (!objectToGrab || !objectType)return;
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
            }
            else
            {
                objectToGrab = GetClosestObject();
                if (!objectToGrab) return;
                objectType = objectToGrab.GetComponent<DynamicObject>();
                if (objectToGrab.GetComponent<ObjectReseter>())
                {
                    objectToGrab.GetComponent<ObjectReseter>().ResetObjects();
                    return;
                }
                
                pushingPullingRotate = true;
                rb.velocity = Vector3.zero;
                grabTimer = 0;
                canMove = false;
                switch (objectType.mobilityType)
                {
                    case DynamicObject.MobilityType.None:
                        return;
                    case DynamicObject.MobilityType.MoveWithHandle:
                        var checkForProximity = Physics.OverlapCapsule(
                            objectType.handlePos.position + Vector3.down,
                            objectType.handlePos.position + Vector3.up,
                            1);

                        bool isPlayerNear = false;
                        foreach (var coll in checkForProximity)
                        {
                            if (coll != playerColl) continue;
                            isPlayerNear = true;
                            /*StartCoroutine(SetPlayerPos(0.3f,
                                new Vector3(objectType.handlePos.position.x, transform.position.y,
                                    objectType.handlePos.position.z)));*/
                            break;
                        }

                        if (!isPlayerNear)
                        {
                            Debug.Log("Player is too far from handle");
                            canMove = true;
                            return;
                        }

                        break;
                }

                isGrabbing = true;
                pushingPullingRotate = true;
                rig[0].SetBool("IsGrabbing", true);
                //transform.DOMove(transform.position, 0.3f).OnComplete((() => {}));
                
            }


        }
        
        /// <summary>
        /// </summary>
        /// <param name="appliedModifier"> modifcateur de force</param>
        #endregion
        private void ApplyForce(float appliedModifier)
        {
            RotateModel();
            if (!proofOfConcept)
            {
                rig[0].SetBool("isWalking", true);
                rig[1].SetBool("isWalking", true);
            }
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
                if (Vector3.Distance(playerDir, objectToGrab.transform.forward) < Vector3.Distance(playerDir, -objectToGrab.transform.forward) && canPush) // Pushing
                {
                    //Debug.Log("pushing");
                    playerDir = new Vector3(fwrd.x, playerDir.y, fwrd.z);
                    var dirPush = playerDir - rb.velocity.normalized;
                    var dpush = Mathf.Abs(dirPush.x) + Mathf.Abs(dirPush.z);
                    if (Mathf.Abs(dpush) > 2f)
                    {
                        rb.velocity = Vector3.zero;
                        accelerationTimer = 0;
                    }
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
                    rig[0].SetBool("IsPushing",true);
                    
                    rb.AddForce(playerDir * (appliedModifier * (accelerationTimer/grabAccelerationTime)));
                }
                else if(Vector3.Distance(playerDir, objectToGrab.transform.forward) > Vector3.Distance(playerDir, -objectToGrab.transform.forward) && canPull) // Pulling
                { 
                    //Debug.Log("pulling");
                    playerDir = new Vector3(-fwrd.x, playerDir.y, -fwrd.z);
                    var dirPush = playerDir - rb.velocity.normalized;
                    var dpush = Mathf.Abs(dirPush.x) + Mathf.Abs(dirPush.z);
                    if (Mathf.Abs(dpush) > 2f)
                    {
                        accelerationTimer = 0;
                    }
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
                    rig[0].SetBool("IsPushing",false);
                    rb.AddForce(playerDir * (appliedModifier * (accelerationTimer/grabAccelerationTime)));
                }
                else
                {
                    Decelerate();
                }
                return;
            }
                //Rotate______________________________________________________________________________________________________________________________________
               
            if (isGrabbing && pushingPullingRotate && playerDir.magnitude > 0.1f)
            {
                rb.velocity = Vector3.zero;
                
                var fwrd = transform.forward;
                var leftDiff = playerDir - savedLeft;
                var rightDiff = playerDir - savedRight;
                //Debug.Log(Mathf.Abs(leftDiff.x + leftDiff.z) + "left");
                //Debug.Log(Mathf.Abs(rightDiff.x + rightDiff.z)+ "right");
                var dirModifier = 1f;
                // 1 is clockwise
                // -1 is counter-clockwise
                if (Mathf.Abs(leftDiff.x + leftDiff.z) > Mathf.Abs(rightDiff.x + rightDiff.z))
                {
                    dirModifier = canRotateCounterClockwise ? -1 : 0;
                }
                else
                {
                    dirModifier = canRotateClockwise ? 1 : 0;
                }
                transform.RotateAround(objectToGrab.position,transform.up, rotationSpeed * dirModifier);
                
                var delta = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                    new Vector2(objectType.handlePos.position.x, objectType.handlePos.position.z));
                if (delta > 0.05f)
                {
                    rb.velocity =  Vector3.zero;
                    transform.position = new Vector3(objectType.handlePos.position.x, transform.position.y,
                        objectType.handlePos.position.z);
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
                rb.AddForce((playerDir + Vector3.down) * (appliedModifier),ForceMode.Force);
                return;
            }
            
            rb.AddForce((playerDir + Vector3.down) * ((appliedModifier) * sprintSpeed),ForceMode.Force);

        }

        private void PickupObject()
        {
            if (isGrabbing)
            {
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
                objectToGrab.transform.DOJump(carrySpot, 1f, 1, pickUpTime).AppendCallback(() =>
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
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.AngleAxis(angle,Vector3.up),modelRotate);
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
        public void SetJoint(bool grabbing)
        {
            if (!objectToGrab)return;
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
                objectType = null;
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

        public IEnumerator OmgJeSuisSurpris(Transform objectToLookAt)
        {
            transform.DOJump(transform.position -transform.forward * 0.2f, jumpPower, 1, 0.2f);
            yield return new WaitForSeconds(0.4f);
            while (!canMove)
            {
                Vector3 dir = objectToLookAt.position - transform.position;
                var dirNormed = dir.normalized;
                var angle2 = Mathf.Atan2(dirNormed.x, dirNormed.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle2,Vector3.up);
                yield return new WaitForFixedUpdate();
            }
        }

        private Quaternion GetDir(Vector3 otherPos, Vector3 ownPos)
        {
            var dir = otherPos - ownPos;
            var dirNormed = dir.normalized;
            var angle = Mathf.Atan2(dirNormed.x, dirNormed.z) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(angle,Vector3.up);
        }

        public IEnumerator LaDerniereRoute()
        {
            
            //Debug.Log("Begining Cinematic");
            rb.isKinematic = true;
            DOTween.defaultEaseType = Ease.Linear;
            transform.rotation = GetDir(playerDestinations[0].position, transform.position);
            rb.velocity = Vector3.zero;
            rig[0].SetBool("isWalking", true);
            rig[1].SetBool("isWalking", true);
            rig[0].SetFloat("Speed", 1);
            rig[1].SetFloat("Speed", 1);
            introCinematic = true;
            var animFille = laPetite.gameObject.GetComponentInChildren<Animator>();
            transform.DOMove(playerDestinations[0].position, playerTTR[0]);
            yield return new WaitForSeconds(filleWaitingTime[0]);
            //Debug.Log("girl picks up hat");
            animFille.SetBool("isPickingUp",true);
            //laPetite.DORotateQuaternion(GetDir(filleDestinations[^1].position,laPetite.position),playerTTR[0] - filleTTR[0] + 0.2f);
            yield return new WaitForSeconds(playerTTR[0] - filleWaitingTime[0]);
            animFille.SetBool("isPickingUp",false);
            transform.DORotateQuaternion(GetDir(laPetite.position,transform.position),0.3f);
            lookAtFille = true;
            //Debug.Log("Player reached first point");
            //Debug.Log("player looks at girl");
            rig[0].SetBool("isWalking", false);
            rig[1].SetBool("isWalking", false);
            yield return new WaitForSeconds(timeToPickChapo);
            chapo.SetParent(girlHand);
            yield return new WaitForSeconds(timeToPutChapo);
            chapo.SetParent(girlHead);
            yield return new WaitForSeconds(0.2f);
            chapo.DOLocalJump(Vector3.up * 0.1f, 0.1f,1, conePutOnDuration);
            chapo.DOLocalRotate(Vector3.zero, conePutOnDuration);
            fireHead.SetActive(false);
            yield return new WaitForSeconds(filleWaitingTime[1] - timeToPickChapo - timeToPutChapo -conePutOnDuration - 0.2f);
            //Debug.Log("Girl Looks at PLayer");
            laPetite.DOLocalRotateQuaternion(GetDir(transform.position, laPetite.position),0.5f);
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(LookingAtGirl());
            yield return new WaitForSeconds(playerWaitingTime[0] - 0.5f);
            //Debug.Log("player moves next to girl");
            transform.rotation = GetDir(playerDestinations[1].position, transform.position);
            transform.DOMove(playerDestinations[1].position, playerTTR[1]);
            rig[0].SetBool("isWalking", true);
            rig[1].SetBool("isWalking", true);
            rig[0].SetFloat("Speed", 1);
            rig[1].SetFloat("Speed", 1);
            yield return new WaitForSeconds(playerTTR[1]);
            lookAtFille = false;
            //Debug.Log("player walks to door");
            transform.rotation = GetDir(playerDestinations[2].position, transform.position);
            laPetite.DORotateQuaternion(GetDir(filleDestinations[0].position,laPetite.position),0.3f);
            transform.DOMove(playerDestinations[2].position, playerTTR[2]).SetEase(Ease.Linear);
            rig[0].SetBool("isWalking", true);
            rig[1].SetBool("isWalking", true);
            rig[0].SetFloat("Speed", 1);
            rig[1].SetFloat("Speed", 1);
            yield return new WaitForSeconds(0.3f);
            //Debug.Log("girl runs to door");
            animFille.SetBool("isRunning",true);
            laPetite.rotation = GetDir(filleDestinations[0].position, laPetite.position);
            laPetite.DOMove(filleDestinations[0].position, filleTTR[0] - 0.3f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(filleTTR[0] - 0.3f);
            //Debug.Log("reached door");
            animFille.SetBool("isRunning",false);
            rig[0].SetBool("isWalking", false);
            rig[1].SetBool("isWalking", false);
            transform.rotation = GetDir(playerDestinations[3].position, transform.position);
            laPetite.rotation = GetDir(filleDestinations[1].position,laPetite.position);
            yield return new WaitForSeconds(filleWaitingTime[2]);
            //Debug.Log("girls runs to door");
            animFille.SetBool("isRunning",true);
            laPetite.DOMove(filleDestinations[1].position, filleTTR[1]);
            yield return new WaitForSeconds(playerWaitingTime[1]);
            //Debug.Log("player follows girls");
            rig[0].SetBool("isWalking", true);
            rig[1].SetBool("isWalking", true);
            rig[0].SetFloat("Speed", 0.5f);
            rig[1].SetFloat("Speed", 0.5f);
            transform.DOMove(playerDestinations[^1].position, playerTTR[^1]);
            yield return new WaitForSeconds(playerTTR[^1]);
            cameraManager.Credits();
            laPetite.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        IEnumerator LookingAtGirl()
        {
            while (lookAtFille)
            {
                laPetite.rotation = GetDir(transform.position,laPetite.position);
                yield return new WaitForFixedUpdate();
            }
        }

        internal void MusicBoxSound()
        {
            _musicBoxCoroutine ??= StartCoroutine(MusicBoxSoundCoroutine());
        }

        private IEnumerator MusicBoxSoundCoroutine()
        {
            musicBoxGameObject.SetActive(true);
            yield return new WaitForSeconds(musicBoxGameObject.GetComponent<AudioSource>().clip.length);
            musicBoxGameObject.SetActive(false);
            _musicBoxCoroutine = null;
        }

        internal void ScribblingSound(float duration)
        {
            if (_scribblingCoroutine != null)
            {
                StopCoroutine(_scribblingCoroutine);
            }
            StartCoroutine(ScribblingSoundCoroutine(duration));
        }

        private IEnumerator ScribblingSoundCoroutine(float duration)
        {
            scribblingGameObject.GetComponent<AudioSource>().volume = 1;
            scribblingGameObject.GetComponent<AudioSource>().pitch = Random.Range(1f-scribblingPitchInterval*0.5f, 1f+scribblingPitchInterval*0.5f);
            scribblingGameObject.SetActive(true);
            yield return new WaitForSeconds(duration-scribblingFadeDuration);
            scribblingGameObject.GetComponent<AudioSource>().DOFade(0, scribblingFadeDuration);
            yield return new WaitForSeconds(scribblingFadeDuration);
            scribblingGameObject.SetActive(false);
            _scribblingCoroutine = null;
        }
    }
    
}