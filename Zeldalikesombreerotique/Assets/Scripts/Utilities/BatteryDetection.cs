using Player;
using UnityEngine;

namespace Utilities
{
    public class BatteryDetection : MonoBehaviour
    {
        private enum ObjectType
        {
            Light,
            Car
        }
        
        [Tooltip("Object type")] [SerializeField] private ObjectType objectType;
        [Tooltip("Light game object")] [SerializeField] private GameObject lightObject;
        [Tooltip("Mesh of the light")] [SerializeField] private GameObject meshObject;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Battery") && PlayerController.instance.isGrabbing)
            {
                PlayerController.instance.isGrabbing = false;
                switch (objectType)
                {
                    case ObjectType.Light:
                        Destroy(other.gameObject);
                        var material = meshObject.GetComponent<MeshRenderer>().material;
                        var materialColor = material.color;
                        material.color = new Color(materialColor.r, materialColor.g, materialColor.b, 1f);
                        break;
                    case ObjectType.Car:
                        Destroy(other.gameObject);
                        // var transform1 = transform;
                        // other.transform.position = transform1.position - transform1.right;
                        break;
                }
                lightObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
