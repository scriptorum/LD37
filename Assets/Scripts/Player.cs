using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Game game;
	public float speed = 10.0f;
	
	// Update is called once per frame
	void Update () 
	{
		float speedx = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
		float speedy = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed;
		transform.Translate(speedx, speedy, 0);
	}

	public void TeleportTo(int level, PortalType portalType)
	{
		if(portalType == PortalType.Open)
			game.levelManager.SetReturnPortal(level, game.level.number);

		else if(portalType == PortalType.Return)
			game.levelManager.SetReturnPortal(game.level.number, Level.NO_LEVEL);
		
		game.level.Load(level);
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Portal")
		{
			Portal portal = other.gameObject.GetComponent<Portal>();

			if(portal.item.portalType == PortalType.Closed)
				return;

			Debug.Log("Teleporting to " + portal.item.number);
			TeleportTo(portal.item.number, portal.item.portalType);
		}
	}
}
