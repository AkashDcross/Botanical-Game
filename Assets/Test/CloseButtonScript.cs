using UnityEngine;

public class CloseButtonScript : MonoBehaviour
{
    public GameObject objectToToggle;

    public void ToggleObject()
    {
        if (objectToToggle != null)
        {
            objectToToggle.SetActive(!objectToToggle.activeSelf);
            Debug.Log("Object visibility toggled to: " + objectToToggle.activeSelf);
        }
        else
        {
            Debug.LogError("Object to toggle not assigned.");
        }
    }
}
