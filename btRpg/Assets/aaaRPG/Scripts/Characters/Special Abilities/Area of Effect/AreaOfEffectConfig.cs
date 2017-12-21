using UnityEngine;


namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Area of Effect"))]
    public class AreaOfEffectConfig : AbilityConfig
    {

        [Header("Area of Effect Specific")]
        [SerializeField] float extraDamage = 10f;
        [SerializeField] float damageRadius = 5f;

        public override void AttachComponentTo(GameObject gameObjectToAttachTo)
        {
            AreaOfEffectBehaviour behaviourComponent = gameObjectToAttachTo.AddComponent<AreaOfEffectBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
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