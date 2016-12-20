using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Sprite addTool;
    public Sprite subtractTool;
    public Sprite multiplyTool;
    public Sprite divideTool;
    public Sprite slotActive;
    public Sprite slotInactive;
    public Sprite slotPassive;
    public Sprite hammer;

    public List<InventorySlot> passiveSlots = new List<InventorySlot>();
    public List<InventorySlot> activeSlots = new List<InventorySlot>();
    int selected = -1; // Selected active slot
    int activeSlotsInUse = 0;

    public void Awake()
    {
        int i = 0;
        foreach (InventorySlot slot in activeSlots)
            slot.id = i++;
    }

    public void TakePassive(ItemType type)
    {
        InventorySlot slot = FindEmpty(passiveSlots);
        slot.Init(type, ToolType.None, 0);
    }

    public void TakeActive(ItemType type, ToolType op, int number)
    {
        InventorySlot slot = FindEmpty(activeSlots);
        slot.Init(type, op, number, selected == -1);
        selected = slot.id;
        activeSlotsInUse++;
        UpdateSlots();
    }

    public bool HasPassive(ItemType type)
    {
        foreach (InventorySlot slot in passiveSlots)
        {
            if (slot.itemType == type)
                return true;
        }
        return false;
    }

    public InventorySlot FindEmpty(List<InventorySlot> slots)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemType == ItemType.None)
                return slot;
        }
        throw new UnityException("Cannot find empty slot");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ChangeSelected(-1);
        else if (Input.GetKeyDown(KeyCode.E))
            ChangeSelected(1);
    }

    public void ChangeSelected(int offset)
    {
        if (activeSlotsInUse <= 0)
            selected = -1;

        else
        {
            selected -= offset;
            if (selected < 0)
                selected = activeSlots.Count - 1;
            else if (selected >= activeSlots.Count)
                selected = 0;
        }

        if (selected >= 0 && activeSlots[selected].itemType == ItemType.None)
        {
            ChangeSelected(offset);
            return;
        }

        UpdateSlots();
    }

    public void UpdateSlots()
    {
        foreach (InventorySlot slot in activeSlots)
            slot.Select(slot.id == selected);
    }

    public InventorySlot GetActiveSlot()
    {
        if (selected == -1)
            return null;
        return activeSlots[selected];
    }

    public void RemoveSlot()
    {
        activeSlotsInUse--;
        ChangeSelected(-1);
    }
}
