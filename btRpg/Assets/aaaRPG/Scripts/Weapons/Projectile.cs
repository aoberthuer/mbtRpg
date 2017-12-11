using UnityEngine;

using RPG.Core;

namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {

        [SerializeField] float projectileSpeed = 10f;
        [SerializeField] GameObject shooter; // so we can inspect who shot projectile
        float damageCaused = 10f;

        const float DESTROY_DELAY = 0.01f;

        public void setShooter(GameObject shooter)
        {
            this.shooter = shooter;
        }

        public void setDamage(float damage)
        {
            damageCaused = damage;
        }

        public float GetDefaultLaunchSpeed()
        {
            return projectileSpeed;
        }


        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject == null)
            {
                return;
            }

            if(shooter == null)
            {
                return;
            }

            if (collision.gameObject.layer != shooter.layer)
            {
                DamageDamageables(collision);
            }
        }

        private void DamageDamageables(Collision collision)
        {
            Component damageableComponent = collision.gameObject.GetComponent(typeof(IDamageable));
            if (damageableComponent != null)
            {
                (damageableComponent as IDamageable).TakeDamage(damageCaused);
            }

            Destroy(gameObject, DESTROY_DELAY);

        }
    }
}
