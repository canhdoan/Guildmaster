using UnityEngine;

namespace Guildmaster.Characters
{
    [CreateAssetMenu(menuName = ("Guildmaster/Playable Character"))]
    public class PlayableCharacter : ScriptableObject
    {
        [Header("Informations")]
        public string firstName;

        [Header("Appearance")]
        public Mesh mesh;
        public Material material;
        public Material circleSelectorMaterial;
    }
}