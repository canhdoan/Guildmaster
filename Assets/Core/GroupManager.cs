using System.Collections.Generic;
using UnityEngine;
using Guildmaster.Characters;

namespace Guildmaster.Core
{
    public class GroupManager : MonoBehaviour
    {
        #region Variables & Components
        [Header("Characters")]
        public GameObject character1;
        public GameObject character2;
        public GameObject character3;
        public GameObject character4;

        public List<GameObject> controlledCharacters = new List<GameObject>();
        #endregion

        void Start()
        {
            controlledCharacters.Add(character1);
        }

        public void ControlCharacter(GameObject character, bool emptyList)
        {
            if (emptyList)
            { controlledCharacters.Clear(); }

            // Identify the character
            GameObject characterToControl = null;
            if (character == character1)
            { characterToControl = character1; }
            else if (character == character2)
            { characterToControl = character2; }
            else if (character == character3)
            { characterToControl = character3; }
            else if (character == character4)
            { characterToControl = character4; }

            // If the character exists, and is not yet in the list, put it into it
            if (characterToControl != null)
            {
                if (!controlledCharacters.Contains(characterToControl))
                { controlledCharacters.Add(characterToControl); }
            }

            // Toggle the Circle Selectors on controlled characters
            ToggleCircleSelectors();
        }

        // Function enabling or disabling the Circle Selectors of characters, depending if they're controlled or not
        public void ToggleCircleSelectors()
        {
            if (controlledCharacters.Contains(character1))
            { character1.GetComponent<PlayerCharacter>().ToggleCircleSelector(true); }
            else
            { character1.GetComponent<PlayerCharacter>().ToggleCircleSelector(false); }

            if (controlledCharacters.Contains(character2))
            { character2.GetComponent<PlayerCharacter>().ToggleCircleSelector(true); }
            else
            { character2.GetComponent<PlayerCharacter>().ToggleCircleSelector(false); }

            if (controlledCharacters.Contains(character3))
            { character3.GetComponent<PlayerCharacter>().ToggleCircleSelector(true); }
            else
            { character3.GetComponent<PlayerCharacter>().ToggleCircleSelector(false); }

            if (controlledCharacters.Contains(character4))
            { character4.GetComponent<PlayerCharacter>().ToggleCircleSelector(true); }
            else
            { character4.GetComponent<PlayerCharacter>().ToggleCircleSelector(false); }
        }
    }
}