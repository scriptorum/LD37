using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

public class Portal : MonoBehaviour
{
    public Game game;
    public Item item;
    public bool hover = false;

    private Text portalText;
    private Text toolText;
    private Transform portalNum;
    private SpriteRenderer portalNumSr;
    private SpriteRenderer frameSR;

    public void Init(Game game, Item newItem)
    {
        this.item = newItem;
        this.game = game;

        // Reference some objects
        portalNum = transform.FindChild("PortalNum");
        portalText = portalNum.FindChild("Canvas/Portal").GetComponentInChildren<Text>();
        toolText = portalNum.FindChild("Canvas/Tool").GetComponentInChildren<Text>();
        frameSR = transform.FindChild("PortalFrame").GetComponent<SpriteRenderer>();
        portalNumSr = portalNum.GetComponent<SpriteRenderer>();

        UpdateView();
    }

    public void UpdateView()
    {
        // Set portal appearance
        portalNum.gameObject.SetActive(item.portalType != PortalType.Closed);
        switch (item.portalType)
        {
            case PortalType.Open:
                frameSR.sprite = game.levelManager.portalOpen;
                break;
            case PortalType.Closed:
                frameSR.sprite = game.levelManager.portalClosed;
                break;
            case PortalType.Return:
                // Hide default return portal
                if (item.number == Level.NO_LEVEL && Application.isPlaying) gameObject.SetActive(false);
                else frameSR.sprite = game.levelManager.portalReturn;
                break;
        }

        int finalNumber = item.number;
        string toolMsg = "";
        switch (item.installedTool)
        {
            case ToolOperation.Add:
                finalNumber += item.installedNumber;
                toolMsg = "+" + item.installedNumber.ToString();
                break;
            case ToolOperation.Subtract:
                finalNumber -= item.installedNumber;
                toolMsg = "-" + item.installedNumber.ToString();
                break;
            case ToolOperation.Multiply:
                finalNumber *= item.installedNumber;
                toolMsg = "*" + item.installedNumber.ToString();
                break;
            case ToolOperation.Divide:
                finalNumber /= item.installedNumber;
                toolMsg = "/" + item.installedNumber.ToString();
                break;
        }
        portalText.text = item.number.ToString();
        toolText.text = toolMsg;
        portalText.color = (toolMsg == "" ? Color.white : Color.black);
        portalNumSr.sprite = (toolMsg == "" ? game.level.portalNumSprite : game.level.portalNumWithToolSprite);
    }

    public void Hover()
    {
        hover = true;
        switch (item.portalType)
        {
            case PortalType.Open:
                game.SetMessage("SPACE to travel through this portal to room " + item.number);
                break;
            case PortalType.Closed:
                game.SetMessage("This portal is currently closed!");
                break;
            case PortalType.Return:
                game.SetMessage("SPACE to travel back to room " + item.number);
                break;
        }
    }

    public void Unhover()
    {
        hover = false;
        game.ClearMessage();
    }

    public void Update()
    {
        if (!hover)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (item.portalType == PortalType.Closed)
            {
                game.SetMessage("You walk into the portal and hurt yourself.");
                return;
            }

            game.TeleportTo(item.number, item.portalType);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            InventorySlot slot = game.inventory.GetActiveSlot();

            if (slot == null)
                return;

            if (item.portalType == PortalType.Return)
            {
                game.SetMessage("Portal tools don't work on return portals.");
                return;
            }

            if (slot.number != Level.NO_LEVEL)
            {
                if (item.portalType == PortalType.Closed)
                {
                    // Restore portal to open
					item.portalType = PortalType.Open; // change to open portal
					item.number = slot.number;	// using number from portal tool in inventory slot
		            game.levelManager.ChangeItem(game.level.number, item); // update level manager item
					UpdateView(); // Change portal on display
					slot.number = Level.NO_LEVEL; // Remove portal number from tool in inventory
					slot.Redraw(); // Update inventory display
                }

                else if (item.portalType == PortalType.Open)
				{
					// Swap tool with open portal that has tool
					
					// Install tool into open portal

				}

                return;
            }

            if (item.portalType == PortalType.Closed)
            {
                game.SetMessage("This portal is disabled and cannot be collected.");
                return;
            }

            slot.number = item.number;
            slot.Redraw();
            game.SetMessage("You collect portal " + item.number + " into the " + slot.op + " tool");

            // Change this portal's type to closed
            item.portalType = PortalType.Closed;

            // Ensure the level manager remembers this change
            game.levelManager.ChangeItem(game.level.number, item);

            // Display it as closed
            UpdateView();
        }
    }
}
