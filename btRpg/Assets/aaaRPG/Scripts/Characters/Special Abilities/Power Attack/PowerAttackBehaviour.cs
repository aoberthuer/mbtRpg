using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        
        public void SetConfig(PowerAttackConfig config)
        {
            this.config = config;
        }

        public override void Use(AbilityUseParameters abilityUseParameters)
        {
            base.Use(abilityUseParameters);
            DealDamage(abilityUseParameters);
        }

        private void DealDamage(AbilityUseParameters abilityUseParameters)
        {
            float damageToDeal = abilityUseParameters.baseDamage + ((PowerAttackConfig)config).GetExtraDamage();
            abilityUseParameters.target.TakeDamage(damageToDeal);
        }
    }
}
