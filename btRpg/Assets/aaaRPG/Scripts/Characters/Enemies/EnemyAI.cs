using UnityEngine;

using RPG.Weapons;
using System.Collections;

namespace RPG.Characters
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof (WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        
        [SerializeField] float chaseRadius = 4f;

        private Character character;
        private PlayerMovement player;

        private float currentWeaponRange = 2f;
        private float distanceToPlayer;

        enum State { idle, patrolling, attacking, chasing } // TODO consider extracting State enum
        State state = State.idle;

        private void Start()
        {
            character = GetComponent<Character>();
            player = FindObjectOfType<PlayerMovement>();
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
                    state = State.patrolling;
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
