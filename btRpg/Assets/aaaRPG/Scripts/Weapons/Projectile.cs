using UnityEngine;

using RPG.Characters;

namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {
        private float damageCaused = 5;
        private GameObject projectileTarget;

        public void SetDamageCaused(float damageCaused)
        {
            this.damageCaused = damageCaused;
        }

        public void SetProjectileTarget(GameObject projectileTarget)
        {
            this.projectileTarget = projectileTarget;
        }

        void OnTriggerEnter(Collider collider)
        {
            if (!collider.gameObject.Equals(projectileTarget))
                return;

            HealthSystem healthSystem = collider.gameObject.GetComponent<HealthSystem>();
            if (healthSystem)
            {
                healthSystem.TakeDamage(damageCaused);
            }

            Destroy(gameObject, 0.1f);
        }
    }
}
