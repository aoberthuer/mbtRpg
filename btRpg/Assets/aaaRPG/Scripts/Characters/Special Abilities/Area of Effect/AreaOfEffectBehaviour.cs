using UnityEngine;

using RPG.Core;
using System;

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
            DealRadialDamage();
            PlayParticleEffect();
        }

        private void DealRadialDamage()
        {
            float extraDamage = config.GetExtraDamage();
            float damageRadius = config.getDamageRadius();

            RaycastHit[] raycastHits = Physics.SphereCastAll(
                transform.position,
                damageRadius,
                Vector3.up,
                damageRadius
            );

            foreach (RaycastHit hit in raycastHits)
            {
                IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
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

        private void PlayParticleEffect()
        {
            // TODO: should we attach the newly instatiated particle system to the player. Right now it stays where it was instatiated.
            GameObject particleSystemPrefab = Instantiate(config.getParticlePrefab(), transform.position, Quaternion.identity);
            ParticleSystem particleSystem = particleSystemPrefab.GetComponent<ParticleSystem>();

            particleSystem.Play();
            Destroy(particleSystemPrefab, particleSystem.main.duration);
        }

        private void Start()
        {
            Debug.Log("Area of Effect attached to: " + gameObject.name);
        }
    }

}
