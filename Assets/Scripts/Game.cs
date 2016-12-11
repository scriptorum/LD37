﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

public class Game : MonoBehaviour
{
	public Level level;
	public LevelManager levelManager;
	public Player player;

	public int startLevel;
	public float messageHold = 3.0f;
	public float messageFade = 1.0f;

	private Text messageBar;
	private Text messageShadow;
	private ActionQueue aq;

	public void Awake()
	{
		messageBar = GameObject.Find("MessageBar").GetComponent<Text>();
		messageShadow = GameObject.Find("MessageShadow").GetComponent<Text>();
		aq = gameObject.AddComponent<ActionQueue>();
	}

	public void Start()
	{
		SetMessage("Escape! Follow the portals to room ONE.");
		level.Load(startLevel);
	}

	public void ClearMessage()
	{
		SetMessage("");
	}

	public void SetMessage(string msg)
	{
		messageBar.text = msg;
		messageShadow.text = msg;

		Color startColor1 = messageBar.color;
		startColor1.a = 1;
		messageBar.color = startColor1;
		Color endColor1 = messageBar.color;
		endColor1.a = 0;

		Color startColor2 = messageShadow.color;
		startColor2.a = 1;
		messageShadow.color = startColor2;
		Color endColor2 = messageShadow.color;
		endColor2.a = 0;

		StopAllCoroutines();
		aq.Reset();
		aq.Add(() => gameObject.SetActive(true));
		aq.Delay(messageHold);
		aq.Add(() => StartCoroutine(startColor1.LerpColor(endColor1, messageFade, (Color c) => messageBar.color = c)));
		aq.Add(() => StartCoroutine(startColor2.LerpColor(endColor2, messageFade, (Color c) => messageShadow.color = c)));
		aq.Delay(messageFade);
		aq.Add(() => gameObject.SetActive(false));
		aq.Run();
	}

	public void TeleportTo(int targetLevel, PortalType portalType)
	{
		if(portalType == PortalType.Open)
			levelManager.SetReturnPortal(targetLevel, level.number);

		else if(portalType == PortalType.Return)
			levelManager.SetReturnPortal(level.number, Level.NO_LEVEL);

		level.Load(targetLevel);
	}
}