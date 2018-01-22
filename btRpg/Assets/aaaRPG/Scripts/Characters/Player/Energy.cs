using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    [RequireComponent(typeof(Image))]
    public class Energy : MonoBehaviour
    {
        [SerializeField] Image energyOrb;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenerateEnergyPerSecond = 1f;

        private float currentEnergyPoints;


        void Start()
        {
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

        private void AddEnergyPoints()
        {
            float newEnergyPoints = currentEnergyPoints + Time.deltaTime * regenerateEnergyPerSecond;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0f, maxEnergyPoints);
        }

        private void SetCurrentMaxEnergy()
        {
            currentEnergyPoints = maxEnergyPoints;
        }

        public bool IsEnergyAvailable(float energyAmount)
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
            energyOrb.fillAmount = GetCurrentEnergyPointsAsPercentage();
        }

        private float GetCurrentEnergyPointsAsPercentage()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }
}
