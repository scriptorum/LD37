using System.Collections.Generic; 
using System.Collections; 
using UnityEngine; 

public class Hammer: MonoBehaviour
{
	public Game game; 
	public bool hover = false; 

	public void Init(Game game)
	{
		this.game = game; 
	}

	public void Hover()
	{
		hover = true; 
		game.SetMessage("SPACE to pick up this orb smasher"); 
	}

	public void Unhover()
	{
		hover = false; 
		game.ClearMessage(); 
	}

	public void Update()
	{
		if (hover && Input.GetKeyDown(KeyCode.Space))
		{
			game.inventory.TakePassive(ItemType.Hammer);
			Object.Destroy(this.gameObject); 
			game.SetMessage("You pick up the orb smasher"); 
		}
	}
}
