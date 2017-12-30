using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {

        public override void Use(AbilityUseParameters abilityUseParameters)
        {
            base.Use(abilityUseParameters);
            HealPlayer(abilityUseParameters);
        }

        private void HealPlayer(AbilityUseParameters abilityUseParameters)
        {
            player.Heal( ((SelfHealConfig) config).getExtraHealth());
        }
    }
}
