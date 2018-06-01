using UnityEngine;
using Guildmaster.Characters;

namespace Guildmaster.Core
{
    public class StartupManager : MonoBehaviour
    {
        void Start()
        {
            // TODO: move to a sensible place later
            MapInitialization();
        }

        public void MapInitialization()
        {
            print("Initialization");

            // Initialize Player Characters
            PlayerCharacter[] playerCharacters = FindObjectsOfType<PlayerCharacter>();
            foreach (PlayerCharacter playerCharacter in playerCharacters)
            { playerCharacter.Initialize(); }
        }
    }
}