using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    public Animator DoorAnimator; // Animator for the door
    public bool IsLocked = true; // Is the door initially locked?
    private Collider doorCollider;
    private bool hasPlayedOpeningAnimation = false; // Prevents animation from playing multiple times

    private void Start()
    {
        // Get the door's collider
        doorCollider = GetComponent<Collider>();
        if (doorCollider == null)
        {
            Debug.LogError("No collider found on the door!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player interacts with the door
        if (other.CompareTag("Player"))
        {
            // If the door is locked and the player doesn't have the key
            if (IsLocked && !KeyPickup.HasKey)
            {
                Debug.Log("Door is locked. Find the key!");
                return; // Exit without opening the door
            }

            // If the door is unlocked and the animation hasn't played
            if (!IsLocked && !hasPlayedOpeningAnimation)
            {
                OpenDoor();
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

        // Disable the collider to allow passage
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
    }
}