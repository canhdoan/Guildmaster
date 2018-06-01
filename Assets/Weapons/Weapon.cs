using UnityEngine;

namespace Guildmaster.Equipment
{
    [CreateAssetMenu(menuName = ("Guildmaster/Weapon"))]
    public class Weapon : ScriptableObject
    {
        public WeaponType weaponType;
        public float attackSpeed = 2f;
    }
}