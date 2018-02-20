using UnityEngine;

using RPG.Weapons;
using System.Collections;

namespace RPG.Characters
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof (WeaponSystem))]
    public class EnemyControlAI : MonoBehaviour
    {
        
        [SerializeField] float chaseRadius = 4f;
        [SerializeField] WaypointContainer patrolPath;
        [SerializeField] float waypointTolerance = 2.0f;
        int nextWaypointIndex;

        private Character character;
        private PlayerControl player;

        private float currentWeaponRange = 2f;
        private float distanceToPlayer;

        enum State { idle, patrolling, attacking, chasing } // TODO consider extracting State enum
        State state = State.idle;

        private void Start()
        {
            character = GetComponent<Character>();
            player = FindObjectOfType<PlayerControl>();
        }

        private void Update()
        {
            if (player != null)
            {
                distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

                WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
                currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

                if (distanceToPlayer > chaseRadius && state != State.patrolling)
                {
                    StopAllCoroutines();
                    StartCoroutine(Patrol());
                }
                if (distanceToPlayer <= chaseRadius && state != State.chasing)
                {
                    StopAllCoroutines();
                    StartCoroutine(ChasePlayer());
                }
                if (distanceToPlayer <= currentWeaponRange && state != State.attacking)
                {
                    StopAllCoroutines();
                    state = State.attacking;
                }
            }
        }

        private IEnumerator ChasePlayer()
        {
            state = State.chasing;
            while (distanceToPlayer >= currentWeaponRange)
            {
                character.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator Patrol()
        {
            state = State.patrolling;

            while (patrolPath != null)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWaypointPos);
                CycleWaypointWhenClose(nextWaypointPos);
                yield return new WaitForSeconds(0.5f); // todo parameterise
            }
        }

        private void CycleWaypointWhenClose(Vector3 nextWaypointPos)
        {
            if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        public string GetTag()
        {
            return tag;
        }

        private void OnDrawGizmos()
        {
            // draw attack sphere
            Gizmos.color = new Color(255f, 0f, 0f, .5f);
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

            // draw chase sphere
            Gizmos.color = new Color(0f, 0f, 255f, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}
