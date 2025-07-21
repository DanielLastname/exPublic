using UnityEngine;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    public InteractableController Interactor;
    public CharacterEquips Equips;

    // Lists for equipment categories
    public List<GameObject> characters = new List<GameObject>();
    public List<GameObject> tops = new List<GameObject>();
    public List<GameObject> leftHands = new List<GameObject>();
    public List<GameObject> rightHands = new List<GameObject>();
    public List<GameObject> extras = new List<GameObject>();

    // Indexes to track the current item
    private int characterIndex = 0;
    private int topIndex = 0;
    private int leftHandIndex = 0;
    private int rightHandIndex = 0;
    private int extraIndex = 0;

    void Start()
    {
        if (tops.Count > 1 && tops[1] != null)
            Debug.Log(tops[1].name);
        else
            Debug.LogWarning("Tops list does not contain a valid element at index 1.");

        Interactor = GetComponent<InteractableController>();
    }

    void PrepInteraction()
    {
        if (Interactor.player == null)
        {
            Debug.Log("No player registered to InteractableController.");
            return;
        }

        Equips = Interactor.player.GetComponentInChildren<CharacterEquips>();
        PlayerController PC = Interactor.player;
        PC.CurrentInteractable = Interactor;
    }

    public void TopButtonNext()
    {
        PrepInteraction();
        GameObject nextTop = CycleList(tops, ref topIndex, +1);
        if (nextTop != null)
        {
            Equips.ReplaceAndEquip(nextTop, Equips.Bod.transform);
            Debug.Log($"Equipped next top: {nextTop.name} (index {topIndex})");
        }
    }

    public void TopButtonBack()
    {
        PrepInteraction();
        GameObject prevTop = CycleList(tops, ref topIndex, -1);
        if (prevTop != null)
        {
            Equips.ReplaceAndEquip(prevTop, Equips.Bod.transform);
            Debug.Log($"Equipped previous top: {prevTop.name} (index {topIndex})");
        }
    }

    public void CharacterButtonNext()
    {
        PrepInteraction();
        GameObject nextChar = CycleList(characters, ref characterIndex, +1);
        if (nextChar != null)
        {
            Equips.ReplaceAndEquip(nextChar, Equips.Character.transform);
            Debug.Log($"Equipped next character: {nextChar.name} (index {characterIndex})");
        }
    }

    public void CharacterButtonBack()
    {
        PrepInteraction();
        GameObject prevChar = CycleList(characters, ref characterIndex, -1);
        if (prevChar != null)
        {
            Equips.ReplaceAndEquip(prevChar, Equips.Character.transform);
            Debug.Log($"Equipped previous character: {prevChar.name} (index {characterIndex})");
        }
    }

    public void LeftHandButtonNext()
    {
        PrepInteraction();
        GameObject nextLeft = CycleList(leftHands, ref leftHandIndex, +1);
        if (nextLeft != null)
        {
            Equips.ReplaceAndEquip(nextLeft, Equips.HandL.transform);
            Debug.Log($"Equipped next left hand: {nextLeft.name} (index {leftHandIndex})");
        }
    }

    public void LeftHandButtonBack()
    {
        PrepInteraction();
        GameObject prevLeft = CycleList(leftHands, ref leftHandIndex, -1);
        if (prevLeft != null)
        {
            Equips.ReplaceAndEquip(prevLeft, Equips.HandL.transform);
            Debug.Log($"Equipped previous left hand: {prevLeft.name} (index {leftHandIndex})");
        }
    }

    public void RightHandButtonNext()
    {
        PrepInteraction();
        GameObject nextRight = CycleList(rightHands, ref rightHandIndex, +1);
        if (nextRight != null)
        {
            Equips.ReplaceAndEquip(nextRight, Equips.HandR.transform);
            Debug.Log($"Equipped next right hand: {nextRight.name} (index {rightHandIndex})");
        }
    }

    public void RightHandButtonBack()
    {
        PrepInteraction();
        GameObject prevRight = CycleList(rightHands, ref rightHandIndex, -1);
        if (prevRight != null)
        {
            Equips.ReplaceAndEquip(prevRight, Equips.HandR.transform);
            Debug.Log($"Equipped previous right hand: {prevRight.name} (index {rightHandIndex})");
        }
    }

    public void ExtraButtonNext()
    {
        PrepInteraction();
        GameObject nextExtra = CycleList(extras, ref extraIndex, +1);
        if (nextExtra != null)
        {
            Equips.ReplaceAndEquip(nextExtra, Equips.Extra.transform);
            Debug.Log($"Equipped next extra: {nextExtra.name} (index {extraIndex})");
        }
    }

    public void ExtraButtonBack()
    {
        PrepInteraction();
        GameObject prevExtra = CycleList(extras, ref extraIndex, -1);
        if (prevExtra != null)
        {
            Equips.ReplaceAndEquip(prevExtra, Equips.Extra.transform);
            Debug.Log($"Equipped previous extra: {prevExtra.name} (index {extraIndex})");
        }
    }

    public T CycleList<T>(List<T> list, ref int index, int direction)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("CycleList called with null or empty list.");
            return default;
        }

        if (list.Count == 1)
        {
            index = 0;
            return list[0];
        }

        index += direction;

        if (index >= list.Count)
            index = 0;
        else if (index < 0)
            index = list.Count - 1;

        return list[index];
    }
}
