using UnityEngine;
using UnityEngine.AI;

namespace Guildmaster.Characters
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class Character : MonoBehaviour
    {
        #region Variables & Components
        [Header("Movement")]
        public Transform movementDestination;
        float currentMovementSpeed;

        // Components
        Animator animator;
        NavMeshAgent navMeshAgent;
        #endregion

        #region Common Methods
        void Awake()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            navMeshAgent.updateRotation = false;
            navMeshAgent.updatePosition = true;
        }

        void Update()
        {
            // If the character has a movement target, tell the Nav Mesh Agent to move to it
            if (movementDestination)
            {
                navMeshAgent.SetDestination(movementDestination.position);

                if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                { Move(navMeshAgent.desiredVelocity); }
            }

            // If the character is near its destination, or has no longer a movement target, make it stop
            if (!movementDestination || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                Move(Vector3.zero);
                navMeshAgent.velocity = Vector3.zero;
            }
        }
        #endregion

        #region Movement
        // TODO: clean-up (harvested from Third Person Character)
        public void Move(Vector3 move)
        {
            // Convert the world relative moveInput vector into a local-relative turn amount and forward amount required to head in the desired direction.
            //if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);
            RaycastHit hitInfo;
            Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, 0.5f);
            move = Vector3.ProjectOnPlane(move, hitInfo.normal);
            currentMovementSpeed = move.z;

            // Help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(1000, 1000, currentMovementSpeed);
            transform.Rotate(0, Mathf.Atan2(move.x, move.z) * turnSpeed * Time.deltaTime, 0);

            // Update the animator parameters
            animator.SetFloat("Speed", currentMovementSpeed, 0.1f, Time.deltaTime);
        }

        // Function checking that the character can move to the designated target
        public bool CheckPathValidity(Vector3 movementTarget)
        {
            NavMeshPath movementPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, movementTarget, NavMesh.AllAreas, movementPath);

            if (movementPath.status == NavMeshPathStatus.PathComplete)
            { return true; }
            else
            { return false; }
        }
        #endregion
    }
}