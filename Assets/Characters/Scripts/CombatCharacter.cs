using System.Collections;
using UnityEngine;
using Guildmaster.Equipment;
using UnityEngine.AI;

namespace Guildmaster.Characters
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Character))]
    public class CombatCharacter : MonoBehaviour
    {
        #region Variables & Components
        [Header("Target")]
        public GameObject target;
        public float distanceToTarget = Mathf.Infinity;

        [Header("Equipment")]
        public Weapon weapon;
        GameObject rightHandWeaponObject;
        GameObject leftHandWeaponObject;

        [Header("Attributes")]
        public float maxHealth = 100f;
        public float currentHealth = 100f;

        [Header("Status")]
        public bool isDead = false;
        public bool isInCombat = false;
        public bool hasWeaponsOut = false;
        public float timeSinceLastAttack = 0f;

        // Combat attributes
        public float effectiveAttackRange = 3f;

        // Various
        public bool isPlayerCharacter = false;
        public bool isHostileCharacter = false;

        // Components
        Animator animator;
        CapsuleCollider capsuleCollider;
        Character character;
        HostileCharacter hostileCharacter;
        NavMeshAgent navMeshAgent;
        PlayerCharacter playerCharacter;
        #endregion

        #region Common Methods
        void Awake()
        {
            animator = GetComponent<Animator>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            character = GetComponent<Character>();
            hostileCharacter = GetComponent<HostileCharacter>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            playerCharacter = GetComponent<PlayerCharacter>();
        }

        void Start()
        {
            // Determine the type of character
            if (GetComponent<PlayerCharacter>() != null)
            { isPlayerCharacter = true; }
            else if (GetComponent<HostileCharacter>() != null)
            { isHostileCharacter = true; }

            Initialize();

            UnSheatheWeapons(true);
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        void Initialize()
        {
            StartCoroutine("CombatBehaviour");

            // If the weapon has an Animator Override Controller, apply it
            if (isPlayerCharacter && weapon.animator != null)
            { animator.runtimeAnimatorController = weapon.animator; }
            else if (isHostileCharacter && hostileCharacter.enemy.animator != null)
            { animator.runtimeAnimatorController = hostileCharacter.enemy.animator; }
        }
        #endregion

        #region Targeting
        // Function assigning a target to the combat character (used for Player Characters)
        public void AssignTarget(GameObject assignedTarget)
        {
            target = assignedTarget;
        }
        #endregion

        #region Combat Behaviour
        IEnumerator CombatBehaviour()
        {
            for (; ; )
            {
                // Determine if the combat behaviour should be ran
                bool shouldRunCombatBehaviour = true;

                if (isDead) // The character is dead
                { shouldRunCombatBehaviour = false; }
                else if (!target) // The character has no target
                {
                    shouldRunCombatBehaviour = false;
                    distanceToTarget = Mathf.Infinity;
                }

                // Running the combat behaviour
                if (shouldRunCombatBehaviour)
                {
                    // Get the target Combat Character component
                    CombatCharacter targetCombatCharacter = target.GetComponent<CombatCharacter>();

                    // Check that the target is not dead (or else, wipe it as a target)
                    if (!targetCombatCharacter.isDead)
                    {
                        bool shouldPursue = false;

                        // Actualize the distance to the target
                        distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

                        // If the character is close enough to its target, see if it should attack it
                        if (distanceToTarget <= effectiveAttackRange)
                        {
                            // If the Character's movement target was defined as the current target, wipe it (the character doesn't need to move towards it anymore)
                            if (character.movementDestination == target.transform)
                            { character.movementDestination = null; }
                            // TODO: delete 2 previous lines?
                            character.movementDestination = null;

                            // If the last attack was made enough time ago, attack the target
                            if (timeSinceLastAttack >= weapon.attackSpeed)
                            {
                                AutoAttackTarget();
                            }
                        }
                        else
                        { shouldPursue = true; }

                        if (shouldPursue)
                        {
                            // Determine the stopping distance (characters stop nearer from their target than necessary, to make sure they don't block other enemies from attacking)
                            float stoppingDistance = effectiveAttackRange - 1f;

                            // Make the character move to its target
                            navMeshAgent.stoppingDistance = stoppingDistance;
                            character.movementDestination = target.transform;
                        }
                    }
                    else
                    {
                        // The target is dead, wipe it
                        target = null;
                    }
                }

                yield return new WaitForSeconds(0.2f);
            }
        }
        #endregion

        #region Attacking
        // Function triggering an auto attack toward the current target
        void AutoAttackTarget()
        {
            // If the character has not brought its weapons out yet, do it
            if (!hasWeaponsOut)
            { UnSheatheWeapons(true); }

            // Make the character look at its target
            character.LookAt(target, 5f);

            timeSinceLastAttack = 0;
            animator.SetTrigger("Attack 1");
        }

        // Function triggered by the attack animation, and doing damage
        public void Hit()
        {
            // Damage the target
            float damage = target.GetComponent<CombatCharacter>().ReceiveDamage(gameObject, CalculateDPS());

            // Increase the Player Character damage done stat
            if (isPlayerCharacter)
            { playerCharacter.damageDone += damage; }
        }

        // Function calculating the character's effective DPS (without random effects such as critical)
        public float CalculateDPS()
        {
            float dps = 0;

            if (isPlayerCharacter)
            {
                // Calculate the basic damage done by the weapon's hit
                float weaponDamage = (weapon.damagePerSecond * weapon.attackSpeed) / weapon.attacksPerCycle;

                dps = weaponDamage;
            }
            else if (isHostileCharacter)
            {
                // DPS is determined by the enemy archetype
                dps = hostileCharacter.enemy.dps * hostileCharacter.enemy.attackSpeed;
            }
                
            return dps;
        }
        #endregion

        #region Damage
        public float ReceiveDamage(GameObject damageDealer, float amount)
        {
            currentHealth -= amount;

            // If the character has no target yet, automatically target the source of the damage
            if (!target)
            { target = damageDealer; }

            // If health is equal to 0, call the Death function
            if (currentHealth <= 0)
            { Death(); }
            else
            {
                if (isHostileCharacter)
                {
                    // Alter the threat table
                    hostileCharacter.IncreaseThreat(damageDealer, amount);
                }
                else if (isPlayerCharacter)
                {
                    // Increase the Player Character damage received stat
                    playerCharacter.damageReceived += amount;
                }
            }

            return amount;
        }

        public void Death()
        {
            // Toggle the Is Dead boolean
            isDead = true;

            // Toggle the death animation
            animator.SetTrigger("Death");

            // Disable the capsule collider
            capsuleCollider.enabled = false;

            // Empty the fields that could cause strange behaviour from the body
            target = null;
            character.movementDestination = null;
        }
        #endregion

        #region Weapons
        public void UnSheatheWeapons(bool unsheathe)
        {
            // Create variables
            Transform rightHandParentTransform = null;
            Transform rightHandTransform = null;
            Transform leftHandParentTransform = null;
            Transform leftHandTransform = null;

            // Destroy the current weapon objects
            if (rightHandWeaponObject) { Destroy(rightHandWeaponObject); }
            if (leftHandWeaponObject) { Destroy(leftHandWeaponObject); }

            // Get the weapon skin informations
            WeaponSkin weaponSkin = null;
            if (isPlayerCharacter)
            { weaponSkin = weapon.defaultSkin; }
            else if (isHostileCharacter)
            { weaponSkin = hostileCharacter.enemy.weaponSkin; }

            // Identify which part of the character should be used as a parent
            if (unsheathe)
            {
                rightHandParentTransform = GetComponentsInChildren<RightHand>()[0].gameObject.transform;
                rightHandTransform = weaponSkin.rightHandWieldTransform;
                leftHandParentTransform = GetComponentsInChildren<LeftHand>()[0].gameObject.transform;
                leftHandTransform = weaponSkin.leftHandWieldTransform;
            }
            else
            {
                if (weaponSkin.rightHandCarryLocation == "spine")
                { rightHandParentTransform = GetComponentsInChildren<Spine>()[0].gameObject.transform; }
                else if (weaponSkin.rightHandCarryLocation == "waist")
                { rightHandParentTransform = GetComponentsInChildren<Waist>()[0].gameObject.transform; }
                rightHandTransform = weaponSkin.rightHandCarryTransform;

                if (weaponSkin.leftHandCarryLocation == "spine")
                { leftHandParentTransform = GetComponentsInChildren<Spine>()[0].gameObject.transform; }
                else if (weaponSkin.leftHandCarryLocation == "waist")
                { leftHandParentTransform = GetComponentsInChildren<Waist>()[0].gameObject.transform; }
                leftHandTransform = weaponSkin.leftHandCarryTransform;
            }

            if (weaponSkin.rightHandPrefab != null)
            {
                // Create the weapon
                rightHandWeaponObject = Instantiate(weaponSkin.rightHandPrefab, rightHandParentTransform);

                // Apply the Transforms
                rightHandWeaponObject.transform.localPosition = rightHandTransform.localPosition;
                rightHandWeaponObject.transform.localRotation = rightHandTransform.localRotation;
                rightHandWeaponObject.transform.localScale = rightHandTransform.localScale;
            }

            if (weaponSkin.leftHandPrefab != null)
            {
                // Create the weapon
                leftHandWeaponObject = Instantiate(weaponSkin.leftHandPrefab, leftHandParentTransform);

                // Apply the Transforms
                leftHandWeaponObject.transform.localPosition = leftHandTransform.localPosition;
                leftHandWeaponObject.transform.localRotation = leftHandTransform.localRotation;
                leftHandWeaponObject.transform.localScale = leftHandTransform.localScale;
            }

            // Set the variables and animator parameters
            if (unsheathe)
            {
                hasWeaponsOut = true;
                animator.SetBool("Weapons Out", true);
            }
            else
            {
                hasWeaponsOut = false;
                animator.SetBool("Weapons Out", false);
            }
        }
        #endregion
    }
}