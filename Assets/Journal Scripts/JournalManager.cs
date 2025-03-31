using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this namespace for TextMeshPro
using System.Collections.Generic;

public class JournalManager : MonoBehaviour
{
    public static JournalManager Instance; // Singleton instance

    public GameObject journalCanvas; // The Journal Canvas
    public Image plantImage; // UI Image component for plant image
    public Text plantNameText; // UI Text component for plant name
    public Text plantDataText; // UI Text component for plant data
    public Text medicinalBenefit1Text; // UI Text component for medicinal benefit 1
    public Text medicinalBenefit2Text; // UI Text component for medicinal benefit 2
    public Text medicinalBenefit3Text; // UI Text component for medicinal benefit 3
    public TextMeshProUGUI pageNumberText; // TextMeshProUGUI component for displaying the current page number

    public Button closeButton; // Button to close the journal
    public Button nextButton; // Button to go to the next plant
    public Button previousButton; // Button to go to the previous plant

    public AudioClip pageTurnSound; // The AudioClip for the page-turning sound
    private AudioSource audioSource; // The AudioSource to play the sound

    private List<PlantData> discoveredPlants = new List<PlantData>();
    private int currentPlantIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource component
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        journalCanvas.SetActive(false);
        closeButton.onClick.AddListener(CloseJournal);
        nextButton.onClick.AddListener(ShowNextPlant);
        previousButton.onClick.AddListener(ShowPreviousPlant);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJournal();
        }
    }

    void ToggleJournal()
    {
        if (journalCanvas != null)
        {
            journalCanvas.SetActive(!journalCanvas.activeSelf);
            if (journalCanvas.activeSelf)
            {
                UpdateJournalDisplay();
            }
        }
    }

    void UpdateJournalDisplay()
    {
        if (discoveredPlants.Count > 0)
        {
            PlantData currentPlant = discoveredPlants[currentPlantIndex];

            if (plantImage != null)
                plantImage.sprite = currentPlant.plantImage;

            if (plantNameText != null)
                plantNameText.text = currentPlant.plantName;

            if (plantDataText != null)
                plantDataText.text = string.Join("\n", currentPlant.plantSentences);

            if (medicinalBenefit1Text != null)
                medicinalBenefit1Text.text = currentPlant.plantEffects.Length > 0 ? currentPlant.plantEffects[0] : "";

            if (medicinalBenefit2Text != null)
                medicinalBenefit2Text.text = currentPlant.plantEffects.Length > 1 ? currentPlant.plantEffects[1] : "";

            if (medicinalBenefit3Text != null)
                medicinalBenefit3Text.text = currentPlant.plantEffects.Length > 2 ? currentPlant.plantEffects[2] : "";

            // undelining the page number using TextMeshPro rich text tags
            if (pageNumberText != null)
                pageNumberText.text = $"<u>Page {currentPlantIndex + 1} of {discoveredPlants.Count}</u>";
        }
        else
        {
            ShowNoPlantsMessage();
        }
    }


    void ShowNoPlantsMessage()
    {
        pageNumberText.text = "No plants discovered"; // Provide feedback when no plants are available
    }

    void CloseJournal()
    {
        journalCanvas.SetActive(false);
    }

    void ShowNextPlant()
    {
        if (discoveredPlants.Count > 0)
        {
            currentPlantIndex = (currentPlantIndex + 1) % discoveredPlants.Count;
            PlayPageTurnSound(); // Play sound on page turn
            UpdateJournalDisplay();
        }
    }

    void ShowPreviousPlant()
    {
        if (discoveredPlants.Count > 0)
        {
            currentPlantIndex = (currentPlantIndex - 1 + discoveredPlants.Count) % discoveredPlants.Count;
            PlayPageTurnSound(); // Play sound on page turn
            UpdateJournalDisplay();
        }
    }

    void PlayPageTurnSound()
    {
        if (audioSource != null && pageTurnSound != null && discoveredPlants.Count > 1)
        {
            audioSource.PlayOneShot(pageTurnSound);
        }
    }


    // Add a plant to the journal
    public void AddPlantToJournal(PlantData plantData)
    {
        if (!discoveredPlants.Contains(plantData))
        {
            discoveredPlants.Add(plantData);
            if (journalCanvas.activeSelf)
            {
                UpdateJournalDisplay(); // Update the display immediately if journal is open
            }
        }
    }
}
