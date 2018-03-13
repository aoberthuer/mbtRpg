using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {

        public override void Use(GameObject target, bool useTargetPosition)
        {
            base.Use(target, useTargetPosition);
            HealPlayer(target);
        }

        private void HealPlayer(GameObject target)
        {
            HealthSystem playerHealth = playerMovement.GetComponent<HealthSystem>();
            playerHealth.Heal( ((SelfHealConfig) config).getExtraHealth());
        }
    }
}
