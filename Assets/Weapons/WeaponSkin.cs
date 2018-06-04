using UnityEngine;

namespace Guildmaster.Equipment
{
    [CreateAssetMenu(menuName = ("Guildmaster/Weapon Skin"))]
    public class WeaponSkin : ScriptableObject
    {
        public string skinId;
        public GameObject rightHandPrefab;
        public string rightHandCarryLocation = "spine";
        public Transform rightHandCarryTransform;
        public Transform rightHandWieldTransform;
        public GameObject leftHandPrefab;
        public string leftHandCarryLocation = "spine";
        public Transform leftHandCarryTransform;
        public Transform leftHandWieldTransform;
    }
}