using UnityEngine;

public class LockAliceRotation : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Quaternion.Euler(80, 0, 0);
    }
}
