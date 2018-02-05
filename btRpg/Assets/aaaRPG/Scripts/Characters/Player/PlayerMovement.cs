using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;
using RPG.Weapons;

namespace RPG.Characters
{

    public class PlayerMovement : MonoBehaviour
    {
        private Character character;
        private WeaponSystem weaponSystem;
        private SpecialAbilities specialAbilities;

        private Enemy enemy; // 'caching' the current enemy
       
        [Header("Critical Hit")]
        [Range(0.0f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem criticalHitParticle;


        private void Start()
        {
            character = GetComponent<Character>();
            weaponSystem = GetComponent<WeaponSystem>();
            specialAbilities = GetComponent<SpecialAbilities>();

            RegisterForMouseEvents();
        }

        private void RegisterForMouseEvents()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();

            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverWalkable += OnMouseOverWalkable;
        }

        

        private void Update()
        {
            ScanForAbilityKeyDown();
        }

        private void ScanForAbilityKeyDown()
        {
            // start by one not zero, as zero is the power attack
            for(int abilityIndex = 1; abilityIndex < specialAbilities.GetNumberOfAbilities(); abilityIndex++)
            {
                if(Input.GetKeyDown(abilityIndex.ToString()))
                {
                    if (enemy != null)
                    {
                        specialAbilities.AttemptSpecialAbility(abilityIndex, enemy.gameObject);
                    }
                    else
                    {
                        specialAbilities.AttemptSpecialAbility(abilityIndex);
                    }
                }
            }
        }

        private void OnMouseOverEnemy(Enemy enemyToSet)
        {
            this.enemy = enemyToSet;
            if(Input.GetMouseButton(0) && IsTargetInRange(this.enemy.gameObject))
            {
                weaponSystem.AttackTarget(enemy.gameObject);
            }
            else if(Input.GetMouseButtonDown(1))
            {
                specialAbilities.AttemptSpecialAbility(0);
            }
        }

        // copied from Character class
        //private void OnMouseOverEnemy(Enemy enemy)
        //{
        //    if(Input.GetMouseButton(0) || Input.GetMouseButtonDown(1))
        //    {
        //        navMeshAgent.SetDestination(enemy.transform.position);
        //    }
        //}

        private void OnMouseOverWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                character.SetDestination(destination);
            }
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
        }

        public string GetTag()
        {
            return tag;
        }
    }
}
