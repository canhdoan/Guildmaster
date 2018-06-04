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
        public bool isMoving = false;
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
                isMoving = true;

                if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                { Move(navMeshAgent.desiredVelocity); }
            }

            // If the character is near its destination, or has no longer a movement target, make it stop and erase the movement destination
            if (!movementDestination || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                Move(Vector3.zero);
                navMeshAgent.velocity = Vector3.zero;
                isMoving = false;
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

        public void FootL()
        { }

        public void FootR()
        { }
        #endregion

        #region Line of Sight
        // Function checking that the target is potentially visible by the character (360° field of view)
        public bool IsVisible(GameObject target)
        {
            // Calculating Character and Target's positions
            Vector3 characterPosition = transform.position;
            Vector3 targetPosition = target.transform.position;

            // Adjusting characters positions (to check line of sight from the head, to the torso)
            characterPosition.y += 1.5f;
            targetPosition.y += 0.8f;

            // Check that the target is visible
            RaycastHit hit;

            if (Physics.Linecast(characterPosition, targetPosition, out hit))
            {
                if (hit.collider == target.GetComponent<Collider>())
                {
                    Debug.DrawLine(characterPosition, targetPosition, Color.green);
                    return true;
                }
                else
                {
                    Debug.DrawLine(characterPosition, targetPosition, Color.red);
                    return false;
                }
            }
            else
            { return false; }
        }
        #endregion

        #region Various
        // Function making the character look at a specific game object. The "max difference" parameter allows to specify an angle in which the character isn't reoriented
        public void LookAt(GameObject target, float maxDifference)
        {
            // Get the direction the character should have
            Vector3 targetDirection = target.transform.position - transform.position;

            // Get the angle difference between the current direction, and the one the character should have
            float targetDirectionAngle = Vector3.Angle(targetDirection, transform.forward);

            // If the difference is superior to the specified max difference, make the character look at the target
            if (targetDirectionAngle > maxDifference)
            {
                gameObject.transform.rotation = Quaternion.LookRotation(targetDirection);
            }
        }
        #endregion
    }
}