using UnityEngine;
using UnityEngine.UI;
using System;

public class InteractableController : MonoBehaviour, IInteractable
{
    public GameObject InteractionUI;
    public PlayerController player;

    public event Action<PlayerController> OnInteracted;

    private void Start()
    {
        //InteractionUI = GetComponentInChildren<Canvas>().gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Call method to suggest interaction and pass this interactable controller to the player
                suggestInteraction(playerController);
                player = playerController;
            }
            else
            {
                Debug.Log("no playercontroller found on interactable");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Set CurrentInteractable to null when the player leaves the trigger
                player.CurrentInteractable = null;
                player = null;
            }
            else
            {
                Debug.Log("no playercontroller found exiting interactable");
            }
        }
    }

    private void suggestInteraction(PlayerController playerController)
    {
        // Set the player's CurrentInteractable to this interactable
        playerController.CurrentInteractable = this;
    }

    // Implement the Interact() method from IInteractable
    public void Interact()
    {
        // Interaction logic here (e.g., pick up, open door, etc.)
        if (InteractionUI != null)
        {
            // Toggle the visibility of the UI
            bool isActive = !InteractionUI.activeSelf;
            InteractionUI.SetActive(isActive);

            // Reactivate mouse cursor when UI is shown, and lock it when hidden
            if (isActive)
            {
                Cursor.lockState = CursorLockMode.None; // Unlock cursor
                Cursor.visible = true; // Make cursor visible
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked; // Lock cursor
                Cursor.visible = false; // Hide cursor
            }
        }

        OnInteracted?.Invoke(player);

    }
}
