using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

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
        public float overlapBox = 1.6f;
        public bool isTutorial;
        [ShowIf("isTutorial")] public float detectionRadius;
        [ShowIf("isTutorial")] public float distanceTravelledRequired;
        [ShowIf("isTutorial")] public float rotationRequired;
        [ShowIf("isTutorial")] public Transform pushPullUI;
        private List<SpriteRenderer> pushPullSprites = new List<SpriteRenderer>();
        [ShowIf("isTutorial")] public Transform rotateUI;
        private List<SpriteRenderer> rotateSprites = new List<SpriteRenderer>();
        private bool pushPullActive;
        private bool rotateActive;
        private Vector3 lastSavedPos;
        private Vector3 lastSavedRotation;
        public Vector3 spawnPos;


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
                rotationRequired -= diff;
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
                if (PlayerPrefs.GetInt("Vibrations", 1) == 1)
                {
                    PlayerController.instance.gamepad?.SetMotorSpeeds(PlayerController.instance.rumbleIntensity,PlayerController.instance.rumbleIntensity);
                }
                rb.constraints = RigidbodyConstraints.FreezeAll;
                Collider[] oui;
                do
                {
                    var closer = PlayerController.instance.transform;
                    transform.position += -transform.forward * 0.1f; // Vector3.Distance(transform.position,handlePos.position);
                    oui = Physics.OverlapBox(col.center + transform.position, col.size / 2 + 0.1f * Vector3.one, transform.rotation,PlayerController.instance.mask);
                    Debug.Log(oui.Length);
                } while (oui.Length > 1);
            }
            isColliding = true;
        }

        private void LautreCheckDeSesMorts()
        {
            var oui = Physics.OverlapBox(transform.position, col.size * PlayerController.instance.overlapBoxSize + Vector3.up, transform.rotation,PlayerController.instance.mask);

            

            var frontSide = transform.forward;
            var backSide = -transform.forward;
            if (oui.Length ==1)
            {
                
                PlayerController.instance.canPull = true;
                PlayerController.instance.canPush = true;
            }
            /*foreach (var non in oui)
            {
                Debug.Log(non, non);
            }*/
            for (int i = 0; i < oui.Length; i++)
            {
                if (oui[i] == col) continue;
                if (oui[i] == GetComponentInChildren<Collider>()) continue;
                var distanceToFrontSide = Vector3.Distance(transform.position + frontSide, oui[i].ClosestPoint(transform.position + frontSide));
                var distanceToBackSide = Vector3.Distance(transform.position + backSide, oui[i].ClosestPoint(transform.position + backSide));
                if (distanceToFrontSide > distanceToBackSide)
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

        IEnumerator FadeOut(List<SpriteRenderer> sprites, bool chainFadeIn)
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
                Debug.Log("oui");
                yield break;
            }

            if (chainFadeIn)
            {
                Debug.Log("cbon laaa");
                lastSavedRotation = transform.rotation.eulerAngles;
                StartCoroutine(FadeIn(rotateSprites));
            }
        }
        IEnumerator FadeIn(List<SpriteRenderer> sprites)
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
