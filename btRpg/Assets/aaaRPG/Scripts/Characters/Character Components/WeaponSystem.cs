using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

using RPG.Characters;
using RPG.Core;

namespace RPG.Weapons
{

    public class WeaponSystem : MonoBehaviour
    {
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
            // check continuously wether we should still be attacking
            bool targetIsDead = false;
            bool targetIsOutofRange = false;

            if(target == null)
            {
                StopAllCoroutines();
            }
            else
            {
                float targetHealth = target.GetComponent<HealthSystem>().healthAsPercentage;
                targetIsDead = (targetHealth <= Mathf.Epsilon);

                float targetDistance = Vector3.Distance(transform.position, target.transform.position);
                targetIsOutofRange = targetDistance > currentWeaponConfig.GetMaxAttackRange();
            }

            float characterHealth = GetComponent<HealthSystem>().healthAsPercentage;
            bool characterIsDead = (characterHealth <= Mathf.Epsilon);

            if (characterIsDead || targetIsDead || targetIsOutofRange)
            {
                StopAllCoroutines();
            }

        }

        public void PutWeaponInHand(WeaponConfig weaponToUse)
        {
            Destroy(weaponObject); // empty hands

            currentWeaponConfig = weaponToUse;
            GameObject weaponPrefab = currentWeaponConfig.GetWeaponPrefab();

            GameObject dominantHand = RequestDominantHand();

            if(dominantHand != null)
            {
                weaponObject = Instantiate(weaponPrefab);
                weaponObject.transform.SetParent(dominantHand.transform);

                weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
                weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
            }
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
            transform.LookAt(target.transform);
            animator.SetTrigger(GameConstants.ANIM_TRIGGER_ATTACK);
            SetAttackAnimation();

            if(currentWeaponConfig.GetRangedAttack())
            {
                SpawnProjectile();
            } 
            else
            {
                StartCoroutine(DamageAfterDelay(currentWeaponConfig.GetDamageDelay()));
            }
        }

        void SpawnProjectile()
        {
            gameObject.transform.LookAt(target.transform);

            Vector3 positionToHit = target.transform.position;
            Vector3 unitVectorToTarget = (positionToHit - gameObject.transform.position).normalized;

            Vector3 positionToSpawnFrom = gameObject.transform.position;
            positionToSpawnFrom.y += 1f; // do not spawn from feet ;-)
            positionToSpawnFrom.z += unitVectorToTarget.z; // move missile away from player in the direction player is facing x
            positionToSpawnFrom.x += unitVectorToTarget.x; // move missile away from player in the direction player is facing z

            GameObject newProjectile = Instantiate(currentWeaponConfig.GetRangedAttackPrefab(), positionToSpawnFrom, Quaternion.identity);
            newProjectile.name = currentWeaponConfig.GetRangedAttackPrefab().name;
            newProjectile.transform.LookAt(target.transform);

            Projectile projectile = newProjectile.GetComponent<Projectile>();
            projectile.SetProjectileTarget(target);
            projectile.SetDamageCaused(currentWeaponConfig.GetAdditionalDamage());

            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToTarget * 10f;



            //Vector3 positionToSpawnFrom = character.transform.position;
            //positionToSpawnFrom.y += 2; // adjust for height so arrow does not hit enemy firing the arrow

            //GameObject newProjectile = Instantiate(currentWeaponConfig.GetRangedAttackPrefab(), positionToSpawnFrom, Quaternion.identity);
            //newProjectile.transform.LookAt(target.transform);

            //Vector3 positionToHit = target.transform.position;
            //// positionToHit.y += 0.25f; // adjust for height we are not aiming at the feet. Should replace this by having a child on the character or even better some component marking place to hit like the DominantHand script...
            //Vector3 unitVectorToPlayer = (positionToHit - character.transform.position).normalized;

            //float projectileSpeed = 10f;
            //newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        }

        IEnumerator DamageAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
        }

        public void StopAttacking()
        {
            animator.StopPlayback();
            StopAllCoroutines();
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

                animatorOverrideController[GameConstants.DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimation();
            }
        }

        private GameObject RequestDominantHand()
        {
            DominantHand[] dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;

            // Assert.IsFalse(numberOfDominantHands <= 0, "No DominantHand found on Player, please add one");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple DominantHand scripts on Player, please remove one");

            if (numberOfDominantHands <= 0)
            {
                return null;
            }

            return dominantHands[0].gameObject;
        }

        private float CalculateDamage()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage();
        }
    }
}
