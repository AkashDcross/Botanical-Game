using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;
    private int npcsSaved = 0;
    private int incorrectPlantsGiven = 0;

    public GameObject scorePopupPrefab; // Reference to the ScorePopupPanel prefab
    public Transform playerTransform; // Reference to the player's transform
    public float popupHeightOffset = 1.5f; // Vertical offset for the popup above the player's head

    // UI TextMeshPro elements
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI npcsSavedText;
    public TextMeshProUGUI incorrectPlantsGivenText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize the UI elements with the current values
        UpdateScoreUI();
        UpdateNPCsSavedUI();
        UpdateIncorrectPlantsGivenUI();
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        npcsSaved++; // Increment NPCs saved count
        Debug.Log($"Score increased by {amount}. Current score: {score}, NPCs saved: {npcsSaved}");

        UpdateScoreUI();
        UpdateNPCsSavedUI();

        ShowScorePopup(amount);
    }

    public void IncrementIncorrectPlantsGiven()
    {
        incorrectPlantsGiven++;
        Debug.Log($"Incorrect plants given: {incorrectPlantsGiven}");

        UpdateIncorrectPlantsGivenUI();
    }

    private void ShowScorePopup(int scoreValue)
    {
        if (scorePopupPrefab != null && playerTransform != null)
        {
            // Instantiate the popup
            GameObject popupInstance = Instantiate(scorePopupPrefab, transform);

            // Set the position above the player's head
            Vector3 popupPosition = playerTransform.position;
            popupPosition.y += popupHeightOffset;

            // Convert world position to screen position
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(popupPosition);
            popupInstance.transform.position = screenPosition;

            // Set the text
            TextMeshProUGUI popupText = popupInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (popupText != null)
            {
                popupText.text = "+" + scoreValue.ToString();
            }
            else
            {
                Debug.LogWarning("No TextMeshProUGUI component found in the ScorePopupPrefab.");
            }

            // Start the coroutine to hide the panel after a delay
            StartCoroutine(HideScorePopup(popupInstance));
        }
        else
        {
            Debug.LogWarning("ScorePopupPrefab or PlayerTransform is not assigned in ScoreManager.");
        }
    }

    private IEnumerator HideScorePopup(GameObject popupInstance)
    {
        yield return new WaitForSeconds(1.5f); // Adjust duration as necessary
        Destroy(popupInstance);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    private void UpdateNPCsSavedUI()
    {
        if (npcsSavedText != null)
        {
            npcsSavedText.text =npcsSaved.ToString();
        }
    }

    private void UpdateIncorrectPlantsGivenUI()
    {
        if (incorrectPlantsGivenText != null)
        {
            incorrectPlantsGivenText.text =incorrectPlantsGiven.ToString();
        }
    }

    public int GetScore()
    {
        return score;
    }

    public int GetNPCsSaved()
    {
        return npcsSaved;
    }

    public int GetIncorrectPlantsGiven()
    {
        return incorrectPlantsGiven;
    }
}
