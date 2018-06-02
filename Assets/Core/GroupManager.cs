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
    }
}