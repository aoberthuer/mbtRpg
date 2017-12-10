using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{


    public class AreaOfEffectBehaviour : MonoBehaviour, ISpecialAbility
    {

        AreaOfEffectConfig config;

        public void SetConfig(AreaOfEffectConfig config)
        {
            this.config = config;
        }

        public void Use(AbilityUseParameters abilityUseParameters)
        {
            float extraDamage = config.GetExtraDamage();
            float damageRadius = config.getDamageRadius();

            RaycastHit[] raycastHits = Physics.SphereCastAll(
                transform.position,
                damageRadius,
                Vector3.up,
                damageRadius
            );

            foreach(RaycastHit hit in raycastHits)
            {
                IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                if(damageable != null)
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

        private void Start()
        {
            Debug.Log("Area of Effect attached to: " + gameObject.name);
        }
    }

}
