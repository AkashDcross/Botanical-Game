using UnityEngine;

public class PlantInteraction : MonoBehaviour
{
    public PlantData plantData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlantDiscoveryManager.Instance.OnPlantInteraction(plantData);
            CollectPlant(); // Call this method to handle the plant after interaction
        }
    }

    // Method to disable or destroy the plant object after interaction
    private void CollectPlant()
    {
        // Option 1: Deactivate the plant (can be reactivated later if needed)
        gameObject.SetActive(false);

        // Option 2: Completely destroy the plant object (cannot be undone)
        // Destroy(gameObject);
    }
}
