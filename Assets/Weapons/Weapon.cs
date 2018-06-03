using UnityEngine;

namespace Guildmaster.Equipment
{
    [CreateAssetMenu(menuName = ("Guildmaster/Weapon"))]
    public class Weapon : ScriptableObject
    {
        public WeaponType weaponType;
        public float attackSpeed = 2f;
        public float damagePerSecond = 10f;

        [Header("Components")]
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