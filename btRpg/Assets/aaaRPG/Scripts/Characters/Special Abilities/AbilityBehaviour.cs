using System.Collections;
using UnityEngine;

namespace RPG.Characters
{


    public abstract class AbilityBehaviour : MonoBehaviour
    {
        private const float PARTICLE_CLEAN_DELAY = 10f;

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
            GameObject particleObject = Instantiate(config.getParticlePrefab(), transform.position, config.getParticlePrefab().transform.rotation);

            // This will child the particle system to the player.
            // You need to set world vs local space on the particle system itself (in main) and do not forget children particle systems (check
            // end of lesson 140 for example).
            particleObject.transform.parent = transform;

            // Play particle system on top level component (if present)...
            ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play(true); // plays child particle systems as well (if present)
                StartCoroutine(DestroyParticleWhenFinished(particleObject));
            }
        }

        IEnumerator DestroyParticleWhenFinished(GameObject particleObject)
        {
            ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
            while (particleSystem.isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_CLEAN_DELAY);
            }

            Destroy(particleObject);
            yield return new WaitForEndOfFrame();
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