using UnityEngine;

using RPG.Weapons;
using System.Collections;
using System;

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
        [SerializeField] float waypointDwellTime = 2.0f;
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

                bool inWeaponRange = distanceToPlayer <= currentWeaponRange;
                bool inChaseRange = distanceToPlayer > currentWeaponRange && distanceToPlayer <= chaseRadius;
                bool outsideChaseRange = distanceToPlayer > chaseRadius;

                if (outsideChaseRange && state != State.patrolling)
                {
                    StopAllCoroutines();
                    weaponSystem.StopAttacking();
                    StartCoroutine(Patrol());
                }
                if (inChaseRange && state != State.chasing)
                {
                    StopAllCoroutines();
                    weaponSystem.StopAttacking();
                    StartCoroutine(ChasePlayer());
                }
                if (inWeaponRange && state != State.attacking)
                {
                    StopAllCoroutines();

                    state = State.attacking;
                    weaponSystem.AttackTarget(player.gameObject); // AttackTarget() already starts a Coroutine in WeaponSystem, 
                                                                  // so no need to do this here...
                }
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
                yield return new WaitForSeconds(waypointDwellTime);
            }
        }

        private void CycleWaypointWhenClose(Vector3 nextWaypointPos)
        {
            if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
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
