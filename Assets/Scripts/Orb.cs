using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

public class Orb : MonoBehaviour
{
    public Game game;
    public Item item;
    public bool hover = false;

    public void Init(Game game, Item item)
    {
        this.game = game;
        this.item = item;

        Text text = transform.GetComponentInChildren<Text>();
        text.ThrowIfNull();
        text.text = item.number.ToString();
    }

    public void Hover()
    {
        hover = true;
        if (game.inventory.HasPassive(ItemType.Hammer))
            game.SetMessage("SPACE to smash this orb with the hammer");
        else game.SetMessage("This portal is encased in a hard orb. You need something to smash it");
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
            if (game.inventory.HasPassive(ItemType.Hammer))
            {
                game.SetMessage("A portal rises from the smashed orb.");
                game.level.ChangeOrbToPortal(this);
            }
        }
    }
}
