using UnityEngine;


namespace RPG.Characters
{ 

    public abstract class AbilityConfig : ScriptableObject
    {

        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particlePrefab;
        [SerializeField] AudioClip[] audioClips;
        [SerializeField] AnimationClip abilityAnimationClip;

        [SerializeField] bool rangedSpecialAbility;

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

        public AnimationClip GetAbilityAnimationClip()
        {
            return abilityAnimationClip;
        }

        public void AttachAbilityTo(GameObject gameObjectToAttachTo)
        {
            AbilityBehaviour behaviourComponent = GetBehaviourComponent(gameObjectToAttachTo);
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public abstract AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo);

        public void Use(GameObject target)
        {
            behaviour.Use(target, rangedSpecialAbility);
        }

    }
}
