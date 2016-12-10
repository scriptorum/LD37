using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float speed = 10.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		float speedx = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
		float speedy = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed;
		transform.Translate(speedx, speedy, 0);
	}
}
