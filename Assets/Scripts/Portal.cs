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
    private Anim frameAnim;

    public void Init(Game game, Item newItem)
    {
        this.item = newItem;
        this.game = game;

        // Reference some objects
        portalNum = transform.FindChild("PortalNum");
        portalText = portalNum.FindChild("Canvas/Portal").GetComponentInChildren<Text>();
        toolText = portalNum.FindChild("Canvas/Tool").GetComponentInChildren<Text>();
        frameAnim = transform.FindChild("PortalFrame").GetComponent<Anim>();
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
                frameAnim.Play("open");
                break;
            case PortalType.Closed:
                frameAnim.Play("closed");
                break;
            case PortalType.Return:
                // Hide default return portal
                if (item.number == Level.NO_LEVEL && Application.isPlaying) gameObject.SetActive(false);
                else frameAnim.Play("return");
                break;
        }

        portalText.text = GetDestination().ToString();
        string aug = GetAugmentation();
        toolText.text = aug;
        portalText.color = (aug == "" ? Color.white : Color.black);
        portalNumSr.sprite = (aug == "" ? game.level.portalNumSprite : game.level.portalNumWithToolSprite);
    }

    public int GetDestination()
    {
        switch (item.installedTool)
        {
            case ToolType.Add:
                return item.number + item.installedNumber;
            case ToolType.Subtract:
                return item.number - item.installedNumber;
            case ToolType.Multiply:
                return item.number * item.installedNumber;
            case ToolType.Divide:
                return item.number / item.installedNumber;
            case ToolType.None:
                return item.number;
        }
        throw new UnityException("Unknown tool " + item.installedTool);
    }

    public string GetAugmentation()
    {
        switch (item.installedTool)
        {
            case ToolType.Add:
                return "+" + item.installedNumber.ToString();
            case ToolType.Subtract:
                return "-" + item.installedNumber.ToString();
            case ToolType.Multiply:
                return "*" + item.installedNumber.ToString();
            case ToolType.Divide:
                return "/" + item.installedNumber.ToString();
            case ToolType.None:
                return "";
        }
        throw new UnityException("Unknown tool " + item.installedTool);
    }

    public void Hover()
    {
        hover = true;
        switch (item.portalType)
        {
            case PortalType.Open:
                game.SetMessage("SPACE to travel through this portal to room " + GetDestination());
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

            game.TeleportTo(GetDestination(), item.portalType);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // If you are over a portal with an installed tool, pick it up
            if (item.portalType == PortalType.Open && item.installedTool != ToolType.None)
            {
                // Reclaim tool to inventory
                game.inventory.TakeActive(ItemType.Tool, item.installedTool, item.installedNumber);
                game.SetMessage("You reclaim the " + item.installedTool + item.installedNumber + " tool.");

                // Remove tool from portal
                item.installedTool = ToolType.None;
                item.installedNumber = Level.NO_LEVEL;
                game.levelManager.ChangeItem(game.level.number, item);
                UpdateView();
                return;
            }

            // All remaining usage of R key requires a selected tool in inventory
            InventorySlot slot = game.inventory.GetActiveSlot();
            if (slot == null)
                return;

            // Cannot use tool on return portal
            if (item.portalType == PortalType.Return)
            {
                game.SetMessage("Portal tools don't work on return portals.");
                return;
            }

            if (item.portalType == PortalType.Closed)
            {
                // EMPTY tool cannot be used on closed portal
                if (slot.number == Level.NO_LEVEL)
                    game.SetMessage("This portal is disabled and cannot be collected.");

                // Tool is loaded; used on closed portal will open it
                else
                {
                    item.portalType = PortalType.Open; // change to open portal
                    item.number = slot.number;  // using number from portal tool in inventory slot
                    game.levelManager.ChangeItem(game.level.number, item); // Remember changes
                    UpdateView(); // Change portal on display
                    slot.number = Level.NO_LEVEL; // Remove portal number from tool in inventory
                    slot.UpdateView(); // Update inventory display
                    game.SetMessage("You transfer the portal back to the arch.");
                }
                return;
            }

            // Portal is open and unaugmented
            Debug.Assert(item.portalType == PortalType.Open);
            Debug.Assert(item.installedTool == ToolType.None);

            // Is tool empty? If so, collect portal into tool and close portal
            if (slot.number == Level.NO_LEVEL)
            {
                slot.number = item.number;  // Copy portal number into tool
                slot.UpdateView();  // Update the inventory display
                item.portalType = PortalType.Closed; // Close portal
                game.levelManager.ChangeItem(game.level.number, item); // Remember change
                UpdateView(); // Update display of portal
                game.SetMessage("You collect portal " + item.number + " into the " + slot.toolType + " tool");
                return;
            }

            // Tool is full: install tool into the portal, if portal is empty
            if (slot.number != Level.NO_LEVEL)
            {
                // TODO Put "final" number in slot.number
                item.installedNumber = slot.number; // Copy tool number into portal
                item.installedTool = slot.toolType; // Copy tool type (add, subtract, etc) into portal
                game.levelManager.ChangeItem(game.level.number, item); // Remember changes
                UpdateView(); // Update display of portal to show augmentation
                slot.Remove(); // Remove inventory slot
                game.SetMessage("You augment the portal with the " + item.installedTool + " tool");
                return;
            }

            Debug.Log("You've discovered a bug. Er, I mean, easter egg.");
        }
    }
}
