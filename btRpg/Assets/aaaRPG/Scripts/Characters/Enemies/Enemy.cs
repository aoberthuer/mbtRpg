using UnityEngine;

using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour, IDamageable
    {

        [SerializeField] float maxHealthPoints = 100f;
        float currentHealthPoints;

        Player player = null;

        [SerializeField] float attackRadius = 2f;
        [SerializeField] float chaseRadius = 4f;
        [SerializeField] float damagePerShot = 5f;

        [SerializeField] float secondsBetweenShots = 0.5f;
        [SerializeField] float shotsVariationPercentage = 0.2f;

        bool isAttacking = false;

        [SerializeField] GameObject projectileToUse = null;
        [SerializeField] GameObject projectileSocket = null;
        [SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);

        private void Start()
        {
            player = FindObjectOfType<Player>();

            currentHealthPoints = maxHealthPoints;
        }

        private void Update()
        {
            if (player != null)
            {
                if(player.healthAsPercentage <= float.Epsilon)
                {
                    StopAllCoroutines();
                    Destroy(this); // stop all enemy behaviour
                }

                float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
                if (distanceToPlayer <= attackRadius && !isAttacking)
                {
                    isAttacking = true;

                    float absoluteVariation = secondsBetweenShots * shotsVariationPercentage;
                    InvokeRepeating("SpawnProjectile", 0f, Random.Range(secondsBetweenShots - absoluteVariation, secondsBetweenShots + absoluteVariation));
                }

                if (distanceToPlayer > attackRadius)
                {
                    CancelInvoke();
                    isAttacking = false;
                }


                if (distanceToPlayer <= chaseRadius)
                {
                    // aiCharacterControl.SetTarget(player.transform);
                }
                else
                {
                    // aiCharacterControl.SetTarget(transform);
                }
            }
        }

        void SpawnProjectile()
        {
            GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
            Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
            projectileComponent.setDamage(damagePerShot);
            projectileComponent.setShooter(gameObject);

            Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
            float projectileSpeed = projectileComponent.GetDefaultLaunchSpeed();
            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        }

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            if (currentHealthPoints <= 0)
            {
                Destroy(gameObject);
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
            Gizmos.DrawWireSphere(transform.position, attackRadius);

            // draw chase sphere
            Gizmos.color = new Color(0f, 0f, 255f, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}
