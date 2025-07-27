using UnityEngine;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    public Stats CharacterStats;
    public PlayerController Player;
    public InteractableController Interactor;
    public Stats PlayerStats;
    CharacterEquips Equips;


    // cached stats
    public float Hp = 1;
    public float Mp = 1;

    public float Str = 1;
    public float Def = 1;

    public float fire = 1;
    public float fireDef = 1;

    public float ice = 1;
    public float iceDef = 1;

    public float CritChance = 1;
    public float knockBack = 1;
    public float speed = 1;
    public float jump = 1;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI strText;
    public TextMeshProUGUI defText;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("triggered");
        
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Call method to suggest interaction and pass this interactable controller to the player
                Player = playerController;
                PrepInteraction(Player);
            }
            else
            {
                Debug.Log("no playercontroller found on interactable");
            }
        }
    }

    void Start()
    {
        var interactable = GetComponent<InteractableController>();
        if (interactable != null)
        {
            interactable.OnInteracted += HandleInteracted;
        }
    }


    private void OnDestroy()
    {
        // Clean up subscription
        if (Equips != null)
        {
            Equips.OnStatUpdate -= UpdateStats;
        }

        var interactable = GetComponent<InteractableController>();
        if (interactable != null)
        {
            interactable.OnInteracted -= HandleInteracted;
        }
    }

    private void HandleInteracted(PlayerController playerWhoInteracted)
    {
        Debug.Log("Interact event fired by: " + playerWhoInteracted.name);
        // You can call StatDisplay.PrepInteraction(playerWhoInteracted) here

        PrepInteraction(playerWhoInteracted);
    }

    /// <summary>
    /// Call this once when a player starts interacting with this object.
    /// For example, from InteractableController.OnInteract(player).
    /// </summary>
    public void PrepInteraction(PlayerController newPlayer)
    {
        if (newPlayer == null)
        {
            Debug.LogWarning("[StatDisplay] Tried to prep with null player.");
            return;
        }

        // If already subscribed to an old player, unsubscribe first
        if (Equips != null)
        {
            Equips.OnStatUpdate -= UpdateStats;
        }

        // Assign new player
        Player = newPlayer;
        Player.CurrentInteractable = Interactor;

        PlayerStats = Player.GetComponent<Stats>();
        Equips = Player.GetComponentInChildren<CharacterEquips>();

        if (Equips != null)
        {
            Equips.OnStatUpdate += UpdateStats;
            UpdateStats(); // initial pull
        }
        else
        {
            Debug.LogWarning("[StatDisplay] Player has no CharacterEquips!");
        }
    }

    private void UpdateStats()
    {
        if (PlayerStats == null)
        {
            Debug.LogWarning("[StatDisplay] UpdateStats called but PlayerStats is null!");
            return;
        }

        Hp = PlayerStats.Hp;
        Mp = PlayerStats.Mp;
        Str = PlayerStats.Str;
        Def = PlayerStats.Def;

        fire = PlayerStats.fire;
        fireDef = PlayerStats.fireDef;

        ice = PlayerStats.ice;
        iceDef = PlayerStats.iceDef;

        CritChance = PlayerStats.CritChance;
        knockBack = PlayerStats.knockBack;

        speed = PlayerStats.speed;
        jump = PlayerStats.jump;

        Debug.Log("[StatDisplay] Stats updated via event.");

        if (hpText != null) hpText.text = $"HP: {Hp}";
        if (mpText != null) mpText.text = $"MP: {Mp}";
        if (strText != null) strText.text = $"STR: {Str}";
        if (defText != null) defText.text = $"DEF: {Def}";
    }
}
