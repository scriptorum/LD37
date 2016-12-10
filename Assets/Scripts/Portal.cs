using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

public class Portal : MonoBehaviour
{
	public Item item;

	public void Awake()
	{
	}

	public void Init(LevelManager mgr, Item item)
	{
		this.item = item;

		// Reference some objects
		Text text = transform.GetComponentInChildren<Text>();
		text.ThrowIfNull();
		Transform portalNum = transform.FindChild("PortalNum");
		portalNum.ThrowIfNull();
		SpriteRenderer sr = transform.FindChild("PortalFrame").GetComponent<SpriteRenderer>();

		// Set portal appearance
		text.text = item.number.ToString();
		portalNum.gameObject.SetActive(item.portalType != PortalType.Closed);
		sr.sprite = (item.portalType == PortalType.Open ? mgr.portalOpen : (item.portalType == PortalType.Closed ? mgr.portalOpen : mgr.portalReturn));
	}
}
