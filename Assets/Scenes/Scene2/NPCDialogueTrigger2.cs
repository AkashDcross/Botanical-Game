using UnityEngine;

public class NPCDialogueTrigger2 : MonoBehaviour
{
    public NPCData npcData; // NPC data to use for dialogue
    public PlantData plantData; // Plant data to use for dialogue
    private bool isDialogueActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isDialogueActive)
        {
            if (npcData != null)
            {
                if (DialogueManager2.Instance != null)
                {
                    // Just start the dialogue. The sentences should already include the treatment request if needed.
                    DialogueManager2.Instance.StartNPCDialogue(npcData);
                    isDialogueActive = true;
                }
                else
                {
                    Debug.LogError("DialogueManager2 instance is missing. Make sure it is properly initialized.");
                }
            }
            else if (plantData != null)
            {
                if (DialogueManager2.Instance != null)
                {
                    DialogueManager2.Instance.StartPlantDialogue(plantData); // Call the StartPlantDialogue method for Plants
                    isDialogueActive = true;
                }
                else
                {
                    Debug.LogError("DialogueManager2 instance is missing. Make sure it is properly initialized.");
                }
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isDialogueActive = false;
        }
    }
}
