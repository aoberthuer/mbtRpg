using System.Collections;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        private const float PARTICLE_CLEAN_DELAY = 10f;

        protected PlayerMovement playerMovement;

        protected AbilityConfig config;


        private void Start()
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        public void SetConfig(AbilityConfig configToSet)
        {
            config = configToSet;
        }

        public virtual void Use(GameObject target)
        {
            PlayParticleEffect();
            PlayAbilityAudioClip();
            PlayAbilityAnimation();
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

        private void PlayAbilityAudioClip()
        {
            AudioSource audioSource = GetComponent<AudioSource>(); // on the player...

            if (audioSource != null && config.getRandomAudioClip() != null)
            {
                audioSource.clip = config.getRandomAudioClip();
                audioSource.Play();
            }
        }

        private void PlayAbilityAnimation()
        {
            AnimatorOverrideController animatorOverrideController = GetComponent<Character>().GetAnimatorOverrideController();
            Animator animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[GameConstants.DEFAULT_ATTACK] = config.GetAbilityAnimationClip();
            animator.SetTrigger(GameConstants.ANIM_TRIGGER_ATTACK);
        }

        IEnumerator DestroyParticleWhenFinished(GameObject particleObject)
        {
            ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
            while (particleSystem!= null && particleSystem.isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_CLEAN_DELAY);
            }

            Destroy(particleObject);
            yield return new WaitForEndOfFrame();
        }

    }
}