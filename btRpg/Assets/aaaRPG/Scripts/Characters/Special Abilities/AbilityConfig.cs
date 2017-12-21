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

    public interface ISpecialAbility
    {
        void Use(AbilityUseParameters abilityUseParameters);
    }

    public abstract class AbilityConfig : ScriptableObject
    {

        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;

        [SerializeField] GameObject particlePrefab = null;
        [SerializeField] bool particleInLocalSpace = false;

        [SerializeField] AudioClip audioClip = null;

        protected ISpecialAbility behaviour;

        public float getEnergyCost()
        {
            return energyCost;
        }

        public GameObject getParticlePrefab()
        {
            return particlePrefab;
        }

        public bool getParticleInLocalSpace()
        {
            return particleInLocalSpace;
        }

        public AudioClip getAudioClip()
        {
            return audioClip;
        }

        public abstract void AttachComponentTo(GameObject gameObjectToAttachTo);

        public void Use(AbilityUseParameters abilityUseParameters)
        {
            behaviour.Use(abilityUseParameters);
        }
    }
  

}
