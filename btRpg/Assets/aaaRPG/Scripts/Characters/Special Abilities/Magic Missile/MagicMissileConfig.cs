using UnityEngine;


namespace RPG.Characters
{

    [CreateAssetMenu(menuName = ("RPG/Special Ability/Magic Missile"))]
    public class MagicMissileConfig : AbilityConfig
    {

        [Header("Magic Missile Specific")]
        [SerializeField] GameObject missile;
        [SerializeField] float missileDamage = 10f;

        public override AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo)
        {
            return gameObjectToAttachTo.AddComponent<MagicMissileBehaviour>();
        }

        public float GetMissileDamage()
        {
            return missileDamage;
        }

        public GameObject GetMissile()
        {
            return missile;
        }
    }
}
