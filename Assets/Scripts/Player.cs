using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public GameObject hammerInInventory;
	public Game game;
	public float speed = 10.0f;
	public bool mustReleaseInputs = false;
	public bool hasHammer = false;
	
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

	public void ArriveAt(Portal portal)
	{
		transform.position = portal.transform.position;
		mustReleaseInputs = true;
	}

	public void TakeHammer()
	{
		hasHammer = true;
		hammerInInventory.SetActive(true);
	}
}
