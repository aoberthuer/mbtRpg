using UnityEngine;

using RPG.Weapons;

namespace RPG.Characters
{
    [RequireComponent(typeof (WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        
        [SerializeField] float chaseRadius = 4f;

        PlayerMovement playerMovement = null;
        float currentWeaponRange = 2f;
        bool isAttacking = false;

        private void Start()
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
        }

        private void Update()
        {
            if (playerMovement != null)
            {
                float distanceToPlayer = Vector3.Distance(playerMovement.transform.position, transform.position);

                WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
                currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
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
