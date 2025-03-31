using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New NPC", menuName = "Character/NPC")]
public class NPCData : ScriptableObject
{
    public string name;
    public Sprite image;
    public string[] sentences;
    public bool needsTreatment; // Whether this NPC needs treatment
    public string[] requiredMedicinalValues; // Medicinal values required by the NPC

    // Method to assign medicinal values from a non-poisonous plant
    public void AssignRandomMedicinalValues(List<PlantData> allPlantData)
    {
        if (!needsTreatment || requiredMedicinalValues != null && requiredMedicinalValues.Length > 0)
            return;

        // Filter out poisonous plants
        var nonPoisonousPlants = allPlantData.Where(plant => !plant.plantEffects.Contains("Poisonous")).ToList();

        if (nonPoisonousPlants.Count > 0)
        {
            // Select a random non-poisonous plant
            PlantData selectedPlant = nonPoisonousPlants[Random.Range(0, nonPoisonousPlants.Count)];
            requiredMedicinalValues = selectedPlant.plantEffects.ToArray(); // Ensure traits are assigned to the array

            // Update the sentences to include the required medicinal values
            sentences = new string[]
            {
                $"I need help! I have fallen terribly ill and need your assistance traveller. Please find a plant with the medicinal traits: {string.Join(", ", requiredMedicinalValues)}."
            };

            Debug.Log($"{name} requires medicinal traits: {string.Join(", ", requiredMedicinalValues)} from plant {selectedPlant.plantName}.");
        }
        else
        {
            Debug.LogError("No non-poisonous plants available to assign medicinal values.");
        }
    }
}
