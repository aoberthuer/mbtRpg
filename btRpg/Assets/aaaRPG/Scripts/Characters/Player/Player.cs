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

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float baseDamage = 10f;

        [SerializeField] Weapon weaponInUse = null;
        float lastHitTime = 0f;

        float currentHealthPoints;

        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        Animator animator = null;

        [SerializeField] AbilityConfig[] specialAbilities;

        [SerializeField] AudioClip[] deathSounds;
        [SerializeField] AudioClip[] damageSounds;

        AudioSource audioSource;

        Enemy enemy = null;


        private void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // might also have added AudioSource component to Player and retrieved it via GetComponent<>
            audioSource.playOnAwake = false;

            RegisterForMouseClick();
            SetCurrentMaxHealth();

            PutWeaponInHand();
            SetupRuntimeAnimator();

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
                specialAbilities[abilityIndex].AttachComponentTo(gameObject);
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

        private void PutWeaponInHand()
        {
            GameObject weaponPrefab = weaponInUse.GetWeaponPrefab();

            GameObject dominantHand = RequestDominantHand();
            GameObject weapon = Instantiate(weaponPrefab, dominantHand.transform);

            weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
            weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;
        }

        private void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["Player Default Attack"] = weaponInUse.GetAttackAnimation();

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
            return distanceToTarget <= weaponInUse.GetMaxAttackRange();
        }

        private void AttackTarget()
        {
            if (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits())
            {
                animator.SetTrigger(ANIM_TRIGGER_ATTACK);
                enemy.TakeDamage(baseDamage);
                lastHitTime = Time.time;
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
