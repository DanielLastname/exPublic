using UnityEngine;

public class UI_basic : MonoBehaviour
{
    public InteractableController Interactor; // use to access player
    public PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Interactor = GetComponentInParent<InteractableController>();
        if (Interactor != null)
        {
            player = Interactor.player;
        }

        this.gameObject.SetActive(false);
    }

    public void closeWindow()
    {
        if (player != null)
        {
            player.InteruptPlayerController = false;
        }
        this.gameObject.SetActive(false);
    }
}
