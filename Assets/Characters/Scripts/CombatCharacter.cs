using System.Collections;
using UnityEngine;
using Guildmaster.Equipment;

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

        [Header("Attributes")]
        public float maxHealth = 100f;
        public float currentHealth = 100f;

        [Header("Status")]
        public bool isDead = false;
        public bool isInCombat = false;
        public bool hasWeaponsOut = false;
        public float timeSinceLastAttack = 0f;

        // Coroutines
        Coroutine combatBehaviourRoutine;

        // Components
        Animator animator;
        Character character;
        #endregion

        #region Common Methods
        void Start()
        {
            combatBehaviourRoutine = StartCoroutine("CombatBehaviour");
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<Character>();
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
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
                { shouldRunCombatBehaviour = false; }

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
                        if (distanceToTarget <= weapon.weaponType.range)
                        {
                            // If the Character's movement target was defined as the current target, wipe it (the character doesn't need to move towards it anymore)
                            if (character.movementDestination == target.transform)
                            { character.movementDestination = null; }

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
            { UnSheatheWeapons(); }

            // Make the character look at its target
            character.LookAt(target, 5f);

            print(gameObject + " attacks " + target);
            timeSinceLastAttack = 0;
            animator.SetTrigger("Attack 1");
        }

        // Function triggered by the attack animation, and doing damage
        public void Hit()
        {
            // Damage the target
            target.GetComponent<CombatCharacter>().ReceiveDamage(CalculateDPS());
        }

        // Functionc alculating the character's DPS
        public float CalculateDPS()
        {
            // Calculate the basic damage done by the weapon's hit
            float weaponDamage = weapon.damagePerSecond * weapon.attackSpeed;

            return weaponDamage;
        }
        #endregion

        #region Damage
        public float ReceiveDamage(float amount)
        {
            currentHealth -= amount;

            return 1f;
        }
        #endregion

        #region Weapons
        public void UnSheatheWeapons()
        {
            hasWeaponsOut = true;
            animator.SetBool("Weapons Out", true);
        }

        public void SheatheWeapons()
        {
            hasWeaponsOut = false;
            animator.SetBool("Weapons Out", false);
        }
        #endregion
    }
}