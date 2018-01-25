using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {

        public override void Use(GameObject target)
        {
            base.Use(target);
            HealPlayer(target);
        }

        private void HealPlayer(GameObject target)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.Heal( ((SelfHealConfig) config).getExtraHealth());
        }
    }
}
