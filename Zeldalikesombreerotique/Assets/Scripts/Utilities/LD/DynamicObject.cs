using NaughtyAttributes;
using Player;
using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace Utilities
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
        [Tooltip("Est'ce que l'objet a essayé de se renverser?")]public bool hasToppled;
        private Rigidbody rb;
        [Tooltip("La force utilisée pour renverser l'objet(les plus gros objets nécessiteront plus de force)")]public float toppleForce;
        private BoxCollider col;
        private bool clockwiseTimer;
        private bool counterClockwiseTimer;
        [ReadOnly]public bool isColliding;
        private bool _start;
        public float overlapBox = 1.6f;
        

        private void Awake()
        {
            _start = true;
            mesh = GetComponentInChildren<MeshRenderer>();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<BoxCollider>();
        }

        private void OnDrawGizmosSelected()
        {
            //if (!_start || gameObject.CompareTag("Footprint")) return;
            //Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, overlapBox * Vector3.one);
        }

        private void FixedUpdate()
        {
            if (rb)
            {
                if (!rb.isKinematic && !PlayerController.instance.pushingPullingRotate)
                {
                    LautreCheckDeSesMorts();
                }
            }
            if (visibilityType != VisibilityType.DelayedReappear) return;
            mesh.material.color = Color.Lerp(mesh.material.color, Color.white, reappearanceSpeed);
            childCollider.enabled = mesh.material.color.a > 0.9f;
        }

        private void CheckDeSesMorts(Collision other)
        {
            if (other.gameObject.CompareTag("Untagged") || other.gameObject.CompareTag("Player") ||
                !PlayerController.instance.isGrabbing) return;
            var frontSide = transform.forward;
            var backSide = -transform.forward;
            var delta1 = Vector3.Distance(frontSide, other.collider.ClosestPoint(frontSide))- Vector3.Distance(backSide, other.collider.ClosestPoint(backSide));
            if (delta1 > 0)
            {
                PlayerController.instance.canRotateClockwise = false;
                PlayerController.instance.canRotateCounterClockwise = false;
                return;
            }
            if (PlayerController.instance.pushingPullingRotate)
            {
                    
                PlayerController.instance.gamepad?.SetMotorSpeeds(PlayerController.instance.rumbleIntensity,PlayerController.instance.rumbleIntensity);
                rb.constraints = RigidbodyConstraints.FreezeAll;
                Collider[] oui;
                do
                {
                    var closer = PlayerController.instance.transform;
                    PlayerController.instance.transform.position +=  -closer.forward * .03f;
                    transform.position = closer.position + 
                                         closer.forward * 
                                         Vector3.Distance(transform.position,handlePos.position);
                    oui = Physics.OverlapBox(col.center + transform.position, col.size / 2 + 0.1f * Vector3.one, transform.rotation,PlayerController.instance.mask);
                    Debug.Log(oui.Length);
                } while (oui.Length > 1);
            }
            isColliding = true;
        }

        private void LautreCheckDeSesMorts()
        {
            var oui = Physics.OverlapBox(col.center + transform.position, col.size * PlayerController.instance.overlapBoxSize + Vector3.up, transform.rotation,PlayerController.instance.mask);

            /*foreach (var non in oui)
            {
                Debug.Log(non, non);
            }*/

            var frontSide = transform.forward;
            var backSide = -transform.forward;
            if (oui.Length ==1)
            {
                
                PlayerController.instance.canPull = true;
                PlayerController.instance.canPush = true;
            }
            for (int i = 0; i < oui.Length; i++)
            {
                if (i == 0) continue;
                var delta1 = Vector3.Distance(frontSide, oui[i].ClosestPoint(frontSide))- Vector3.Distance(backSide, oui[i].ClosestPoint(backSide));
                Debug.Log(delta1);
                if (delta1 > 0)
                {
                    PlayerController.instance.canPull = false;
                    PlayerController.instance.canPush = true;
                }
                else
                {
                    PlayerController.instance.canPull = true;
                    PlayerController.instance.canPush = false;
                }
            }
        }
        private void OnCollisionEnter(Collision other)
        {
            CheckDeSesMorts(other);
        }
        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerController.instance.isProtected = true;
            }
            CheckDeSesMorts(other);
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
    }
}
