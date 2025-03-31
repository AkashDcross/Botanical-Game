using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public NPCData npcData;
    private bool isDialogueActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isDialogueActive)
        {
            DialogueManager.Instance.StartDialogue(npcData);
            isDialogueActive = true;
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
