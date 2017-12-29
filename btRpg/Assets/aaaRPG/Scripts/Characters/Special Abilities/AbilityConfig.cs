using UnityEngine;

using RPG.Core;


namespace RPG.Characters
{ 

    public struct AbilityUseParameters
    {
        public IDamageable target;
        public float baseDamage;

        public AbilityUseParameters(IDamageable target, float baseDamage)
        {
            this.target = target;
            this.baseDamage = baseDamage;
        }
    }

    public abstract class AbilityConfig : ScriptableObject
    {

        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;

        [SerializeField] GameObject particlePrefab = null;

        [SerializeField] AudioClip[] audioClips = null;

        protected AbilityBehaviour behaviour;

        public float getEnergyCost()
        {
            return energyCost;
        }

        public GameObject getParticlePrefab()
        {
            return particlePrefab;
        }

        public AudioClip getRandomAudioClip()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }

        public abstract void AttachComponentTo(GameObject gameObjectToAttachTo);

        public void Use(AbilityUseParameters abilityUseParameters)
        {
            behaviour.Use(abilityUseParameters);
        }
    }
  

}
