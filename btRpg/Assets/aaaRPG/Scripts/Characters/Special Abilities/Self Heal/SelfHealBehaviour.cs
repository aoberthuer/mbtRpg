using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility
    {

        SelfHealConfig config;
        Player player;

        public void SetConfig(SelfHealConfig config)
        {
            this.config = config;
        }

        public void Use(AbilityUseParameters abilityUseParameters)
        {
            HealPlayer(abilityUseParameters);
            PlayParticleEffect();
        }

        private void HealPlayer(AbilityUseParameters abilityUseParameters)
        {
            player.TakeDamage(-config.getExtraHealth()); // heal as negative damage ;-)
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
            Debug.Log("Self Heal attached to: " + gameObject.name);
            player = GetComponent<Player>();
        }
    }
}
