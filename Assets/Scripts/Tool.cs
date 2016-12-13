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
		switch(item.toolOperation)
		{
			case ToolOperation.Add:
				sr.sprite = game.level.adderSprite;
				break;
			case ToolOperation.Subtract:
				sr.sprite = game.level.subtracterSprite;
				break;
			case ToolOperation.Divide:
				sr.sprite = game.level.dividerSprite;
				break;
			case ToolOperation.Multiply:
				sr.sprite = game.level.multiplierSprite;
				break;
			default:
				throw new UnityException("Unknown tool type:" + item.toolOperation);		
		}
	}

	public string getToolType()
	{
		switch(item.toolOperation)
		{
			case ToolOperation.Add:
				return "Adder";
			case ToolOperation.Subtract:
				return "Subtracter";
			case ToolOperation.Divide:
				return "Divider";
			case ToolOperation.Multiply:
				return "Multiplier";			
		}
		throw new UnityException("Unknown tool type:" + item.toolOperation);
	}

	public void Hover()
	{
		Debug.Log(item.toolOperation + " HOVER!");
		hover = true;
		game.SetMessage("SPACE to pick up this Portal " + getToolType());
	}

	public void Unhover()
	{
		Debug.Log(item.toolOperation + " unhover...");
		hover = false;
		game.ClearMessage();
	}

	public void Update()
	{
		if(hover && Input.GetKeyDown(KeyCode.Space))
		{
			game.player.TakeTool(item.toolOperation);
			Object.Destroy(this.gameObject);
			game.SetMessage("You pick up the Portal " + getToolType() + ". A/D to select it, S to use it.");
		}
	}
}
