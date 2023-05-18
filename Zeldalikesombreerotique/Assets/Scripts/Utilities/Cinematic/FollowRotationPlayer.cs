using Player;
using UnityEngine;

namespace Utilities.Cinematic
{
    public class FollowRotationPlayer : MonoBehaviour
    {
        private void Update()
        {
            transform.LookAt(PlayerController.instance.transform);
        }
    }
}
