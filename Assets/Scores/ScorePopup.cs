using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScorePopup : MonoBehaviour
{
    public Text scoreText;

    public void ShowPopup(int scoreValue)
    {
        if (scoreText != null)
        {
            scoreText.text = "+" + scoreValue.ToString();
            StartCoroutine(FadeAndDestroy());
        }
        else
        {
            Debug.LogError("ScoreText is not assigned in the ScorePopup script.");
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        float fadeDuration = 1.5f; // Adjust as necessary
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        Destroy(gameObject);
    }
}
