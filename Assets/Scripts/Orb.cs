using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

public class Orb : MonoBehaviour
{
	public Item item;

	public void Awake()
	{
	}

	public void Init(LevelManager mgr, Item item)
	{
		this.item = item;

		Text text = transform.GetComponentInChildren<Text>();
		text.ThrowIfNull();
		text.text = item.number.ToString();
	}
}
