using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    [RequireComponent(typeof(Image))]
    public class SpecialAbilities : MonoBehaviour
    {
        [Header("Special Abilities")]
        [SerializeField] AbilityConfig[] specialAbilities;

        [SerializeField] Image energyOrb;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenerateEnergyPerSecond = 1f;

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
            for (int abilityIndex = 0; abilityIndex < specialAbilities.Length; abilityIndex++)
            {
                specialAbilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        public void AttemptSpecialAbility(int abilityIndex)
        {
            float energyCost = specialAbilities[abilityIndex].getEnergyCost();
            if (IsEnergyAvailable(energyCost))
            {
                ConsumeEnergy(energyCost);

                print("Using special ability " + abilityIndex);  // todo make work
                specialAbilities[abilityIndex].Use(null);
            }
            else
            {
                // todo play out of energy sound
            }
        }

        public int GetNumberOfAbilities()
        {
            return specialAbilities.Length;
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
