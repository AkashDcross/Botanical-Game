using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlantDiscoveryManager : MonoBehaviour
{
    public static PlantDiscoveryManager Instance; // Singleton instance
    public PlantHotbarManager plantHotbarManager; // Reference to the SingleItemHotbarManager

    public GameObject discoveryDialogueBox; // The GameObject for the dialogue box
    public Text plantNameText; // Text component for plant name
    public Image plantImage; // Image component for plant image
    public AudioSource audioSource; // AudioSource component for playing sounds
    public AudioClip discoverySound; // AudioClip to play when a plant is discovered

    private HashSet<string> discoveredPlants; // To keep track of discovered plants
    private Queue<PlantData> discoveryQueue; // Queue to manage multiple discoveries
    private bool isDialogueActive; // Flag to track if the dialogue is currently active

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
            Debug.Log("PlantDiscoveryManager instance created.");
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate
            Debug.Log("Duplicate PlantDiscoveryManager instance destroyed.");
        }

        discoveredPlants = new HashSet<string>(); // Initialize the set
        discoveryQueue = new Queue<PlantData>(); // Initialize the queue
    }

    void Start()
    {
        if (discoveryDialogueBox != null)
        {
            discoveryDialogueBox.SetActive(false); // Ensure the dialogue box is hidden at the start
        }

        // Ensure audio source is not null
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component is missing from the PlantDiscoveryManager.");
            }
        }
    }

    public void OnPlantInteraction(PlantData plantData)
    {
        // Add the plant to the hotbar (or increment its count)
        if (plantHotbarManager != null)
        {
            plantHotbarManager.AddPlantToHotbar(plantData); // Corrected method name
        }

        // Check if the plant has been discovered already
        if (!discoveredPlants.Contains(plantData.plantName))
        {
            discoveredPlants.Add(plantData.plantName); // Mark as discovered
            discoveryQueue.Enqueue(plantData); // Queue the discovery
            JournalManager.Instance.AddPlantToJournal(plantData); // Notify JournalManager

            if (!isDialogueActive) // If no dialogue is active, start displaying the queue
            {
                StartCoroutine(ProcessDiscoveryQueue());
            }
        }
    }

    private IEnumerator ProcessDiscoveryQueue()
    {
        while (discoveryQueue.Count > 0)
        {
            isDialogueActive = true;
            PlantData plantData = discoveryQueue.Dequeue();

            // Show discovery dialogue
            ShowDiscoveryDialogue(plantData);

            yield return new WaitForSeconds(3f); // Adjust this for the dialogue duration
        }
        isDialogueActive = false;
    }

    void ShowDiscoveryDialogue(PlantData plantData)
    {
        if (discoveryDialogueBox == null || plantNameText == null || plantImage == null)
        {
            Debug.LogError("One or more UI references not set in PlantDiscoveryManager.");
            return;
        }

        plantNameText.text = plantData.plantName; // Set the plant name
        plantImage.sprite = plantData.plantImage; // Set the plant image

        discoveryDialogueBox.SetActive(true); // Show the dialogue box

        // Play the discovery sound
        if (audioSource != null && discoverySound != null)
        {
            audioSource.PlayOneShot(discoverySound);
        }

        // Hide the dialogue box after a delay
        StartCoroutine(HideDialogueBoxAfterDelay(3f));
    }

    IEnumerator HideDialogueBoxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (discoveryDialogueBox != null)
        {
            discoveryDialogueBox.SetActive(false); 
        }
    }
}
