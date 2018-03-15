using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        
        public override void Use(GameObject target, bool useTargetPosition)
        {
            base.Use(target, useTargetPosition);
            DealDamage(target);
        }

        private void DealDamage(GameObject target)
        {
            float damageToDeal = ((PowerAttackConfig)config).GetExtraDamage();
            target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
        }
    }
}
