using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueManager2 : MonoBehaviour
{
    public static DialogueManager2 Instance; // Singleton instance

    public Text dialogueText;  // UI Text component for displaying dialogue
    public Text characterNameText;  // UI Text component for displaying NPC or plant names
    public Image characterImage;    // UI Image component for displaying NPC or plant images
    public GameObject dialogueBox;  // Dialogue box GameObject

    public CharacterController2DD characterController;

    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip dialogueStartSound; // Audio clip to play when the dialogue starts

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
            Debug.Log("DialogueManager instance created.");
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate
            Debug.Log("Duplicate DialogueManager instance destroyed.");
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

    public void StartNPCDialogue(NPCData npcData)
    {
        if (dialogueBox == null || characterNameText == null || characterImage == null)
        {
            Debug.LogError("One or more UI references not set in DialogueManager.");
            return;
        }

        PlayDialogueStartSound(); // Play sound when NPC dialogue starts

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

    public void StartPlantDialogue(PlantData plantData)
    {
        if (dialogueBox == null || characterNameText == null || characterImage == null)
        {
            Debug.LogError("One or more UI references not set in DialogueManager.");
            return;
        }

        PlayDialogueStartSound(); // Play sound when Plant dialogue starts

        dialogueBox.SetActive(true);
        sentences.Clear();
        characterNameText.text = plantData.plantName; // Set the plant name
        characterImage.sprite = plantData.plantImage; // Set the plant image

        foreach (string sentence in plantData.plantSentences)
        {
            sentences.Enqueue(sentence);
        }

        DisablePlayerMovement();
        DisplayNextSentence();
    }

    void PlayDialogueStartSound()
    {
        if (audioSource != null && dialogueStartSound != null)
        {
            audioSource.PlayOneShot(dialogueStartSound);
        }
        else
        {
            Debug.LogWarning("AudioSource or DialogueStartSound is not assigned.");
        }
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

            // Accelerate by decreasing the delay
            currentDelay = Mathf.Max(minimumDelay, currentDelay - accelerationFactor);
        }

        isTyping = false;

        // additional mouse click after the sentence is completed before proceeding.
        // used as a safety measure to not proceed to the next sentence  once the sentence is completed.
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
            Debug.Log("Player movement disabled.");
        }
    }

    void EnablePlayerMovement()
    {
        if (characterController != null)
        {
            characterController.ResumeMovement();
            Debug.Log("Player movement enabled.");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HideDialogueBox(); // Hide dialogue box when a new scene is loaded

        if (dialogueBox == null)
        {
            dialogueBox = GameObject.Find("DialogueBox");
            if (dialogueBox != null)
            {
                Debug.Log("DialogueBox reference reassigned.");
                dialogueText = dialogueBox.GetComponentInChildren<Text>();
                characterNameText = dialogueBox.GetComponentInChildren<Text>();
                characterImage = dialogueBox.GetComponentInChildren<Image>();
            }
            else
            {
                Debug.LogError("DialogueBox reference is missing after scene load.");
            }
        }
    }

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
