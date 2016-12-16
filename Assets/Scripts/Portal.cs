using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

public class Portal : MonoBehaviour
{
	public Game game;
	public Item item;
	public bool hover = false;

	private Text text;
	private Transform portalNum;
	private SpriteRenderer sr;

	public void Init(Game game, Item newItem)
	{
		this.item = newItem;
		this.game = game;

		// Reference some objects
		text = transform.GetComponentInChildren<Text>();
		text.ThrowIfNull();
		portalNum = transform.FindChild("PortalNum");
		portalNum.ThrowIfNull();
		sr = transform.FindChild("PortalFrame").GetComponent<SpriteRenderer>();

		UpdateView();
	}

	public void UpdateView()
	{
		// Set portal appearance
		portalNum.gameObject.SetActive(item.portalType != PortalType.Closed);
		switch(item.portalType)
		{
			case PortalType.Open:
				sr.sprite = game.levelManager.portalOpen;
				break;
			case PortalType.Closed:
				sr.sprite = game.levelManager.portalClosed;
				break;
			case PortalType.Return:
				// Hide default return portal
				if(item.number == Level.NO_LEVEL && Application.isPlaying) gameObject.SetActive(false);
				else sr.sprite = game.levelManager.portalReturn;
				break;
		}

		text.text = item.number.ToString();
	}

	public void Hover()
	{
		hover = true;
		switch(item.portalType)
		{
			case PortalType.Open:
				game.SetMessage("SPACE to travel through this portal to room " + item.number);
				break;
			case PortalType.Closed:
				game.SetMessage("This portal is currently closed!");
				break;
			case PortalType.Return:
				game.SetMessage("SPACE to travel back to room " + item.number);
				break;
		}
	}

	public void Unhover()
	{
		hover = false;
		game.ClearMessage();
	}

	public void Update()
	{
		if(!hover)
			return;

		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(item.portalType == PortalType.Closed) 
			{
				game.SetMessage("You walk into the portal and hurt yourself.");
				return;
			}

			game.TeleportTo(item.number, item.portalType);
		}

		if(Input.GetKeyDown(KeyCode.R))
		{
			InventorySlot slot = game.inventory.GetActiveSlot();

			if(slot == null)
				return;

			if(slot.number != Level.NO_LEVEL)
			{
				game.SetMessage("Installing portal tools not implemented");
				return;
			}

			if(item.portalType == PortalType.Closed)
			{
				game.SetMessage("This portal is disabled and cannot be collected.");
				return;
			}

			if(item.portalType == PortalType.Return)
			{
				game.SetMessage("Portal tools don't work on return portals.");
				return;
			}

			slot.number = item.number;
			slot.Redraw();
			game.SetMessage("You collect portal " + item.number + " into the " + slot.op +  " tool");

			// Change this portal's type to closed
			item.portalType = PortalType.Closed;

			// Ensure the level manager remembers this change
			game.levelManager.ChangeItem(game.level.number, item);

			// Display it as closed
			UpdateView();
		}
	}
}
