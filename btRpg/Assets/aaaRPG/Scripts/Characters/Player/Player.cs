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
    public class Player : MonoBehaviour, IDamageable
    {
        private const string ANIM_TRIGGER_ATTACK = "Attack";
        private const string ANIM_TRIGGER_DEATH = "Death";
        private const string PLAYER_DEFAULT_ATTACK = "Player Default Attack";

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float baseDamage = 10f;

        [SerializeField] Weapon currentWeaponConfig = null;

        private GameObject weaponObject;
        private float lastHitTime = 0f;
        private float currentHealthPoints;

        private Enemy enemy = null; // 'caching' the current enemy

        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        Animator animator = null;

        [Header("Special Abilities")]
        [SerializeField] AbilityConfig[] specialAbilities;

        [Header("Sounds")]
        [SerializeField] AudioClip[] deathSounds;
        [SerializeField] AudioClip[] damageSounds;

        private AudioSource audioSource = null;

        [Header("Critical Hit")]
        [Range(0.0f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;

        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem criticalHitParticle = null;


        private void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // might also have added AudioSource component to Player and retrieved it via GetComponent<>
            audioSource.playOnAwake = false;

            RegisterForMouseClick();
            SetCurrentMaxHealth();

            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();

            AttachSpecialAbilities();
        }

        private void Update()
        {
            if (healthAsPercentage > Mathf.Epsilon)
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

        private void SetCurrentMaxHealth()
        {
            currentHealthPoints = maxHealthPoints;
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
                enemy.TakeDamage(CalculateDamage());
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

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);

            audioSource.clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            audioSource.Play();

            if (currentHealthPoints <= 0)
            {
                StartCoroutine(KillPlayer());
            }
        }

        public void Heal(float points)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0f, maxHealthPoints);
        }

        IEnumerator KillPlayer()
        {
            // trigger death animation
            animator.SetTrigger(ANIM_TRIGGER_DEATH);

            // play death sound
            audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.Play();

            // wait a bit (actually length of clip)
            yield return new WaitForSeconds(audioSource.clip.length);

            // reload scene
            SceneManager.LoadScene("Scene.01"); 

            // final return
            yield return null;
        }

        public string GetTag()
        {
            return tag;
        }
    }
}
