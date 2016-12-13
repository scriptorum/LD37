using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public GameObject hammerInInventory;
	public GameObject adderInInventory;
	public GameObject subtracterInInventory;
	public GameObject dividerInInventory;
	public GameObject multiplierInInventory;
	public Game game;
	public float speed = 10.0f;
	public bool mustReleaseInputs = false;
	public bool hasHammer = false;
	public bool hasAdder = false;
	public bool hasSubtracter = false;
	public bool hasDivider = false;
	public bool hasMultiplier = false;

	public void Awake()
	{
		hammerInInventory.SetActive(false);
		adderInInventory.SetActive(false);
		subtracterInInventory.SetActive(false);
		dividerInInventory.SetActive(false);
		multiplierInInventory.SetActive(false);
	}

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

	public void TakeTool(ToolOperation toolOp)
	{
		switch(toolOp)
		{
			case ToolOperation.Add:
				hasAdder = true;
				adderInInventory.SetActive(true);
				break;
			case ToolOperation.Subtract:
				hasSubtracter = true;
				subtracterInInventory.SetActive(true);
				break;
			case ToolOperation.Divide:
				hasDivider = true;
				dividerInInventory.SetActive(true);
				break;
			case ToolOperation.Multiply:
				hasMultiplier = true;
				multiplierInInventory.SetActive(true);
				break;
			default:
				throw new UnityException("Unsupposed tool operation:" + toolOp);
		}
	}
}
