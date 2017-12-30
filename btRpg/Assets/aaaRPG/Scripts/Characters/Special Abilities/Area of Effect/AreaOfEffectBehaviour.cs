using UnityEngine;

using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaOfEffectBehaviour : AbilityBehaviour
    {

        public override void Use(AbilityUseParameters abilityUseParameters)
        {
            base.Use(abilityUseParameters);
            DealRadialDamage();
        }

        private void DealRadialDamage()
        {
            AreaOfEffectConfig aoeConfig = (AreaOfEffectConfig) config;

            float extraDamage = aoeConfig.GetExtraDamage();
            float damageRadius = aoeConfig.getDamageRadius();

            RaycastHit[] raycastHits = Physics.SphereCastAll(
                transform.position,
                damageRadius,
                Vector3.up,
                damageRadius
            );

            foreach (RaycastHit hit in raycastHits)
            {
                IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();

                // could solve it like this... (course's solution)
                // bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();
                // if (damageable != null && !hitPlayer)

                // or like this... here the origin (not specifically the player) will not be affected by the aoe.
                if (damageable != null)
                {

                    Debug.Log("Game object's tag: " + gameObject.tag + ", IDamageable's tag: " + damageable.GetTag());

                    if (gameObject.tag != damageable.GetTag())
                    {
                        // float damageToDeal = abilityUseParameters.baseDamage + extraDamage;
                        float damageToDeal = extraDamage; // do not think the base damage should factor in here...?
                        damageable.TakeDamage(damageToDeal);

                    }

                }
            }
        }

    }

}
