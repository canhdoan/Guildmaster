using UnityEngine;

namespace Guildmaster.Equipment
{
    [CreateAssetMenu(menuName = ("Guildmaster/Weapon"))]
    public class Weapon : ScriptableObject
    {
        [Header("Attributes")]
        public float damagePerSecond = 10f;
        public float attackSpeed = 2f;
        public int attacksPerCycle = 1;
        public float range = 3f;
        public AnimatorOverrideController animator;

        [Header("Skins")]
        public WeaponSkin defaultSkin;
    }
}