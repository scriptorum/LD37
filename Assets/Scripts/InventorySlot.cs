using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

public class InventorySlot : MonoBehaviour
{
	public int id;
    private Inventory inventory;
    private Text text;
    private SpriteRenderer tool;
    private SpriteRenderer slot;

    public ItemType itemType = ItemType.None;
    public ToolType toolType = ToolType.None;
    public int number = Level.NO_LEVEL;
    public bool selectable = false;
    public bool selected = false;

    public void Awake()
    {
        inventory = transform.parent.GetComponent<Inventory>();
        text = gameObject.GetComponentInChildren<Text>();
        tool = transform.Find("Item").GetComponent<SpriteRenderer>();
        slot = GetComponent<SpriteRenderer>();

        inventory.ThrowIfNull();
        text.ThrowIfNull();
        tool.ThrowIfNull();
        slot.ThrowIfNull();

        UpdateView();
    }

    public void Init(ItemType type, ToolType op, int number, bool selected = false)
    {
        this.itemType = type;
        this.toolType = op;
        this.number = number;
        this.selected = selected;
        UpdateView();
    }

	public void Select(bool enabled)
	{
		selected = enabled;
		UpdateView();
	}

    public void UpdateView()
    {
        if (itemType == ItemType.None)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        switch (itemType)
        {
            case ItemType.Hammer:
                selectable = false;
                tool.sprite = inventory.hammer;
                text.text = "";
                break;

            case ItemType.Tool:
                selectable = true;
                switch (toolType)
                {
                    case ToolType.Add:
                        tool.sprite = inventory.addTool;
                        break;
                    case ToolType.Subtract:
                        tool.sprite = inventory.subtractTool;
                        break;
                    case ToolType.Divide:
                        tool.sprite = inventory.divideTool;
                        break;
                    case ToolType.Multiply:
                        tool.sprite = inventory.multiplyTool;
                        break;
                }
                text.text = number == Level.NO_LEVEL ? "?" : number.ToString();
                break;

            default:
                throw new UnityException("Unsupported item type:" + itemType);
        }

        if (selectable)
            slot.sprite = selected ? inventory.slotActive : inventory.slotInactive;
        else slot.sprite = inventory.slotPassive;
    }

    // Removes the inventory slot
    public void Remove()
    {
        // Remove slot
        itemType = ItemType.None;         

        // Notify inventory
        inventory.RemoveSlot();

        // Update view
        UpdateView();
        
    }
}
