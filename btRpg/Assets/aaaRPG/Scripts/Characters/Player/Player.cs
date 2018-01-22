using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;
using System;

namespace RPG.Characters
{
    [RequireComponent(typeof(Animator))]
    public class Player : MonoBehaviour
    {
        private const string ANIM_TRIGGER_ATTACK = "Attack";
        private const string PLAYER_DEFAULT_ATTACK = "Player Default Attack";

        [SerializeField] float baseDamage = 10f;

        [SerializeField] Weapon currentWeaponConfig;

        private GameObject weaponObject;
        private float lastHitTime = 0f;

        private Enemy enemy; // 'caching' the current enemy

        [SerializeField] AnimatorOverrideController animatorOverrideController;
        Animator animator = null;

        [Header("Special Abilities")]
        [SerializeField] AbilityConfig[] specialAbilities;

        [Header("Critical Hit")]
        [Range(0.0f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;

        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem criticalHitParticle;


        private void Start()
        {
            RegisterForMouseClick();
            
            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();

            AttachSpecialAbilities();
        }

        private void Update()
        {
            HealthSystem hs = GetComponent<HealthSystem>();
            if (hs.healthAsPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyDown();
            }
        }

        private void AttachSpecialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < specialAbilities.Length; abilityIndex++)
            {
                specialAbilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        private void ScanForAbilityKeyDown()
        {
            // start by one not zero, as zero is the power attack
            for(int abilityIndex = 1; abilityIndex < specialAbilities.Length; abilityIndex++)
            {
                if(Input.GetKeyDown(abilityIndex.ToString()))
                {
                    AttemptSpecialAbility(abilityIndex);
                }
            }
        }

        private void RegisterForMouseClick()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        public void PutWeaponInHand(Weapon weaponToUse)
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

        private void SetAttackAnimation()
        {
            animator = GetComponent<Animator>();
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

        private void OnMouseOverEnemy(Enemy enemyToSet)
        {
            this.enemy = enemyToSet;
            if(Input.GetMouseButton(0) && IsTargetInRange(this.enemy.gameObject))
            {
                AttackTarget();
            }
            else if(Input.GetMouseButtonDown(1))
            {
                AttemptSpecialAbility(0);
            }
        }

        private void AttemptSpecialAbility(int abilityIndex)
        {
            Energy energy = GetComponent<Energy>();

            if (energy != null && energy.IsEnergyAvailable(specialAbilities[abilityIndex].getEnergyCost()))
            {
                energy.ConsumeEnergy(specialAbilities[abilityIndex].getEnergyCost());

                AbilityUseParameters abilityUseParameters = new AbilityUseParameters(enemy, baseDamage);
                specialAbilities[abilityIndex].Use(abilityUseParameters);
            }
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange();
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
            bool isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            float damageBeforeCritical = baseDamage + currentWeaponConfig.GetAdditionalDamage();
            if (isCriticalHit)
            {
                if (criticalHitParticle != null)
                {
                    criticalHitParticle.Play();
                }
                return damageBeforeCritical * criticalHitMultiplier;
            }
            else
            {
                return damageBeforeCritical;
            }
        }

        public string GetTag()
        {
            return tag;
        }
    }
}
