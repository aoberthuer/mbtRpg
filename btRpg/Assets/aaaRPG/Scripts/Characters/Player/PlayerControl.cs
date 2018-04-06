using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;
using RPG.Weapons;

namespace RPG.Characters
{

    public class PlayerControl : MonoBehaviour
    {
        private Character character;
        private WeaponSystem weaponSystem;
        private SpecialAbilities specialAbilities;


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
           
            for(int abilityIndex = 0; abilityIndex < specialAbilities.GetNumberOfAbilitiesOnPlayer(); abilityIndex++)
            {
                if(Input.GetKeyDown(abilityIndex.ToString()))
                {
                    specialAbilities.AttemptSpecialAbilityOnPlayer(abilityIndex);
                }
            }
        }

        private void OnMouseOverEnemy(EnemyControlAI enemy)
        {
            if(Input.GetMouseButton(0))
            {
                if (IsTargetInRange(enemy.gameObject))
                {
                    weaponSystem.AttackTarget(enemy.gameObject);
                } 
                else
                {
                    StartCoroutine(MoveAndAttack(enemy));
                }
            }
            else if(Input.GetMouseButtonDown(1))
            {
                AbilityConfig meleeConfig = specialAbilities.getSpecialAbilityMelee();
                if(meleeConfig.GetRangedSpecialAbility())
                {
                    specialAbilities.AttemptSpecialAbilityMelee(enemy.gameObject);
                }
                else
                {
                    if (IsTargetInRange(enemy.gameObject))
                    {
                        specialAbilities.AttemptSpecialAbilityMelee(enemy.gameObject);
                    }
                    else
                    {
                        StartCoroutine(MoveAndPowerAttack(enemy));
                    }
                }
            }
        }

        IEnumerator MoveToTarget(GameObject target)
        {
            character.SetDestination(target.transform.position);
            while (!IsTargetInRange(target))
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }

        IEnumerator MoveAndAttack(EnemyControlAI enemy)
        {
            yield return StartCoroutine(MoveToTarget(enemy.gameObject));
            weaponSystem.AttackTarget(enemy.gameObject);
        }

        IEnumerator MoveAndPowerAttack(EnemyControlAI enemy)
        {
            yield return StartCoroutine(MoveToTarget(enemy.gameObject));
            specialAbilities.AttemptSpecialAbilityMelee(enemy.gameObject);
        }

        private void OnMouseOverWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                weaponSystem.StopAttacking();
                character.SetDestination(destination);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                GameObject targetGameObject = new GameObject();
                targetGameObject.transform.position = destination;
                specialAbilities.AttemptSpecialAbilityRanged(0, targetGameObject);
            }
        }

        private bool IsTargetInRange(GameObject target)
        {
            if (target == null)
                return false;

            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
        }

        public string GetTag()
        {
            return tag;
        }
    }
}
