using System;
using DG.Tweening;
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
            CanMove
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
        [SerializeField] internal Collider _childCollider;
        internal MeshRenderer mesh;
        public float reappearanceSpeed;
        public float pushTimer;
        public bool hasToppled;
        private Rigidbody rb;
        public float toppleForce;

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

            if (pushTimer > 0.1f && !hasToppled)
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
            }
        }

        private void OnCollisionStay(Collision other)
        {


            if (other.gameObject.CompareTag("Player"))
            {
                PlayerController.instance.isProtected = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Shadows"))
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
        }
    }
}
