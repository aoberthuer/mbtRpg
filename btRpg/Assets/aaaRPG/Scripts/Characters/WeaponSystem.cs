using UnityEngine;
using UnityEngine.Assertions;

using RPG.Characters;
using System.Collections;

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
            StartCoroutine(AttackTargetRepeatedly());
        }


        private IEnumerator AttackTargetRepeatedly()
        {
            // determine if alive (attacker and defender)
            bool attackerStillAlive = GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            bool targetStillAlive = target.GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;

            while (attackerStillAlive && targetStillAlive)
            {
                float weaponHitPeriod = currentWeaponConfig.GetMinTimeBetweenHits();
                float timeToWait = weaponHitPeriod * character.GetAnimSpeedMultiplier();

                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;

                if (isTimeToHitAgain)
                {
                    AttackTargetOnce();
                    lastHitTime = Time.time;
                }
                yield return new WaitForSeconds(timeToWait);
            }
        }

        private void AttackTargetOnce()
        {
            if(target == null)
            {
                return;
            }

            transform.LookAt(target.transform);
            animator.SetTrigger(ANIM_TRIGGER_ATTACK);
            float damageDelay = 1.0f; // todo get from the weapon
            SetAttackAnimation();
            StartCoroutine(DamageAfterDelay(damageDelay));
        }

        IEnumerator DamageAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            if(target != null)
            {
                target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
            }
        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        private void SetAttackAnimation()
        {
            if (!character.GetAnimatorOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("Please provide " + gameObject + " with an animator override controller.");
            }
            else
            {
                AnimatorOverrideController animatorOverrideController = character.GetAnimatorOverrideController();
                animator.runtimeAnimatorController = animatorOverrideController;
                animatorOverrideController[PLAYER_DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimation();
            }
        }

        private GameObject RequestDominantHand()
        {
            DominantHand[] dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;

            Assert.IsFalse(numberOfDominantHands <= 0, "No DominantHand found on Player, please add one");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple DominantHand scripts on Player, please remove one");

            return dominantHands[0].gameObject;
        }

        private float CalculateDamage()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage();
        }
    }
}
