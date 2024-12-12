using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    public Animator DoorAnimator; // Animator for the door
    public bool IsLocked = true; // Is the door initially locked?
    private bool hasPlayedOpeningAnimation = false; // Prevents animation from playing multiple times
    private Collider doorCollider; // Reference to the door's collider

    public static bool IsKeyPickedUp = false; // Static variable to track if the player has picked up the key
    private Collider playerCollider; // Reference to the player's collider

    private void Start()
    {
        // Get the door's collider
        doorCollider = GetComponent<Collider>();

        if (doorCollider == null)
        {
            Debug.LogError("No collider found on the door!");
        }

        // Get the player's collider (assuming the player has a collider)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player interacts with the door
        if (other.CompareTag("Player"))
        {
            // If the door is locked and the player doesn't have the key
            if (IsLocked && !IsKeyPickedUp)
            {
                Debug.Log("Door is locked. Find the key!");
                // Prevent the player from passing through by disabling the door's collider
                if (doorCollider != null)
                {
                    doorCollider.enabled = true; // Keep the door collider active for blocking the player
                    doorCollider.isTrigger = false;
                }
                return; // Exit without opening the door
            }
            doorCollider.isTrigger = true;

            // If the door is unlocked and the animation hasn't played yet
            if (!IsLocked && !hasPlayedOpeningAnimation)
            {
                OpenDoor();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player exits the door's trigger zone
        if (other.CompareTag("Player"))
        {
            // Re-enable the door's collider once the player leaves the door area
            if (doorCollider != null)
            {
                doorCollider.enabled = true;
                doorCollider.isTrigger = true;// Reactivate the door's collider when player leaves
            }
        }
    }

    private void OpenDoor()
    {
        hasPlayedOpeningAnimation = true; // Mark the animation as played
        Debug.Log("Door is opening!");

        // Trigger the door opening animation
        if (DoorAnimator != null)
        {
            DoorAnimator.SetTrigger("OpenDoor");
        }

        // Disable the door collider to allow passage (if key is picked up)
        if (doorCollider != null)
        {
            doorCollider.enabled = false; // Disable the door collider once the door is unlocked
        }
    }
}
