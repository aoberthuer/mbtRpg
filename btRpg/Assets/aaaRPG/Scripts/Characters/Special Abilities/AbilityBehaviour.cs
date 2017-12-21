using UnityEngine;

namespace RPG.Characters
{


    public abstract class AbilityBehaviour : MonoBehaviour, ISpecialAbility
    {

        protected Player player;

        protected AudioSource audioSource;

        protected AbilityConfig config;


        private void Start()
        {
            player = GetComponent<Player>();
            audioSource = GetComponent<AudioSource>(); // on the player...
        }

        public virtual void Use(AbilityUseParameters abilityUseParameters)
        {
            PlayParticleEffect();
            PlayAudioClip();
        }

        public void PlayParticleEffect()
        {
            // TODO: should we attach the newly instatiated particle system to the player. Right now it stays where it was instatiated.
            GameObject particleSystemPrefab = Instantiate(config.getParticlePrefab(), transform.position, Quaternion.identity);

            if(config.getParticleInLocalSpace())
            {
                particleSystemPrefab.transform.parent = transform;
            }

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