using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public static bool HasKey = false; // Tracks if the player has the key

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HasKey = true; // Player has collected the key
            Debug.Log("Key collected!");
            Destroy(gameObject); // Destroy the key object
        }
    }
}