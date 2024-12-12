using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public GameObject keyPrefab; // Reference to the key prefab
    public GameObject door;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collides with the key
        if (other.CompareTag("Player"))
        {
            // Mark the key as picked up
            DoorUnlock.IsKeyPickedUp = true; // Set the static variable to true
            Debug.Log("Key picked up! Now you can open the door.");

            // Destroy the key prefab after pickup
            Destroy(GameObject.Find("panel_door"));
            Destroy(gameObject);
        }
    }
}
