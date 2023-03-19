using Player;
using UnityEngine;

namespace Utilities
{
    public class BatteryDetection : MonoBehaviour
    {
        [SerializeField] private GameObject lightObject;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Battery") && PlayerController.instance.isGrabbing)
            {
                PlayerController.instance.isGrabbing = false;
                Destroy(other.gameObject);
                lightObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
