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
        public bool isCtrlDown = false;

        // Layers
        const int walkableLayerNumber = 8;
        const int playerCharacterLayerNumber = 9;
        const int hostileCharacterLayerNumber = 10;

        // Managers
        CameraManager cameraManager;
        GroupManager groupManager;
        StartupManager startupManager;

        // Components
        CameraRaycaster cameraRaycaster;
        #endregion

        #region Common Methods
        void Awake()
        {
            // Identify the managers
            cameraManager = Camera.main.GetComponent<CameraManager>();
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
                // Escape: Pause & Main Menu
                if (Input.GetKeyDown(KeyCode.Escape))
                { ShowMainMenu(); }
                else
                {
                    // Space: Tactical Pause
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (!tacticalPauseActive)
                        { StartTacticalPause(); }
                        else
                        { StopTacticalPause(); }
                    }

                    // Ctrl: Multiple selection
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    { isCtrlDown = true; }
                    else
                    { isCtrlDown = false; }
                }
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
            if (!gamePaused)
            {
                if (layerHit == playerCharacterLayerNumber)
                {
                    // Check if the character is already part of the controlled characters list
                    if (groupManager.controlledCharacters.Contains(raycastHit.collider.gameObject))
                    {
                        // If the list contains only one controlled character (the clicked one), focus the camera on it
                        if (groupManager.controlledCharacters.Count == 1)
                        {
                            cameraManager.targetFollow = raycastHit.collider.gameObject.transform;
                        }
                        // If the list contains multiple characters, empty it to control only the clicked one
                        else
                        { groupManager.ControlCharacter(raycastHit.collider.gameObject, true); }
                    }
                    // The character is not already controlled: control it
                    else
                    {
                        // If Ctrl is pushed, add the clicked character to the controlled list
                        if (isCtrlDown)
                        { groupManager.ControlCharacter(raycastHit.collider.gameObject, false); }
                        // If Ctrl is not pushed, empty the controlled character list, and put the clicked character into it
                        else
                        { groupManager.ControlCharacter(raycastHit.collider.gameObject, true); }
                    }
                }
                // The player clicks on an "empty" area: wipes the controlled characters list
                else if (layerHit == walkableLayerNumber)
                {
                    groupManager.UnControlCharacters();
                }
            }
        }

        // Right click
        void OnMouseRightClick(RaycastHit raycastHit, int layerHit)
        {
            if (!gamePaused)
            {
                if (layerHit == walkableLayerNumber)
                {
                    // If characters are controlled
                    if (groupManager.controlledCharacters.Count > 0)
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
                }
                else if (layerHit == hostileCharacterLayerNumber)
                {
                    // Get each currently controlled character
                    foreach (GameObject characterObject in groupManager.controlledCharacters)
                    {
                        // Get the character's Combat Character component
                        CombatCharacter combatCharacter = characterObject.GetComponent<CombatCharacter>();

                        // If the enemy isn't dead, make it the characters target
                        if (!combatCharacter.isDead)
                        { combatCharacter.AssignTarget(raycastHit.collider.gameObject); }
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