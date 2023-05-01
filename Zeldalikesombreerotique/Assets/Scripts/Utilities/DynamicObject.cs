using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using Player;
using UnityEngine;

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
        [ShowIf("visibilityType",VisibilityType.DelayedReappear)] [SerializeField] [Tooltip("Le collider dans le mesh en enfant du préfab")]internal Collider _childCollider;
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

        private void Awake()
        {
            mesh = GetComponentInChildren<MeshRenderer>();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<BoxCollider>();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(col.center, col.size + Vector3.one * 0.1f);
        }

        private void FixedUpdate()
        {
            if (visibilityType == VisibilityType.DelayedReappear)
            {
                mesh.material.color = Color.Lerp(mesh.material.color, Color.white, reappearanceSpeed);
                if (mesh.material.color.a > 0.9f)
                {
                    _childCollider.enabled = true;
                }
                else
                {
                    _childCollider.enabled = false;
                }
            }

            /*if (pushTimer > 0.1f && !hasToppled)
            {
                hasToppled = true;
                rb.constraints = RigidbodyConstraints.None;
                PlayerController.instance.joint.connectedBody = null;
                PlayerController.instance.joint.gameObject.SetActive(false);
                PlayerController.instance.isGrabbing = false;
                var dir = transform.position -PlayerController.instance.transform.position;
                rb.AddForceAtPosition(toppleForce* dir.normalized,transform.position + Vector3.up * 3);
                PlayerController.instance.controls.Disable();
                PlayerController.instance.isProtected = true;
                transform.DOScale(transform.localScale, 1.5f).OnComplete((() =>
                {
                    rb.isKinematic = true;
                    transform.DOMove(transform.position + Vector3.down * 0.5f, 0.5f).OnComplete((() =>
                    {
                        
                        PlayerController.instance.transform.DOJump(
                            PlayerController.instance.transform.position + PlayerController.instance.transform.forward*2,
                            0.5f, 1, 0.5f).AppendCallback((() =>
                        {
                            PlayerController.instance.controls.Enable();
                        }));
                    }));

                }));
            }*/
        }

        void CheckDeSesMorts(Collision other)
        {
            
            if (!other.gameObject.CompareTag("Untagged") && !other.gameObject.CompareTag("Player") && PlayerController.instance.isGrabbing)
            {
                if (PlayerController.instance.pushingPulling_Rotate)
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    Collider[] oui;
                    do
                    {
                        PlayerController.instance.transform.position +=  -PlayerController.instance.transform.forward * .03f;
                        transform.position = PlayerController.instance.transform.position + 
                                             PlayerController.instance.transform.forward * 
                                             Vector3.Distance(transform.position,handlePos.position);
                        oui = Physics.OverlapBox(col.center + transform.position, col.size / 2 + 0.1f * Vector3.one, transform.rotation,PlayerController.instance.mask);
                        foreach (var VARIABLE in oui)
                        {
                            Debug.Log(VARIABLE, VARIABLE);
                        }

                    } while (oui.Length > 1);
                }
                isColliding = true;
                return;
                var leftSide = -transform.right;
                var rightSide = transform.right;
                var delta = Mathf.Abs(Vector3.Distance(leftSide, other.collider.ClosestPoint(leftSide))- Vector3.Distance(rightSide, other.collider.ClosestPoint(rightSide)));
                Debug.Log(delta);
                if (delta < 0.1f)
                {
                    //Debug.Log("Colliding full front");
                    //PlayerController.instance.transform.Translate(-PlayerController.instance.transform.forward * .5f);
                }

                if (Vector3.Distance(leftSide,other.collider.ClosestPoint(leftSide)) > Vector3.Distance(rightSide,other.collider.ClosestPoint(rightSide)))
                {
                    var rightForwardCorner = transform.position + (rightSide + transform.forward).normalized;
                    var rightBackCorner = transform.position + (rightSide + -transform.forward).normalized;
                    if (Vector3.Distance(rightForwardCorner,other.collider.ClosestPoint(rightForwardCorner)) > Vector3.Distance(rightBackCorner,other.collider.ClosestPoint(rightBackCorner)))
                    {
                        //Right Back Quadrant
                        Debug.Log("Colliding with right back quadrant");
                        //PlayerController.instance.canRotateClockwise = false;
                        
                    }
                    else
                    {
                        //Right Forward Quadrant
                        Debug.Log("Colliding with right forward quadrant");
                        //PlayerController.instance.canRotateCounterClockwise = false;
                    }
                }
                else
                {
                    var leftForwardCorner = transform.position + (leftSide + transform.forward).normalized;
                    var leftBackCorner = transform.position + (leftSide + -transform.forward).normalized;
                    if (Vector3.Distance(leftForwardCorner,other.collider.ClosestPoint(leftForwardCorner)) > Vector3.Distance(leftBackCorner,other.collider.ClosestPoint(leftBackCorner)))
                    {
                        //Left Back Quadrant
                        Debug.Log("Colliding with left back quadrant");
                        //PlayerController.instance.canRotateCounterClockwise = false;
                    }
                    else
                    {
                        //Left Forward Quadrant
                        Debug.Log("Colliding with left forward quadrant");
                        //PlayerController.instance.canRotateClockwise = false;
                    }
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
        
        

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Shadows") && !hasToppled)
            {
                pushTimer += Time.deltaTime;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Shadows"))
            {
                pushTimer = 0;
            }
        }

        private void OnCollisionExit(Collision other)
        {            

            if (other.gameObject.CompareTag("Player"))
            {
                PlayerController.instance.isProtected = false;
            }
            if (!other.gameObject.CompareTag("Untagged") && !other.gameObject.CompareTag("Player"))
            {
                if (PlayerController.instance.pushingPulling_Rotate)
                {
                    rb.constraints = RigidbodyConstraints.FreezePosition | 
                                     RigidbodyConstraints.FreezeRotationX |
                                     RigidbodyConstraints.FreezeRotationZ;
                }
                isColliding = false;
            }
        }
    }
}
