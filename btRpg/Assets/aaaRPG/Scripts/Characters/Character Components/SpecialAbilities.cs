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
            for (int abilityIndex = 0; abilityIndex < specialAbilities.Length; abilityIndex++)
            {
                specialAbilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            float energyCost = specialAbilities[abilityIndex].getEnergyCost();
            if (IsEnergyAvailable(energyCost))
            {
                ConsumeEnergy(energyCost);
                specialAbilities[abilityIndex].Use(null);
            }
            else
            {
                if(!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(outOfEnergyClip);
                }
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
