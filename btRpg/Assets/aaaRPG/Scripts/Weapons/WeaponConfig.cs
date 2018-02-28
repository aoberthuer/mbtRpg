using UnityEngine;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = ("RPG/Weapon"))]
    public class WeaponConfig : ScriptableObject
    {

        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;

        [SerializeField] float minTimeBetweenHits = .5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] float additionalDamage = 10f;
        [SerializeField] float damageDelay = .5f;

        [SerializeField] bool rangedAttack = false;
        [SerializeField] GameObject rangedAttackPrefab; // may be null if ranged attack is false, if true e.g. an arrow

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        public AnimationClip GetAttackAnimation()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        public float GetMinTimeBetweenHits()
        {
            return minTimeBetweenHits;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
        }

        public float GetAdditionalDamage()
        {
            return additionalDamage;
        }

        public float GetDamageDelay()
        {
            return damageDelay;
        }

        public bool GetRangedAttack()
        {
            return rangedAttack;
        }

        public GameObject GetRangedAttackPrefab()
        {
            return rangedAttackPrefab;
        }

        // Method removes all animation events so they cannot break existing code.
        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0];
        }
    }
}
