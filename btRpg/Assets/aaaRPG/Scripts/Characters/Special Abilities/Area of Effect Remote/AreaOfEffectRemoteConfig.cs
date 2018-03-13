using UnityEngine;


namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Area of Effect Remote"))]
    public class AreaOfEffectRemoteConfig : AbilityConfig
    {

        [Header("Area of Effect Remote Specific")]
        [SerializeField] float extraDamage = 10f;
        [SerializeField] float damageRadius = 5f;

        public override AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo)
        {
            return gameObjectToAttachTo.AddComponent<AreaOfEffectRemoteBehaviour>();
        }

        public float GetExtraDamage()
        {
            return extraDamage;
        }

        public float getDamageRadius()
        {
            return damageRadius;
        }
    }
}
