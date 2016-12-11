using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spewnity;
using UnityEngine.Events;

public class VicinityLabel : MonoBehaviour
{
	public Game game;
	public string message;
	public UnityEvent onTrigger; // Instead of setting message will call this

	public void Awake()
	{
		game = GameObject.Find("/Game").GetComponent<Game>();
		game.ThrowIfNull();
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{
			if(onTrigger.GetPersistentEventCount() == 0)
				game.SetMessage(message);
			else onTrigger.Invoke();
		}
	}
}
