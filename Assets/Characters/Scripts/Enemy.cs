using UnityEngine;
using Guildmaster.Equipment;

namespace Guildmaster.Characters
{
    [CreateAssetMenu(menuName = ("Guildmaster/Enemy"))]
    public class Enemy : ScriptableObject
    {
        [Header("Attributes")]
        public float detectionRange = 10f;
        public float dps = 10f;
        public float attackSpeed = 2f;
        public float attackRange = 3f;

        [Header("Weapon")]
        public WeaponSkin weaponSkin;
        public AnimatorOverrideController animator;

        [Header("Appearance")]
        public Mesh primaryMesh;
        public Material primaryMaterial;
        public Mesh secondaryMesh;
        public Material secondaryMaterial;
        public float scale = 1f;
        public float hitboxCenter = 1f;
        public float hitboxRadius = 0.5f;
        public float hitboxHeight = 1.85f;
    }
}