using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility
    {

        SelfHealConfig config;
        Player player;

        AudioSource audioSource;

        private void Start()
        {
            Debug.Log("Self Heal attached to: " + gameObject.name);

            player = GetComponent<Player>();
            audioSource = GetComponent<AudioSource>(); // on the player...
        }


        public void SetConfig(SelfHealConfig config)
        {
            this.config = config;
        }

        public void Use(AbilityUseParameters abilityUseParameters)
        {
            HealPlayer(abilityUseParameters);
            PlayParticleEffect();
            PlayAudioClip();
        }

        private void HealPlayer(AbilityUseParameters abilityUseParameters)
        {
            player.Heal(config.getExtraHealth());
        }

        private void PlayParticleEffect()
        {
            // TODO: should we attach the newly instatiated particle system to the player. Right now it stays where it was instatiated.
            GameObject particleSystemPrefab = Instantiate(config.getParticlePrefab(), transform.position, Quaternion.identity);
            ParticleSystem particleSystem = particleSystemPrefab.GetComponent<ParticleSystem>();

            particleSystem.Play();
            Destroy(particleSystemPrefab, particleSystem.main.duration);
        }

        private void PlayAudioClip()
        {
            if (audioSource != null && config.getAudioClip() != null)
            {
                audioSource.clip = config.getAudioClip();
                audioSource.Play();
            }
        }
    }
}
