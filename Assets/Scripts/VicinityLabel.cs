using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spewnity;
using UnityEngine.Events;

public class VicinityLabel : MonoBehaviour
{
	public Game game;
	public string message; // message will be shown on enter and clear on exit if no events are set
	public UnityEvent onEnter; // Instead of setting message, will call this
	public UnityEvent onExit;  // Instead of clearing mesasge, will call this

	public void Awake()
	{
		game = GameObject.Find("/Game").GetComponent<Game>();
		game.ThrowIfNull();
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{
			if(onEnter.GetPersistentEventCount() == 0)
				game.SetMessage(message);
			else onEnter.Invoke();
		}
	}
		
	public void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{
			if(onExit.GetPersistentEventCount() == 0)
				game.ClearMessage();
			else onExit.Invoke();
		}
	}
}
