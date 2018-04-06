using UnityEngine;
using UnityEngine.AI;


namespace RPG.Characters
{
    [SelectionBase]
    public class Character : MonoBehaviour
    {

        private const float MOVE_NORMALIZE_THRESHOLD = 1f;

        [Header("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Avatar characterAvatar;
        [SerializeField] [Range(0.1f, 1f)] float animatorForwardCap = 1.0f;

        [Header("Audio")]
        [SerializeField] float audioSourceSpatialBlend = 0.5f;

        [Header("Capsule Collider")]
        [SerializeField] Vector3 colliderCenter = new Vector3(0, 0.5f, 0);
        [SerializeField] float colliderRadius = 0.5f;
        [SerializeField] float colliderHeight = 1.8f;
        [SerializeField] PhysicMaterial physicMaterial;

        [Header("Nav Mesh Agent")]
        [SerializeField] float navMeshAgentBaseOffset = -0.2f;
        [SerializeField] float navMeshAgentSteeringSpeed = 6f;
        [SerializeField] float navMeshAgentAcceleration = 4f;
        [SerializeField] float navMeshAgentStoppingDistance = 1.5f;
        [SerializeField] bool navMeshAgentAutoBrake = false;

        [Header("RigidBody")]
        [SerializeField] bool freezeRotation = true;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeedMultiplier = 1f;
        [SerializeField] float animationSpeedMultiplier = 1.2f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;

        private float turnAmount;
        private float forwardAmount;

        private Animator animator;
        private Rigidbody ridigBody;
        private NavMeshAgent navMeshAgent;

        bool isAlive = true;


        private void Awake()
        {
            AddRequiredComponents();
        }

        private void AddRequiredComponents()
        {
            // Animator setup
            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = characterAvatar;

            // AudioSource setup
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = audioSourceSpatialBlend;

            // CapsuleCollider setup
            CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>(); // no need to store in field
            capsuleCollider.center = colliderCenter;
            capsuleCollider.radius = colliderRadius;
            capsuleCollider.height = colliderHeight;
            capsuleCollider.material = physicMaterial;

            // NavMeshAgent setup
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.baseOffset = navMeshAgentBaseOffset;
            navMeshAgent.speed = navMeshAgentSteeringSpeed;
            navMeshAgent.acceleration = navMeshAgentAcceleration;
            navMeshAgent.stoppingDistance = navMeshAgentStoppingDistance;
            navMeshAgent.autoBraking = navMeshAgentAutoBrake;
            navMeshAgent.updatePosition = true;

            navMeshAgent.updateRotation = false;
            navMeshAgent.updatePosition = true;

            // Rigidbody setup
            ridigBody = gameObject.AddComponent<Rigidbody>();
            if (freezeRotation)
            {
                ridigBody.constraints = RigidbodyConstraints.FreezeRotation;
            }

        }

        private void Update()
        {
            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive)
            {
                Move(navMeshAgent.desiredVelocity);
            }
            else
            {
                Move(Vector3.zero);
            }
        }

        public void SetDestination(Vector3 worldPos)
        {
            navMeshAgent.destination = worldPos;
        }

        public void Kill()
        {
            isAlive = false;
        }

        private void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                velocity.y = ridigBody.velocity.y;
                ridigBody.velocity = velocity;
            }
        }

        private void ProcessDirectMovement()
        {
            // read inputs
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

            Move(movement);
        }

        private void Move(Vector3 move)
        {
            SetForwardAndTurn(move);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        private void SetForwardAndTurn(Vector3 move)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired direction.
            if (move.magnitude > MOVE_NORMALIZE_THRESHOLD)
            {
                move.Normalize();
            }

            Vector3 localMove = transform.InverseTransformDirection(move);
            localMove = Vector3.ProjectOnPlane(localMove, Vector3.up);

            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        void UpdateAnimator()
        {
            animator.SetFloat("Forward", forwardAmount * animatorForwardCap, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.speed = animationSpeedMultiplier;
        }

        public AnimatorOverrideController GetAnimatorOverrideController()
        {
            return animatorOverrideController;
        }

        public float GetAnimSpeedMultiplier()
        {
            return animationSpeedMultiplier; // this is our member, might also use animator.speed;
        }
    }
}