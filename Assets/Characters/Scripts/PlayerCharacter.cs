using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guildmaster.Characters
{
    public class PlayerCharacter : MonoBehaviour
    {
        #region Variables & Components
        GameObject movementTarget;

        // Components
        Character character;
        #endregion

        #region Common Methods
        void Awake()
        {
            // Identify components
            character = GetComponent<Character>();
        }

        public void Initialize()
        {
            movementTarget = new GameObject("Movement Target");
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
            }
            else
            { print("can't move there"); }
        }
        #endregion
    }
}