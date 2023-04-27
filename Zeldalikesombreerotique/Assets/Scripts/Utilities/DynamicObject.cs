using System;
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

        private void Awake()
        {
            mesh = GetComponentInChildren<MeshRenderer>();
            rb = GetComponent<Rigidbody>();
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

        private void OnCollisionStay(Collision other)
        {


            if (other.gameObject.CompareTag("Player"))
            {
                PlayerController.instance.isProtected = true;
            }
            if (!other.gameObject.CompareTag("Untagged") && !other.gameObject.CompareTag("Player"))
            {
                PlayerController.instance.allowedToRotate = false;
            }
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
                PlayerController.instance.allowedToRotate = true;
            }
        }
    }
}
