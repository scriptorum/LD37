using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Game game;
	public float speed = 10.0f;
	public bool mustReleaseInputs = false;
	
	// Update is called once per frame
	void Update()
	{
		float speedx = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
		float speedy = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed;

		if(mustReleaseInputs)
		{
			if(speedx == 0 && speedy == 0) mustReleaseInputs = false;
			else return;
		}
		
		transform.Translate(speedx, speedy, 0);
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Portal")
		{
			Portal portal = other.gameObject.GetComponent<Portal>();

			if(portal.item.portalType == PortalType.Closed) return;

			Debug.Log("Teleporting to " + portal.item.number);
			game.TeleportTo(portal.item.number, portal.item.portalType);
		}
	}

	public void ArriveAt(Portal portal)
	{
		Transform target = portal.transform.FindChild("Arrival");
		transform.position = target.position;
		mustReleaseInputs = true;
	}
}
