using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    public Character character; // initialising the appropariate character for the dialogue

    [TextArea(3, 10)]
    public string[] sentences;
}