using Player;
using UnityEngine;

namespace Utilities.Cinematic
{
    public class FollowRotationPlayer : MonoBehaviour
    {
        private void Update()
        {
            var direction = PlayerController.instance.transform.position - transform.position;
            direction = new Vector3(direction.x, 0f, direction.z);
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
