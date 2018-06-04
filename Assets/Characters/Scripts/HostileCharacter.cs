using System.Collections;
using UnityEngine;

namespace Guildmaster.Characters
{
    public class HostileCharacter : MonoBehaviour
    {
        #region Variables & Components
        public Enemy enemy;

        [Header("State")]
        public bool isAlerted = false;

        [Header("Aggro")]
        public GameObject target1 = null;
        public float target1Threat = 0f;
        public GameObject target2 = null;
        public float target2Threat = 0f;
        public GameObject target3 = null;
        public float target3Threat = 0f;
        public GameObject target4 = null;
        public float target4Threat = 0f;
        public float timeSinceTargetChange = 0f;

        // Components
        Character character;
        CombatCharacter combatCharacter;
        #endregion

        #region Common Methods
        void Awake()
        {
            character = GetComponent<Character>();
            combatCharacter = GetComponent<CombatCharacter>();
        }

        void Start()
        {
            Initialize();
        }

        void Update()
        {
            timeSinceTargetChange += Time.deltaTime;
        }

        public void Initialize()
        {
            // CombatCharacter.cs
            combatCharacter.effectiveAttackRange = enemy.attackRange;

            // Starting the Behaviour routine
            StartCoroutine("Behaviour");

            UpdateAppearance();
        }
        #endregion

        // Function looking for potential targets while not in combat
        IEnumerator Behaviour()
        {
            for (; ; )
            {
                if (!isAlerted)
                { CheckForPotentialTargets(); }
                else
                {
                    // If the last target change was more than a second ago, check the aggro table to choose a target
                    if (timeSinceTargetChange >= 1f)
                    { ChooseTargetByThreat(); }
                }

                yield return new WaitForSeconds(0.2f);
            }
        }

        #region Waiting
        // Function checking the surroundings for potential targets
        void CheckForPotentialTargets()
        {
            // Create variables
            bool targetFound = false;
            GameObject nearestTarget = null;
            float distanceToNearestTarget = Mathf.Infinity;

            // Find all potential targets (objects with the "PlayerFaction" tag)
            GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("PlayerFaction");

            // Analyze each potential target
            foreach (GameObject targetCharacter in potentialTargets)
            {
                // Get the target's Combat Character component
                CombatCharacter targetCombatCharacter = targetCharacter.GetComponent<CombatCharacter>();

                // Check if the target is a viable one
                bool isAViableTarget = true;
                if (targetCombatCharacter == null)
                {
                    // The target is not a combat character
                    isAViableTarget = false;
                }
                else if (targetCombatCharacter.isDead)
                {
                    // The target is dead
                    isAViableTarget = false;
                }

                if (isAViableTarget)
                {
                    // Calculate the distance to the target
                    float distanceToTargetCharacter = Vector3.Distance(targetCharacter.transform.position, transform.position);

                    // Check that the target is under the detection range (otherwise, ignore it completely)
                    if (distanceToTargetCharacter <= enemy.detectionRange)
                    {
                        // Check that the target is visible (otherwise, ignore it completely)
                        if (character.IsVisible(targetCharacter))
                        {
                            // Check that the target is the nearest one
                            if (distanceToTargetCharacter < distanceToNearestTarget)
                            {
                                targetFound = true;
                                nearestTarget = targetCharacter;
                                distanceToNearestTarget = distanceToTargetCharacter;
                            }
                        }
                    }
                }
            }

            // If a visible target is in the detection range, alert the character
            if (targetFound)
            {
                Alert(nearestTarget);
            }
        }

        // Function called when the character is alerted
        void Alert(GameObject target)
        {
            // Set the Is Alerted toggle
            isAlerted = true;

            // Build the Aggro Table
            BuildAggroTable(target);

            // Set the detected character as the combat target
            combatCharacter.target = target;
        }
        #endregion

