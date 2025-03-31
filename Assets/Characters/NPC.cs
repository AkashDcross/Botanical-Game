using UnityEngine;
using System.Linq;
using System.Collections;

public class NPC : MonoBehaviour
{
    public NPCData npcData;
    public string[] requiredMedicinalValues; // The specific medicinal values needed to cure this NPC
    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip correctSound; // Sound to play when the correct plant is given
    public AudioClip incorrectSound; // Sound to play when the incorrect plant is given
    public int scoreValue = 10; // The amount of score to increase when this NPC is cured

    private bool isCured = false;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // Get the AudioSource component attached to the NPC
            if (audioSource == null)
            {
                Debug.LogError("No AudioSource found on NPC. Please add an AudioSource component.");
            }
        }

        AssignRandomMedicinalTraits(); // Assign random medicinal traits to the NPC
    }

    public bool TreatWithPlant(PlantData plantData)
    {
        if (isCured) return false; // Prevent further treatment after the NPC is already cured.

        if (IsCureCorrect(plantData.plantEffects))
        {
            Debug.Log($"{npcData.name} has been cured!");
            PlaySound(correctSound); // Play the correct sound
            ScoreManager.Instance?.IncreaseScore(scoreValue); // Increase the player's score
            StartCoroutine(DeactivateAfterSound(correctSound?.length ?? 0)); // Deactivate after sound is played

            isCured = true; // Mark the NPC as cured
            return true;
        }
        else
        {
            Debug.Log($"{npcData.name} was not cured. The plant used was incorrect.");
            PlaySound(incorrectSound); // Play the incorrect sound
            ScoreManager.Instance?.IncrementIncorrectPlantsGiven(); // Increment the incorrect plants given count
            return false;
        }
    }

    private bool IsCureCorrect(string[] plantEffects)
    {
        // Check if the plant contains all the required medicinal values
        foreach (string value in requiredMedicinalValues)
        {
            if (!plantEffects.Contains(value))
            {
                return false; // If any value is missing, return false
            }
        }
        return true; // All required values are present
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator DeactivateAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false); // Deactivate the NPC after the sound has finished playing
    }

    private void AssignRandomMedicinalTraits()
    {
        PlantHotbarManager hotbarManager = FindObjectOfType<PlantHotbarManager>();
        if (hotbarManager != null && hotbarManager.allPlantData != null && hotbarManager.allPlantData.Count > 0)
        {
            var nonPoisonousPlants = hotbarManager.allPlantData.Where(p => !p.plantEffects.Contains("Poisonous")).ToList();
            if (nonPoisonousPlants.Count > 0)
            {
                PlantData randomPlant = nonPoisonousPlants[Random.Range(0, nonPoisonousPlants.Count)];
                requiredMedicinalValues = randomPlant.plantEffects;

                // Update the NPCData sentences to include the medicinal traits request
                string treatmentRequest = $"I need help! I have fallen terribly ill and need your assistance, traveller. Please find a plant with the medicinal traits: {string.Join(", ", requiredMedicinalValues)}.";
                npcData.sentences = new string[] { treatmentRequest };

                Debug.Log($"{npcData.name} requires medicinal traits: {string.Join(", ", requiredMedicinalValues)} from plant {randomPlant.plantName}.");
            }
        }
        else
        {
            Debug.LogError("Plant data list is empty or not assigned in PlantHotbarManager.");
        }
    }
}
