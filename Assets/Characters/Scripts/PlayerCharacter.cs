using UnityEngine;

namespace Guildmaster.Characters
{
    public class PlayerCharacter : MonoBehaviour
    {
        #region Variables & Components
        [Header("Identity")]
        public PlayableCharacter playableCharacter;

        [Header("Stats")]
        public float damageDone = 0f;
        public float damageReceived = 0f;

        // Elements
        GameObject movementTarget;
        Projector circleSelector;

        // Components
        Character character;
        CombatCharacter combatCharacter;
        #endregion

        #region Common Methods
        void Awake()
        {
            // Identify components
            character = GetComponent<Character>();
            combatCharacter = GetComponent<CombatCharacter>();

            // Identify the Elements
            circleSelector = transform.Find("Circle Selector").gameObject.GetComponent<Projector>();
        }

        public void Initialize()
        {
            movementTarget = new GameObject("Movement Target");

            // PlayerCharacter.cs
            damageDone = 0f;
            damageReceived = 0f;

            // CombatCharacter.cs
            combatCharacter.effectiveAttackRange = combatCharacter.weapon.range;

            // Set the Circle Selector colour
            //print(circleSelector.material.color + " - " + playableCharacter.color);
            circleSelector.material = playableCharacter.circleSelectorMaterial;

            // Update the character's appearance
            UpdateAppearance();
        }
        #endregion

        #region Movement
        public void MoveToDestination(Vector3 destination)
        {
            if (character.CheckPathValidity(destination))
            {
                // Move the Movement Target to the destination
                movementTarget.transform.position = destination;

                // Make the Movement target the character's destination
                character.movementDestination = movementTarget.transform;

                // Delete the character's current combat target
                combatCharacter.target = null;
            }
            else
            { print("can't move there"); }
        }
        #endregion

        #region Appearance
        public void UpdateAppearance()
        {
            // Identify the Mesh Renderer
            SkinnedMeshRenderer meshRenderer = gameObject.transform.Find("Primary Mesh").gameObject.GetComponent<SkinnedMeshRenderer>();

            // Apply the mesh
            meshRenderer.sharedMesh = playableCharacter.mesh;

            // Apply the material
            meshRenderer.material = playableCharacter.material;
        }
        #endregion

        #region Various
        public void ToggleCircleSelector(bool enabled)
        {
            circleSelector.enabled = enabled;
        }
        #endregion
    }
}