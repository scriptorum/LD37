using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
	public Item item;
	public Game game;
	public bool hover = false;

	public void Init(Game game, Item item)
	{
		this.game = game;
		this.item = item;

		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
		switch(item.toolType)
		{
			case ToolType.Add:
				sr.sprite = game.level.adderSprite;
				break;
			case ToolType.Subtract:
				sr.sprite = game.level.subtracterSprite;
				break;
			case ToolType.Divide:
				sr.sprite = game.level.dividerSprite;
				break;
			case ToolType.Multiply:
				sr.sprite = game.level.multiplierSprite;
				break;
			default:
				throw new UnityException("Unknown tool type:" + item.toolType);		
		}
	}

	public string getToolType()
	{
		switch(item.toolType)
		{
			case ToolType.Add:
				return "Adder";
			case ToolType.Subtract:
				return "Subtracter";
			case ToolType.Divide:
				return "Divider";
			case ToolType.Multiply:
				return "Multiplier";			
		}
		throw new UnityException("Unknown tool type:" + item.toolType);
	}

	public void Hover()
	{
		// Debug.Log(item.toolOperation + " HOVER!");
		hover = true;
		game.SetMessage("SPACE to pick up this Portal " + getToolType());
	}

	public void Unhover()
	{
		// Debug.Log(item.toolOperation + " unhover...");
		hover = false;
		game.ClearMessage();
	}

	public void Update()
	{
		if(hover && Input.GetKeyDown(KeyCode.Space))
		{
			game.inventory.TakeActive(ItemType.Tool, item.toolType, Level.NO_LEVEL);
			item.type = ItemType.None;
			game.levelManager.ChangeItem(game.level.room, item);
			Object.Destroy(this.gameObject);
			game.SetMessage("You pick up the Portal " + getToolType() + ". Q/E to select it, R to use it.");
		}
	}
}
