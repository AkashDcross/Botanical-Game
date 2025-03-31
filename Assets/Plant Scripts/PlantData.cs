using UnityEngine;

[CreateAssetMenu(fileName = "New Plant", menuName = "Character/Plant")]
public class PlantData : ScriptableObject
{
    public string plantName;  // The name of the plant
    public Sprite plantImage; // Image representing the plant
    public string[] plantSentences; // Array of sentences for the plant
    public string[] plantEffects;  // Effects or additional information about the plant
}
