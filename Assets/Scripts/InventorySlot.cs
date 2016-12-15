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

    public ItemType type = ItemType.None;
    public ToolOperation op = ToolOperation.None;
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

        Reconfigure();
    }

    public void Init(ItemType type, ToolOperation op, int number, bool selected = false)
    {
        this.type = type;
        this.op = op;
        this.number = number;
        this.selected = selected;
        Reconfigure();
    }

	public void Select(bool enabled)
	{
		selected = enabled;
		Reconfigure();
	}

    private void Reconfigure()
    {
        if (type == ItemType.None)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        switch (type)
        {
            case ItemType.Hammer:
                selectable = false;
                tool.sprite = inventory.hammer;
                text.text = "";
                break;

            case ItemType.Tool:
                selectable = true;
                switch (op)
                {
                    case ToolOperation.Add:
                        tool.sprite = inventory.addTool;
                        break;
                    case ToolOperation.Subtract:
                        tool.sprite = inventory.subtractTool;
                        break;
                    case ToolOperation.Divide:
                        tool.sprite = inventory.divideTool;
                        break;
                    case ToolOperation.Multiply:
                        tool.sprite = inventory.multiplyTool;
                        break;
                }
                text.text = number == Level.NO_LEVEL ? "?" : number.ToString();
                break;

            default:
                throw new UnityException("Unsupported item type:" + type);
        }

        if (selectable)
            slot.sprite = selected ? inventory.slotActive : inventory.slotInactive;
        else slot.sprite = inventory.slotPassive;
    }
}
