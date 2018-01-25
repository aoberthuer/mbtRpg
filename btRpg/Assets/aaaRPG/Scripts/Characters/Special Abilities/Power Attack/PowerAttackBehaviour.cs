using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        
        public override void Use(GameObject target)
        {
            base.Use(target);
            DealDamage(target);
        }

        private void DealDamage(GameObject target)
        {
            float damageToDeal = ((PowerAttackConfig)config).GetExtraDamage();
            target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
        }
    }
}
