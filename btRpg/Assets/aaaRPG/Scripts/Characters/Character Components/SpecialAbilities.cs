using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    [RequireComponent(typeof(Image))]
    public class SpecialAbilities : MonoBehaviour
    {
        [Header("Special Abilities")]
        [SerializeField] AbilityConfig specialAbilityMelee;
        [SerializeField] AbilityConfig[] specialAbilitiesOnPlayer;
        [SerializeField] AbilityConfig[] specialAbilitiesRanged;


        [SerializeField] Image energyOrb;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenerateEnergyPerSecond = 1f;

        [SerializeField] AudioClip outOfEnergyClip;

        private float currentEnergyPoints;

        private AudioSource audioSource;


        void Start()
        {
            audioSource = GetComponent<AudioSource>();

            AttachSpecialAbilities();

            SetCurrentMaxEnergy();
            UpdateEnergyOrb();
        }

        private void Update()
        {
            if(currentEnergyPoints < maxEnergyPoints)
            {
                AddEnergyPoints();
                UpdateEnergyOrb();
            }
        }

        private void SetCurrentMaxEnergy()
        {
            currentEnergyPoints = maxEnergyPoints;
        }

        private void AttachSpecialAbilities()
        {
            specialAbilityMelee.AttachAbilityTo(gameObject);

            for (int abilityIndex = 0; abilityIndex < specialAbilitiesOnPlayer.Length; abilityIndex++)
            {
                specialAbilitiesOnPlayer[abilityIndex].AttachAbilityTo(gameObject);
            }

            for (int abilityIndex = 0; abilityIndex < specialAbilitiesRanged.Length; abilityIndex++)
            {
                specialAbilitiesRanged[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        public void AttemptSpecialAbilityMelee(GameObject targetGameObject = null)
        {
            float energyCost = specialAbilityMelee.getEnergyCost();
            if (IsEnergyAvailable(energyCost))
            {
                ConsumeEnergy(energyCost);
                specialAbilityMelee.Use(targetGameObject);
            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(outOfEnergyClip);
                }
            }
        }

        public AbilityConfig getSpecialAbilityMelee()
        {
            return specialAbilityMelee;
        }

        public void AttemptSpecialAbilityOnPlayer(int abilityIndex, GameObject targetGameObject = null)
        {
            float energyCost = specialAbilitiesOnPlayer[abilityIndex].getEnergyCost();
            if (IsEnergyAvailable(energyCost))
            {
                ConsumeEnergy(energyCost);
                specialAbilitiesOnPlayer[abilityIndex].Use(targetGameObject);
            }
            else
            {
                if(!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(outOfEnergyClip);
                }
            }
        }

        public void AttemptSpecialAbilityRanged(int abilityIndex, GameObject targetGameObject = null)
        {
            float energyCost = specialAbilitiesRanged[abilityIndex].getEnergyCost();
            if (IsEnergyAvailable(energyCost))
            {
                ConsumeEnergy(energyCost);
                specialAbilitiesRanged[abilityIndex].Use(targetGameObject);
            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(outOfEnergyClip);
                }
            }
        }

        public int GetNumberOfAbilitiesOnPlayer()
        {
            return specialAbilitiesOnPlayer.Length;
        }

        public int GetNumberOfAbilitiesRanged()
        {
            return specialAbilitiesRanged.Length;
        }

        private void AddEnergyPoints()
        {
            float newEnergyPoints = currentEnergyPoints + Time.deltaTime * regenerateEnergyPerSecond;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0f, maxEnergyPoints);
        }

        private bool IsEnergyAvailable(float energyAmount)
        {
            return energyAmount <= currentEnergyPoints;
        }

        public void ConsumeEnergy(float energyConsumed)
        {
            float newEnergyPoints = currentEnergyPoints - energyConsumed;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0f, maxEnergyPoints);

            UpdateEnergyOrb();
        }

        private void UpdateEnergyOrb()
        {
            energyOrb.fillAmount = EnergyAsPercentage;
        }

        float EnergyAsPercentage { get { return currentEnergyPoints / maxEnergyPoints; } }
    }
}
