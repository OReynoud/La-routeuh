using Player;
using UnityEngine;

namespace Utilities
{
    public class BatteryDetection : MonoBehaviour
    {
        [Tooltip("Light game object")] [SerializeField] private GameObject lightObject;
        [Tooltip("Mesh of the light")] [SerializeField] private GameObject meshObject;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Battery") && PlayerController.instance.isGrabbing)
            {
                PlayerController.instance.isGrabbing = false;
                Destroy(other.gameObject);
                var material = meshObject.GetComponent<MeshRenderer>().material;
                var materialColor = material.color;
                material.color = new Color(materialColor.r, materialColor.g, materialColor.b, 1f);
                lightObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
