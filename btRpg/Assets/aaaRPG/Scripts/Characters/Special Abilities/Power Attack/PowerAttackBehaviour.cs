using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config;
        
        public void SetConfig(PowerAttackConfig config)
        {
            this.config = config;
        }


        public void Use(AbilityUseParameters abilityUseParameters)
        {
            float damageToDeal = abilityUseParameters.baseDamage + config.GetExtraDamage();
            abilityUseParameters.target.TakeDamage(damageToDeal);

            Debug.Log(gameObject.name + " dealt power attack to" + abilityUseParameters.target + ". Value: " + damageToDeal);
        }

        private void Start()
        {
            Debug.Log("Power Attack attached to: " + gameObject.name);
        }
    }
}
