using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Player;
using UnityEngine;

// ReSharper disable Unity.InefficientPropertyAccess
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace Utilities.LD
{
    public class DynamicObject : MonoBehaviour
    {
        public enum MobilityType
        {
            None,
            CanCarry,
            CanMove,
            MoveWithHandle
        }
        
        public enum VisibilityType
        {
            None,
            CanBeRevealed,
            CanBeHidden,
            DelayedReappear
        }
        
        [SerializeField] internal MobilityType mobilityType;
        [SerializeField] internal VisibilityType visibilityType;
        [SerializeField] internal GameObject meshObjectForVisibility;
        [ShowIf("visibilityType",VisibilityType.DelayedReappear)] [SerializeField] [Tooltip("Le collider dans le mesh en enfant du préfab")]internal Collider childCollider;
        internal MeshRenderer mesh;
        [ShowIf("visibilityType",VisibilityType.DelayedReappear)][SerializeField][Tooltip("Valeur entre 0 et 1, détermine la vitesse à laquelle l'ombre réapparait")] internal float reappearanceSpeed;
        private float pushTimer;
        [ShowIf("mobilityType",MobilityType.MoveWithHandle)][SerializeField][Tooltip("La ou l'avatar du joueur se positionne pour manipuler objet")] internal Transform handlePos;
        private Rigidbody rb;
        private BoxCollider col;
        private bool clockwiseTimer;
        private bool counterClockwiseTimer;
        [ReadOnly]public bool isColliding;
        public float overlapBox = 1.6f;
        public bool isTutorial;
        [ShowIf("isTutorial")] public float detectionRadius;
        [ShowIf("isTutorial")] public float distanceTravelledRequired;
        [ShowIf("isTutorial")] public float rotationRequired;
        [ShowIf("isTutorial")] public Transform pushPullUI;
        private List<SpriteRenderer> pushPullSprites = new List<SpriteRenderer>();
        [ShowIf("isTutorial")] public Transform rotateUI;
        private List<SpriteRenderer> rotateSprites = new List<SpriteRenderer>();
        public SpriteRenderer[] limitations;
        private bool pushPullActive;
        private bool rotateActive;
        private Vector3 lastSavedPos;
        private Vector3 lastSavedRotation;
        public Vector3 spawnPos;
        private List<float> closestPos = new List<float>();
        public float timer = 0;
        public Color appearColor;
        public Color invisColor;


        private void Awake()
        {
            mesh = GetComponentInChildren<MeshRenderer>();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<BoxCollider>();
            
            if (!isTutorial) return;
            
            for (int i = 0; i < pushPullUI.childCount; i++)
            {
                pushPullSprites.Add(pushPullUI.GetChild(i).GetComponent<SpriteRenderer>());
            }
            
            for (int i = 0; i < rotateUI.childCount; i++)
            {
                rotateSprites.Add(rotateUI.GetChild(i).GetComponent<SpriteRenderer>());
            }

            spawnPos = transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            //if (!_start || gameObject.CompareTag("Footprint")) return;
            //Gizmos.matrix = transform.localToWorldMatrix;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, overlapBox * Vector3.one);
            
            if (isTutorial)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position,detectionRadius);
            }
        }

        private void FixedUpdate()
        {
            if (rb)
            {
                if (!rb.isKinematic && !PlayerController.instance.pushingPullingRotate)
                {
                    LautreCheckDeSesMorts();
                }
                else if (PlayerController.instance.pushingPullingRotate && PlayerController.instance.objectType == this)
                {
                    CheckDeSesMorts();
                }

                if (PlayerController.instance.objectType == this)
                {
                    if (timer <= 1)
                    { 
                        timer += Time.deltaTime;
                    }
                    foreach (var sprite in limitations)
                    {
                        sprite.color = Color.Lerp(invisColor, appearColor,timer);
                    }
                }
                else
                {
                    if (timer >= 0)
                    {
                        timer -= Time.deltaTime;
                    }
                    foreach (var sprite in limitations)
                    {
                        sprite.color = Color.Lerp( invisColor, appearColor,timer);
                    }
                }
                
            }
            

            if (isTutorial && PlayerController.instance)
            {
                var coll = Physics.OverlapSphere(transform.position,detectionRadius);
                foreach (var hit in coll)
                {
                    if (hit.gameObject == PlayerController.instance.gameObject && isTutorial)
                    {
                        isTutorial = false;
                        pushPullActive = true;
                        StartCoroutine(FadeIn(pushPullSprites));
                        lastSavedPos = transform.position;
                    }
                }
            }
            
            if (pushPullActive)
            {
                var diff = Mathf.Abs(lastSavedPos.magnitude - transform.position.magnitude);
                distanceTravelledRequired -= diff;
                
                if (distanceTravelledRequired <= 0)
                {
                    pushPullActive = false;
                    rotateActive = true;
                    StartCoroutine(FadeOut(pushPullSprites,true));
                }
            }

            if (rotateActive)
            {
                var diff = Mathf.Abs(lastSavedRotation.magnitude - transform.rotation.eulerAngles.magnitude);
                lastSavedRotation = transform.rotation.eulerAngles;
                rotationRequired -= diff;
                
                // Debug.Log(diff + ", required: "+ rotationRequired);
                
                if (rotationRequired <= 0)
                {
                    rotateActive = false;
                    StartCoroutine(FadeOut(rotateSprites, false));
                }
            }
            
            if (visibilityType != VisibilityType.DelayedReappear) return;
            
            mesh.material.color = Color.Lerp(mesh.material.color, Color.white, reappearanceSpeed);
            childCollider.enabled = mesh.material.color.a > 0.9f;
        }

        private void CheckDeSesMorts()
        {
            if (!(Mathf.Abs(PlayerController.instance.playerDir.x) >= 0.1f) &&
                !(Mathf.Abs(PlayerController.instance.playerDir.z) >= 0.1f))return;

            var pl = PlayerController.instance.transform;
            var oui = Physics.OverlapBox(col.center + transform.position, 
                col.size * PlayerController.instance.overlapBoxSize + Vector3.up,
                transform.rotation,~PlayerController.instance.mask,
                QueryTriggerInteraction.Ignore);
            
            
            /*foreach (var non in oui)
            {
                Debug.Log(non+ "Check si les objets on les bons layers",non );
            }*/
            
            if (oui.Length > 1)
            {
                if (PlayerPrefs.GetInt("Vibrations", 1) == 1)
                {
                    PlayerController.instance.gamepad?.SetMotorSpeeds(PlayerController.instance.rumbleIntensity,PlayerController.instance.rumbleIntensity);
                }

                var rightSide = transform.right;
                var leftSide = -transform.right;
                for (int i = 0; i < oui.Length; i++)
                {
                    if (oui[i] == col) continue;
                    if (oui[i] == GetComponentInChildren<Collider>()) continue;
                    Debug.Log(oui[i].name,oui[i].gameObject);
                    var distToRight = Vector3.Distance(transform.position + rightSide, oui[i].ClosestPoint(transform.position + rightSide));
                    var distToLeft = Vector3.Distance(transform.position + leftSide, oui[i].ClosestPoint(transform.position + leftSide));
                    if (distToRight < distToLeft)
                    {
                        transform.position += -transform.right * 0.01f;
                        pl.position += -pl.right * 0.01f;
                    }
                    else
                    {
                        transform.position += transform.right * 0.01f;
                        pl.position += pl.right * 0.01f;
                    }
                    transform.position += -transform.forward * 0.01f; // Vector3.Distance(transform.position,handlePos.position);
                    pl.position += -pl.forward * 0.01f;
                }
            }
        }

        private void LautreCheckDeSesMorts()
        {
            var oui = Physics.OverlapBox(transform.position, col.size * PlayerController.instance.overlapBoxSize + Vector3.up, transform.rotation,~PlayerController.instance.mask,
                QueryTriggerInteraction.Ignore);
            
            var frontSide = transform.forward;
            var backSide = -transform.forward;
            var frontRight = frontSide + transform.right;
            var frontLeft = frontSide - transform.right;
            var backRight = backSide + transform.right;
            var backLeft = backSide - transform.right;
            
            if (oui.Length == 1)
            {
                PlayerController.instance.canPull = true;
                PlayerController.instance.canPush = true;
            }
            
            /*foreach (var non in oui)
            {
                Debug.Log(non, non);
            }*/
            
            for (var i = 0; i < oui.Length; i++)
            {
                if (oui[i] == col) continue;
                if (oui[i] == GetComponentInChildren<Collider>()) continue;
                
                Debug.Log(oui[i].name,oui[i].gameObject);
                closestPos.Clear();
                
                closestPos.Add(Vector3.Distance(transform.position + frontRight, oui[i].ClosestPoint(transform.position + frontRight)));  //FrontRight = 0
                closestPos.Add(Vector3.Distance(transform.position + frontLeft, oui[i].ClosestPoint(transform.position + frontLeft)));    //FrontLeft  = 1
                closestPos.Add(Vector3.Distance(transform.position + backRight, oui[i].ClosestPoint(transform.position + backRight)));    //BackRight  = 2
                closestPos.Add(Vector3.Distance(transform.position + backLeft, oui[i].ClosestPoint(transform.position + backLeft)));      //BackLeft   = 3


                switch (closestPos.IndexOf(closestPos.Min()))
                {
                    case 0:
                        if (Mathf.Abs(closestPos[0] - closestPos[1]) >= 0.3f)
                        {
                            transform.position -= transform.right * 0.01f;
                            PlayerController.instance.transform.position -= PlayerController.instance.transform.right * 0.01f;   
                        }
                        break;
                    case 1:
                        if (Mathf.Abs(closestPos[0] - closestPos[1]) >= 0.3f)
                        {
                            transform.position += transform.right * 0.01f;
                            PlayerController.instance.transform.position += PlayerController.instance.transform.right * 0.01f;
                        }
                        break;
                    case 2:
                        if (Mathf.Abs(closestPos[2] - closestPos[3]) >= 0.3f)
                        {
                            transform.position -= transform.right * 0.01f;
                            PlayerController.instance.transform.position -= PlayerController.instance.transform.right * 0.01f;
                        }
                        break;
                    case 3:
                        if (Mathf.Abs(closestPos[2] - closestPos[3]) >= 0.3f)
                        {
                            transform.position += transform.right * 0.01f;
                            PlayerController.instance.transform.position += PlayerController.instance.transform.right * 0.01f;
                        }
                        break;
                }
                /*if (distanceToFrontSide > distanceToBackSide)
                {
                    PlayerController.instance.canPull = false;
                    PlayerController.instance.canPush = true;
                }
                else
                {
                    PlayerController.instance.canPull = true;
                    PlayerController.instance.canPush = false;
                }*/
            }
        }
        
        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerController.instance.isProtected = true;
            }
        }
        
        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerController.instance.isProtected = false;
            }

            if (other.gameObject.CompareTag("Untagged") || other.gameObject.CompareTag("Player")) return;
            
            if (PlayerController.instance.pushingPullingRotate)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                                 RigidbodyConstraints.FreezePosition |
                                 RigidbodyConstraints.FreezeRotationZ;
            }
            
            isColliding = false;
            PlayerController.instance.canRotateClockwise = true;
            PlayerController.instance.canRotateCounterClockwise = true;
        }

        private IEnumerator FadeOut(List<SpriteRenderer> sprites, bool chainFadeIn)
        {
            foreach (var sr in sprites)
            {
                sr.color = Color.Lerp(sr.color,
                    new Color(sr.color.r, sr.color.g, sr.color.b, 0),0.06f);
            }
            
            yield return new WaitForFixedUpdate();
            
            if (sprites[0].color.a > 0.001f)
            {
                StartCoroutine(FadeOut(sprites, chainFadeIn));
                // Debug.Log("oui");
                yield break;
            }

            if (chainFadeIn)
            {
                // Debug.Log("cbon laaa");
                lastSavedRotation = transform.rotation.eulerAngles;
                StartCoroutine(FadeIn(rotateSprites));
            }
        }

        private IEnumerator FadeIn(List<SpriteRenderer> sprites)
        {
            foreach (var sr in sprites)
            {
                sr.color = Color.Lerp(sr.color,
                    new Color(sr.color.r, sr.color.g, sr.color.b, 1),0.03f);
            }
            
            yield return new WaitForFixedUpdate();
            
            if (sprites[0].color.a <= 0.9f)
            {
                StartCoroutine(FadeIn(sprites));
            }
        }
    }
}
