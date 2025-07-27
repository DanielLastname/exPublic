using UnityEngine;
using System;
using System.Collections;

public class CharacterEquips : MonoBehaviour
{
    public Stats CharacterStats;
    public PlayerController Player;
    
    public GameObject HandR;
    public GameObject HandL;
    public GameObject Bod;
    public GameObject Character;
    public GameObject Extra;

    public GameObject defaultHandR;
    public GameObject defaultHandL;
    public GameObject defaultBod;

    public event Action OnStatUpdate;

    void Awake()
    {
        // Instantiate and parent default items to corresponding empty GameObjects
        if (defaultHandR != null && HandR != null)
        {
            GameObject handRInstance = Instantiate(defaultHandR, HandR.transform);
            handRInstance.transform.localPosition = Vector3.zero;
            handRInstance.transform.localRotation = Quaternion.identity;
        }

        if (defaultHandL != null && HandL != null)
        {
            GameObject handLInstance = Instantiate(defaultHandL, HandL.transform);
            handLInstance.transform.localPosition = Vector3.zero;
            handLInstance.transform.localRotation = Quaternion.identity;
        }

        if (defaultBod != null && Bod != null)
        {
            GameObject bodInstance = Instantiate(defaultBod, Bod.transform);
            bodInstance.transform.localPosition = Vector3.zero;
            bodInstance.transform.localRotation = Quaternion.identity;
        }

        Player = GetComponent<PlayerController>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (defaultHandR != null && HandR != null)
        {
            ReplaceAndEquip(defaultHandR, HandR.transform);
        }

        if (defaultHandL != null && HandL != null)
        {
            ReplaceAndEquip(defaultHandL, HandL.transform);
        }

        if (defaultBod != null && Bod != null)
        {
            ReplaceAndEquip(defaultBod, Bod.transform);
        }

    }


    // Update is called once per frame
    void Update()
    {
        
    }
    void StatEquip(Stats eq)
    {
        CharacterStats.Hp += eq.Hp;
        CharacterStats.Mp += eq.Mp;
        CharacterStats.Str += eq.Str;
        CharacterStats.Def += eq.Def;

        CharacterStats.fire += eq.fire;
        CharacterStats.fireDef += eq.fireDef;

        CharacterStats.ice += eq.ice;
        CharacterStats.iceDef += eq.iceDef;

        CharacterStats.CritChance += eq.CritChance;
        CharacterStats.knockBack += eq.knockBack;

        if (eq.isSword == true)Player.AttackType = eq.attackType;
        ChangePlayerAttack();

        OnStatUpdate?.Invoke();
    }

    void StatRemove(Stats eq)
    {
        CharacterStats.Hp -= eq.Hp;
        CharacterStats.Mp -= eq.Mp;
        CharacterStats.Str -= eq.Str;
        CharacterStats.Def -= eq.Def;

        CharacterStats.fire -= eq.fire;
        CharacterStats.fireDef -= eq.fireDef;

        CharacterStats.ice -= eq.ice;
        CharacterStats.iceDef -= eq.iceDef;

        CharacterStats.CritChance -= eq.CritChance;
        CharacterStats.knockBack -= eq.knockBack;

        Player.currentAttack = null;

        OnStatUpdate?.Invoke();
    }

    void ChangePlayerAttack()
    {
        try
        {
            var DT = GetComponentInChildren<DamageTrigger>();
            Player.currentAttack = DT.gameObject;
            Player.currentAttack.SetActive(false); // in case the game object was left on
        }
        catch
        {
            Debug.Log("current atk = null");
        }
    }

    public void ReplaceAndEquip(GameObject GO, Transform parent)
    {
        Player.InteruptPlayerController = true;
        
        if (GO == null)
        {
            //Debug.LogError("ReplaceAndEquip FAILED: GO (new equipment prefab) is null.");
            return;
        }

        if (parent == null)
        {
            //Debug.LogError("ReplaceAndEquip FAILED: parent is null.");
            return;
        }

        //Debug.Log($"[ReplaceAndEquip] Attempting to equip '{GO.name}' to parent '{parent.name}'.");

        // Step 1: Check existing children for Stats component
        Stats oldStats = null;
        GameObject oldObject = null;

        foreach (Transform child in parent)
        {
            if (child == null)
            {
                //Debug.LogWarning($"[ReplaceAndEquip] Found null child under '{parent.name}'");
                continue;
            }

            oldStats = child.GetComponentInChildren<Stats>();
            if (oldStats != null)
            {
                oldObject = child.gameObject;
                //Debug.Log($"[ReplaceAndEquip] Found existing equipped object '{oldObject.name}' with Stats. Preparing to remove.");
                break;
            }
        }

        // Step 2: Remove stats and destroy old object if found
        if (oldStats != null && oldObject != null)
        {
            //Debug.Log($"[ReplaceAndEquip] Removing stats from '{oldObject.name}'.");
            StatRemove(oldStats);
            Destroy(oldObject);
        }
        else
        {
            //Debug.Log($"[ReplaceAndEquip] No existing equipment with Stats found under '{parent.name}'.");
        }

        // Step 3: Instantiate new object
        GameObject newInstance = Instantiate(GO, parent);
        if (newInstance == null)
        {
            //Debug.LogError("[ReplaceAndEquip] Instantiate FAILED. newInstance is null.");
            return;
        }

        newInstance.transform.localPosition = Vector3.zero;
        newInstance.transform.localRotation = Quaternion.identity;
        //Debug.Log($"[ReplaceAndEquip] Instantiated new equipment '{newInstance.name}' under '{parent.name}'.");

        // Step 4: Equip stats from new object
        Stats newStats = newInstance.GetComponentInChildren<Stats>();
        if (newStats != null)
        {
            //Debug.Log($"[ReplaceAndEquip] Applying stats from '{newInstance.name}'.");
            StatEquip(newStats);
        }
        else
        {
            //Debug.LogWarning($"[ReplaceAndEquip] No Stats found on newly instantiated '{newInstance.name}'. Nothing to apply.");
        }

        StartCoroutine(DelayedAnimatorRefresh());
    }

    private IEnumerator DelayedAnimatorRefresh()
    {
        // Wait until end of frame so Destroy() is processed
        yield return new WaitForEndOfFrame();

        if (Player != null)
        {
            Player.RefreshAnimators();
            //Debug.Log("[CharacterEquips] Animator refresh completed.");
        }

        Player.InteruptPlayerController = false;
        /*
        yield return new WaitForSeconds(0.5f);
        Player.StrikeUp();
        */
    }


}
