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
            // start by one not zero, as zero is the power attack
            for(int abilityIndex = 1; abilityIndex < specialAbilities.GetNumberOfAbilities(); abilityIndex++)
            {
                if(Input.GetKeyDown(abilityIndex.ToString()))
                {
                    specialAbilities.AttemptSpecialAbility(abilityIndex);
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
                if (IsTargetInRange(enemy.gameObject))
                {
                    specialAbilities.AttemptSpecialAbility(0, enemy.gameObject);
                }
                else
                {
                    StartCoroutine(MoveAndPowerAttack(enemy));
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
            specialAbilities.AttemptSpecialAbility(0, enemy.gameObject);
        }

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
