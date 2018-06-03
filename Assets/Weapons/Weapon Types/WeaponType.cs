using UnityEngine;

namespace Guildmaster.Equipment
{
    [CreateAssetMenu(menuName = ("Guildmaster/Weapon Type"))]
    public class WeaponType : ScriptableObject
    {
        public float range;
        public AnimatorOverrideController animatorOverrideController;
    }
}