using UnityEngine;
using UnityEngine.Assertions;

using RPG.Characters;

namespace RPG.Weapons
{


    public class WeaponSystem : MonoBehaviour
    {
        private const string ANIM_TRIGGER_ATTACK = "Attack";
        private const string PLAYER_DEFAULT_ATTACK = "Player Default Attack";

        [SerializeField] float baseDamage = 10f;
        [SerializeField] WeaponConfig currentWeaponConfig;

        private Character character;
        private GameObject weaponObject;
        private GameObject target;

        private Animator animator;

        private float lastHitTime = 0f;

        void Start()
        {
            character = GetComponent<Character>();
            animator = GetComponent<Animator>();

            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();
        }

        void Update()
        {

        }

        public void PutWeaponInHand(WeaponConfig weaponToUse)
        {
            Destroy(weaponObject); // empty hands

            currentWeaponConfig = weaponToUse;
            GameObject weaponPrefab = currentWeaponConfig.GetWeaponPrefab();

            GameObject dominantHand = RequestDominantHand();
            weaponObject = Instantiate(weaponPrefab);
            weaponObject.transform.SetParent(dominantHand.transform);

            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            print("Attacking " + targetToAttack);

            // todo use a repeat attack co-routine
            AttackTarget();
        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        private void SetAttackAnimation()
        {
            AnimatorOverrideController animatorOverrideController = character.GetAnimatorOverrideController();

            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[PLAYER_DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimation();
        }

        private GameObject RequestDominantHand()
        {
            DominantHand[] dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;

            Assert.IsFalse(numberOfDominantHands <= 0, "No DominantHand found on Player, please add one");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple DominantHand scripts on Player, please remove one");

            return dominantHands[0].gameObject;
        }

        private void AttackTarget()
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())
            {
                SetAttackAnimation(); // set here as only setting weapon change will not suffice (for e.g. a power attack)
                animator.SetTrigger(ANIM_TRIGGER_ATTACK);
                // enemy.TakeDamage(CalculateDamage());
                lastHitTime = Time.time;
            }
        }

        private float CalculateDamage()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage();
        }
    }
}
