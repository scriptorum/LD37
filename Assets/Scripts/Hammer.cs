using System.Collections.Generic; 
using System.Collections; 
using UnityEngine; 

public class Hammer: MonoBehaviour
{
	public Game game; 
	public Item item;
	public bool hover = false; 

	public void Init(Game game, Item item)
	{
		this.game = game; 
		this.item = item;
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
			// Add item to inventory
			game.inventory.TakePassive(ItemType.Hammer);
			game.SetMessage("You pick up the orb smasher"); 

			// Remove item from display
			Object.Destroy(this.gameObject); 

			// Remove item from level manager
			item.type = ItemType.None;
			game.levelManager.ChangeItem(game.level.number, item);
		}
	}
}
