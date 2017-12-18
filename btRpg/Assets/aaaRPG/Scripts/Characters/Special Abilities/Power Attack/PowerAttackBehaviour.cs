﻿using System.Collections;
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
            DealDamage(abilityUseParameters);
            PlayParticleEffect();
        }

        private void DealDamage(AbilityUseParameters abilityUseParameters)
        {
            float damageToDeal = abilityUseParameters.baseDamage + config.GetExtraDamage();
            abilityUseParameters.target.TakeDamage(damageToDeal);
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
            Debug.Log("Power Attack attached to: " + gameObject.name);
        }
    }
}
