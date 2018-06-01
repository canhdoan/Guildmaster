using System.Collections;
using UnityEngine;
using Guildmaster.Equipment;

namespace Guildmaster.Characters
{
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
        public float timeSinceLastAttack = 0f;

        // Coroutines
        Coroutine combatBehaviourRoutine;

        // Components
        Character character;
        #endregion

        #region Common Methods
        void Start()
        {
            combatBehaviourRoutine = StartCoroutine("CombatBehaviour");
        }

        void Awake()
        {
            character = GetComponent<Character>();
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
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
            print(gameObject + " attacks " + target);
            timeSinceLastAttack = 0;
        }
        #endregion
    }
}