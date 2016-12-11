using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

public class Portal : MonoBehaviour
{
	public Game game;
	public Item item;

	public void Awake()
	{
		
	}

	public void Init(Game game, Item newItem)
	{
		this.item = newItem;
		this.game = game;

		// Reference some objects
		Text text = transform.GetComponentInChildren<Text>();
		text.ThrowIfNull();
		Transform portalNum = transform.FindChild("PortalNum");
		portalNum.ThrowIfNull();
		SpriteRenderer sr = transform.FindChild("PortalFrame").GetComponent<SpriteRenderer>();

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

	public void SetMessage()
	{
		switch(item.portalType)
		{
			case PortalType.Open:
				game.SetMessage("This portal leads to room " + item.number);
				break;
			case PortalType.Closed:
				game.SetMessage("This portal is currently closed!");
				break;
			case PortalType.Return:
				game.SetMessage("This is the back of the portal from room " + item.number);
				break;
		}
	}
}