        #region Threat
        // Function building the aggro table
        void BuildAggroTable(GameObject initialTarget)
        {
            int targetID = 1;

            // Find all potential targets (objects with the "PlayerFaction" tag)
            GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("PlayerFaction");

            // Analyze each potential target, and put it in the aggro table
            foreach (GameObject targetCharacter in potentialTargets)
            {
                switch (targetID)
                {
                    case 1:
                        target1 = targetCharacter;
                        if (targetCharacter == initialTarget) { target1Threat = 1; }
                        break;
                    case 2:
                        target2 = targetCharacter;
                        if (targetCharacter == initialTarget) { target2Threat = 1; }
                        break;
                    case 3:
                        target3 = targetCharacter;
                        if (targetCharacter == initialTarget) { target3Threat = 1; }
                        break;
                    case 4:
                        target4 = targetCharacter;
                        if (targetCharacter == initialTarget) { target4Threat = 1; }
                        break;
                }

                targetID++;
            }
        }

        // Function checking the aggro table to choose a target
        public void ChooseTargetByThreat()
        {
            // Create the variables
            GameObject highestThreatTarget = target1;
            float highestThreatTargetThreat = target1Threat;

            if (target2Threat > highestThreatTargetThreat)
            { highestThreatTarget = target2; }

            if (target3Threat > highestThreatTargetThreat)
            { highestThreatTarget = target3; }

            if (target4Threat > highestThreatTargetThreat)
            { highestThreatTarget = target4; }

            if (highestThreatTarget != combatCharacter.target)
            {
                combatCharacter.target = highestThreatTarget;
                timeSinceTargetChange = 0f;
            }
        }

        // Function increasing the threat value for a character
        public void IncreaseThreat(GameObject source, float value)
        {
            if (source == target1)
            { target1Threat += value; }
            else if (source == target2)
            { target2Threat += value; }
            else if (source == target3)
            { target3Threat += value; }
            else if (source == target4)
            { target4Threat += value; }
        }
        #endregion

        #region Appearance
        public void UpdateAppearance()
        {
            // Apply the Primary Mesh
            if (enemy.primaryMesh != null)
            {
                // Identify the Body Mesh Renderer
                GameObject primaryMeshObject = gameObject.transform.Find("Primary Mesh").gameObject;
                SkinnedMeshRenderer primaryMeshRenderer = primaryMeshObject.GetComponent<SkinnedMeshRenderer>();

                // Apply the mesh
                primaryMeshRenderer.sharedMesh = enemy.primaryMesh;

                // Apply the material(s)
                Material[] materials = primaryMeshObject.GetComponent<Renderer>().materials;
                materials[0] = enemy.primaryMaterial;
            }
            else
            { gameObject.transform.Find("Primary Mesh").gameObject.SetActive(false); }

            // Apply the Secondary Mesh
            if (enemy.secondaryMesh != null)
            {
                // Identify the Body Mesh Renderer
                GameObject secondaryMeshObject = gameObject.transform.Find("Secondary Mesh").gameObject;
                SkinnedMeshRenderer secondaryMeshRenderer = secondaryMeshObject.GetComponent<SkinnedMeshRenderer>();

                // Apply the mesh
                secondaryMeshRenderer.sharedMesh = enemy.secondaryMesh;

                // Apply the material
                Material[] materials = secondaryMeshObject.GetComponent<Renderer>().materials;
                materials[0] = enemy.secondaryMaterial;
            }
            else
            { gameObject.transform.Find("Secondary Mesh").gameObject.SetActive(false); }

            // Update the character's scale
            transform.localScale = new Vector3(enemy.scale, enemy.scale, enemy.scale);

            // Update the character's hitbox
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            capsuleCollider.center = new Vector3(0, enemy.hitboxCenter, 0);
            capsuleCollider.radius = enemy.hitboxRadius;
            capsuleCollider.height = enemy.hitboxHeight;
        }
        #endregion
    }
}