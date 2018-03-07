using UnityEngine;

using RPG.Characters;

namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {
        public float damageCaused = 5;

        void OnTriggerEnter(Collider collider)
        {
            HealthSystem healthSystem = collider.gameObject.GetComponent<HealthSystem>();
            if (healthSystem)
            {
                healthSystem.TakeDamage(damageCaused);
            }

            Destroy(gameObject, 0.1f);
        }
    }
}
