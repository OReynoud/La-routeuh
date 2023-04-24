using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(menuName = "Light Type")]
    public class LightColorType : ScriptableObject
    {
        [Tooltip("Color")] [SerializeField] internal Color color;
        [Tooltip("Can the light kill shadows?")] [SerializeField] internal bool canKillShadows;
        [Tooltip("Can the light kill the player?")] [SerializeField] internal bool canKillPlayer;
        [Tooltip("Can the light reveal objects?")] [SerializeField] internal bool canRevealObjects;
        [Tooltip("Can the light hide objects?")] [SerializeField] internal bool canHideObjects;
    }
}