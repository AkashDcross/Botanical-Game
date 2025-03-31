using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlantIdentificationManager : MonoBehaviour
{
    [Header("Evidence Toggles")]
    public Toggle toggleRespiratoryAid;
    public Toggle toggleImmuneSupport;
    public Toggle toggleAntiviral;
    public Toggle togglePainRelief;

    public Toggle toggleAntimicrobial;
    public Toggle toggleCardiovascularHealth;
    public Toggle toggleDigestiveHealth;
    public Toggle toggleSoothing;

    public Toggle toggleAnxietyRelief;
    public Toggle toggleWomensHealth;
    public Toggle toggleMenstrualAid;
    public Toggle toggleAntimalarial;

    public List<PlantData> allPlantData; // Assign this in the Unity Inspector with all PlantData objects

    [Header("Plant Names")]
    public TextMeshProUGUI[] plantNameTexts;

    [Header("Evidence Manager")]
    public GameObject evidenceManager; // Drag the EvidenceManager GameObject here in the Inspector
    public Button closeButton; // Drag your CloseButton here in the Inspector

    private Dictionary<TextMeshProUGUI, PlantData> plantDataMapping = new Dictionary<TextMeshProUGUI, PlantData>();

    void Start()
    {
        // Initialize the mapping between the plant names (TextMeshProUGUI) and their corresponding PlantData
        foreach (var plantNameText in plantNameTexts)
        {
            PlantData correspondingPlantData = GetPlantDataByName(plantNameText.text);
            if (correspondingPlantData != null)
            {
                plantDataMapping[plantNameText] = correspondingPlantData;
            }
        }

        // Attach listeners to the toggles to update the display whenever they change
        toggleRespiratoryAid.onValueChanged.AddListener(delegate { UpdatePlantDisplay(); });
        toggleImmuneSupport.onValueChanged.AddListener(delegate { UpdatePlantDisplay(); });
        toggleAntiviral.onValueChanged.AddListener(delegate { UpdatePlantDisplay(); });
        togglePainRelief.onValueChanged.AddListener(delegate { UpdatePlantDisplay(); });

        toggleAntimicrobial.onValueChanged.AddListener(delegate { UpdatePlantDisplay(); });
        toggleCardiovascularHealth.onValueChanged.AddListener(delegate { UpdatePlantDisplay(); });
        toggleDigestiveHealth.onValueChanged.AddListener(delegate { UpdatePlantDisplay(); });
        toggleSoothing.onValueChanged.AddListener(delegate { UpdatePlantDisplay(); });

        toggleAnxietyRelief.onValueChanged.AddListener(delegate { UpdatePlantDisplay(); });
        toggleWomensHealth.onValueChanged.AddListener(delegate { UpdatePlantDisplay(); });
        toggleMenstrualAid.onValueChanged.AddListener(delegate { UpdatePlantDisplay(); });
        toggleAntimalarial.onValueChanged.AddListener(delegate { UpdatePlantDisplay(); });

        // Attach listener to the CloseButton to close the Evidence Manager
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseEvidenceManager);
        }
        else
        {
            Debug.LogError("Close button reference is not assigned.");
        }
    }

    void Update()
    {
        // Check for the "I" key press to toggle the Evidence Manager
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleEvidenceManager();
        }
    }

    void UpdatePlantDisplay()
    {
        foreach (var entry in plantDataMapping)
        {
            var plantText = entry.Key;
            var plantData = entry.Value;

            if (IsPlantEliminated(plantData))
            {
                // Adds strike-through and transparency
                plantText.text = $"<s>{plantData.plantName}</s>";
                plantText.color = new Color(plantText.color.r, plantText.color.g, plantText.color.b, 0.5f); // 50% transparency
            }
            else
            {
                // Remove strike-through and restore full opacity
                plantText.text = plantData.plantName;
                plantText.color = new Color(plantText.color.r, plantText.color.g, plantText.color.b, 1.0f); // 100% opacity
            }
        }
    }

    bool IsPlantEliminated(PlantData plantData)
    {
        bool eliminated = false;

        // Check each toggle and corresponding evidence
        if (toggleRespiratoryAid.isOn && !plantData.plantEffects.Contains("Respiratory Aid"))
            eliminated = true;
        if (toggleImmuneSupport.isOn && !plantData.plantEffects.Contains("Immune Support"))
            eliminated = true;
        if (toggleAntiviral.isOn && !plantData.plantEffects.Contains("Antiviral"))
            eliminated = true;
        if (togglePainRelief.isOn && !plantData.plantEffects.Contains("Pain Relief"))
            eliminated = true;

        if (toggleAntimicrobial.isOn && !plantData.plantEffects.Contains("Antimicrobial"))
            eliminated = true;
        if (toggleCardiovascularHealth.isOn && !plantData.plantEffects.Contains("Cardiovascular Health"))
            eliminated = true;
        if (toggleDigestiveHealth.isOn && !plantData.plantEffects.Contains("Digestive Health"))
            eliminated = true;
        if (toggleSoothing.isOn && !plantData.plantEffects.Contains("Soothing"))
            eliminated = true;

        if (toggleAnxietyRelief.isOn && !plantData.plantEffects.Contains("Anxiety Relief"))
            eliminated = true;
        if (toggleWomensHealth.isOn && !plantData.plantEffects.Contains("Women's Health"))
            eliminated = true;
        if (toggleMenstrualAid.isOn && !plantData.plantEffects.Contains("Menstrual Aid"))
            eliminated = true;
        if (toggleAntimalarial.isOn && !plantData.plantEffects.Contains("Antimalarial"))
            eliminated = true;

        return eliminated;
    }

    public PlantData GetPlantDataByName(string plantName)
    {
        if (allPlantData == null || allPlantData.Count == 0)
        {
            Debug.LogError("Plant data list is empty or not assigned.");
            return null;
        }

        PlantData plantData = allPlantData.FirstOrDefault(p => p.plantName.Equals(plantName, System.StringComparison.OrdinalIgnoreCase));

        if (plantData == null)
        {
            Debug.LogWarning($"No PlantData found for plant name: {plantName}");
        }

        return plantData;
    }

    public void CloseEvidenceManager()
    {
        if (evidenceManager != null)
        {
            evidenceManager.SetActive(false);
            Debug.Log("Evidence Manager closed.");
        }
        else
        {
            Debug.LogError("Evidence Manager reference is not assigned.");
        }
    }

    void ToggleEvidenceManager()
    {
        if (evidenceManager != null)
        {
            evidenceManager.SetActive(!evidenceManager.activeSelf);
        }
    }
}
