using UnityEngine;
using Guildmaster.Characters;

namespace Guildmaster.Core
{
    public class GameController : MonoBehaviour
    {
        #region Variables & Components
        [Header("Tactical Pause")]
        public bool tacticalPauseActive = false;
        public float tacticalPauseDuration = 0f;

        [Header("Game State")]
        public bool gamePaused = false;

        // Layers
        const int walkableLayerNumber = 8;
        const int playerCharacterLayerNumber = 9;
        const int hostileCharacterLayerNumber = 10;

        // Managers
        GroupManager groupManager;
        StartupManager startupManager;

        // Components
        CameraRaycaster cameraRaycaster;
        #endregion

        #region Common Methods
        void Awake()
        {
            // Identify the managers
            groupManager = GetComponent<GroupManager>();
            startupManager = GetComponent<StartupManager>();

            // Identifying the Camera Ray Caster, and subscribing to the delegates
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyMouseClickObservers += OnMouseLeftClick;
            cameraRaycaster.notifyMouseRightClickObservers += OnMouseRightClick;
        }

        void Update()
        {
            #region Keyboard Inputs
            if (!gamePaused)
            {
                // Space: Tactical Pause
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (!tacticalPauseActive)
                    { StartTacticalPause(); }
                    else
                    { StopTacticalPause(); }
                }
                // Escape: Pause & Main Menu
                else if (Input.GetKeyDown(KeyCode.Escape))
                { ShowMainMenu(); }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                { HideMainMenu(); }
            }
            #endregion

            // Calculate the time passed in Tactical Pause
            if (tacticalPauseActive && !gamePaused)
            { tacticalPauseDuration += Time.unscaledDeltaTime; }
        }
        #endregion

        #region Mouse clicks
        // Left click
        void OnMouseLeftClick(RaycastHit raycastHit, int layerHit)
        {
            print("left click");
        }

        // Right click
        void OnMouseRightClick(RaycastHit raycastHit, int layerHit)
        {
            if (!gamePaused)
            {
                if (layerHit == walkableLayerNumber)
                {
                    // Get each currently controlled character
                    foreach (GameObject characterObject in groupManager.controlledCharacters)
                    {
                        // Get the character's Player Character component
                        PlayerCharacter playerCharacter = characterObject.GetComponent<PlayerCharacter>();

                        // Make the characters move to the clicked destination
                        playerCharacter.MoveToDestination(raycastHit.point);
                    }
                }
                else if (layerHit == hostileCharacterLayerNumber)
                {
                    // Get each currently controlled character
                    foreach (GameObject characterObject in groupManager.controlledCharacters)
                    {
                        // Get the character's Combat Character component
                        CombatCharacter combatCharacter = characterObject.GetComponent<CombatCharacter>();

                        // Make the characters target the clicked enemy
                        combatCharacter.AssignTarget(raycastHit.collider.gameObject);
                    }
                }
            }
        }
        #endregion

        #region Tactical Pause
        // Start the Tactical Pause
        void StartTacticalPause()
        {
            tacticalPauseActive = true;
            Time.timeScale = 0;
        }

        // Stop the Tactical Pause
        void StopTacticalPause()
        {
            tacticalPauseActive = false;
            Time.timeScale = 1;
            tacticalPauseDuration = 0f;
        }
        #endregion

        #region Main Menu
        void ShowMainMenu()
        {
            gamePaused = true;
            Time.timeScale = 0;
        }

        void HideMainMenu()
        {
            gamePaused = false;
            Time.timeScale = 1;
        }
        #endregion
    }
}