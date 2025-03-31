using UnityEngine;

public class Plant : MonoBehaviour
{
    public PlantData plantData; // Updated to use PlantData

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (plantData == null)
            {
                Debug.LogError("PlantData not set on " + gameObject.name);
                return;
            }

            FindObjectOfType<DialogueManager2>().StartPlantDialogue(plantData);
        }
    }
}
