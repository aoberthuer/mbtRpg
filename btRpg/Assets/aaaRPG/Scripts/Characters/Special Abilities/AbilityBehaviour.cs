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
            GameObject particleSystemPrefab = Instantiate(config.getParticlePrefab(), transform.position, config.getParticlePrefab().transform.rotation);

            if(config.getParticleInLocalSpace())
            {
                particleSystemPrefab.transform.parent = transform;
            }


            // Play particle system on top level component (if present)
            ParticleSystem particleSystem = particleSystemPrefab.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play(true); // plays child particle systems as well (if present)
                Destroy(particleSystemPrefab, particleSystem.main.duration);
            }
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