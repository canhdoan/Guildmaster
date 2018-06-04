using System.Collections.Generic;
using UnityEngine;

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
        }
    }
}