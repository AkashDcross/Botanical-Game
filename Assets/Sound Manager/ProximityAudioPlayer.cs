using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityAudioPlayer : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the AudioSource component

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play(); // Play the audio when the player enters the range
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // Stop the audio when the player leaves the range
            }
        }
    }
}

