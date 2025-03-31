using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance; // Singleton instance

    public Text dialogueText;  // UI Text component for displaying dialogue
    public Text characterNameText;  // UI Text component for displaying NPC names
    public Image characterImage;    // UI Image component for displaying NPC images
    public GameObject dialogueBox;  // Dialogue box GameObject

    public CharacterController2DD characterController;

    private Queue<string> sentences;

    private Coroutine typingCoroutine;
    private bool isTyping;
    private string currentFullText = ""; // To store the full text of the current sentence

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate
        }
    }

    void Start()
    {
        sentences = new Queue<string>();
        HideDialogueBox(); // Ensure the dialogue box is hidden at the start of each scene
    }

    void Update()
    {
        if (dialogueBox != null && dialogueBox.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                CompleteCurrentSentence();
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }

    public void StartDialogue(NPCData npcData)
    {
        if (dialogueBox == null || characterNameText == null || characterImage == null || dialogueText == null)
        {
            Debug.LogError("One or more UI references not set in DialogueManager.");
            return;
        }

        dialogueBox.SetActive(true);
        sentences.Clear();
        characterNameText.text = npcData.name; // Set the NPC name
        characterImage.sprite = npcData.image; // Set the NPC image

        foreach (string sentence in npcData.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisablePlayerMovement();
        DisplayNextSentence();
    }

    void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        if (!isTyping)
        {
            string sentence = sentences.Dequeue();
            currentFullText = sentence; // Store the full sentence for immediate display if needed.
            typingCoroutine = StartCoroutine(TypeSentence(sentence));
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        if (dialogueText == null)
        {
            Debug.LogError("Dialogue Text component is missing.");
            yield break;
        }

        dialogueText.text = "";
        isTyping = true;

        float initialDelay = 0.05f;  // Start with a reasonable delay.
        float minimumDelay = 0.01f; // Minimum delay to reach at the end of the sentence.
        float accelerationFactor = (initialDelay - minimumDelay) / (sentence.Length * 0.5f); // Accelerate faster by reducing the divisor.

        float currentDelay = initialDelay;

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(currentDelay);

            // Accelerate by decreasing the delay, but not less than the minimum delay.
            currentDelay = Mathf.Max(minimumDelay, currentDelay - accelerationFactor);
        }

        isTyping = false;

        // Wait for an additional mouse click after the sentence is completed before proceeding.
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }

    void CompleteCurrentSentence()
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Stops the current typing effect
            dialogueText.text = currentFullText; // Displays the full text immediately
            isTyping = false;
        }
    }

    void EndDialogue()
    {
        HideDialogueBox();
        EnablePlayerMovement();
    }

    void HideDialogueBox()
    {
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }
    }

    void DisablePlayerMovement()
    {
        if (characterController != null)
        {
            characterController.StopMovement();
            characterController.enabled = false;
        }
    }

    void EnablePlayerMovement()
    {
        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset the dialogue box when a new scene is loaded
        HideDialogueBox();
        ResetUIComponents();
    }

    private void ResetUIComponents()
    {
        // Attempt to find the UI components if they are not assigned
        if (dialogueBox == null)
        {
            dialogueBox = GameObject.Find("DialogueBox");
        }
        if (dialogueText == null)
        {
            dialogueText = GameObject.Find("DialogueText")?.GetComponent<Text>();
        }
        if (characterNameText == null)
        {
            characterNameText = GameObject.Find("CharacterNameText")?.GetComponent<Text>();
        }
        if (characterImage == null)
        {
            characterImage = GameObject.Find("CharacterImage")?.GetComponent<Image>();
        }

        // Log warnings if any UI references are still missing
        if (dialogueBox == null || dialogueText == null || characterNameText == null || characterImage == null)
        {
            Debug.LogWarning("One or more UI references are missing. Please check your scene setup.");
        }
    }
}
