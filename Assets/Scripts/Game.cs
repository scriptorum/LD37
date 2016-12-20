using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.UI; 
using Spewnity; 

public class Game:MonoBehaviour {
	public Level level; 
	public Inventory inventory;
	public LevelManager levelManager; 
	public Player player; 

	public int startLevel; 
	public float messageHold = 3.0f; 
	public float messageFade = 1.0f; 

	private Text messageBar; 
	private Text messageShadow; 
	private ActionQueue aq; 

	public void Awake() {
		messageBar = GameObject.Find("MessageBar").GetComponent < Text > (); 
		messageShadow = GameObject.Find("MessageShadow").GetComponent < Text > (); 
		aq = gameObject.AddComponent < ActionQueue > (); 
	}

	public void Start() {
		
		SetMessage("Escape! Follow the portals to ROOM ONE."); 
		level.Load(startLevel, false, startLevel); 
	}

	public void ClearMessage() {
		SetMessage(""); 
	}

	public void SetMessage(string msg) {
		messageBar.text = msg; 
		messageShadow.text = msg; 

		Color startColor1 = messageBar.color; 
		startColor1.a = 1; 
		messageBar.color = startColor1; 
		Color endColor1 = messageBar.color; 
		endColor1.a = 0; 

		Color startColor2 = messageShadow.color; 
		startColor2.a = 1; 
		messageShadow.color = startColor2; 
		Color endColor2 = messageShadow.color; 
		endColor2.a = 0; 

		messageBar.gameObject.SetActive(true); 
		messageShadow.gameObject.SetActive(true); 

		aq.Reset(); 
		aq.Delay(messageHold); 
		aq.AddCoroutine(startColor1.LerpColor(endColor1, messageFade, (Color c) => messageBar.color = c)); 
		aq.AddCoroutine(startColor2.LerpColor(endColor2, messageFade, (Color c) => messageShadow.color = c)); 
		aq.Delay(messageFade); 
		aq.Add(() => messageBar.gameObject.SetActive(false)); 
		aq.Add(() => messageShadow.gameObject.SetActive(false)); 
		aq.Run(); 
	}

	public void TeleportTo(int destination, PortalType portalType) {
		if ( ! levelManager.IsValidLevel(destination)) {
			// TODO Go to procedural level based on number
			SetMessage("No. You can see monsters through that portal."); 
			return; 
		}
		
		if (portalType == PortalType.Open) {
			levelManager.SetReturnPortal(destination, level.number); 
			level.Load(destination, false, destination); 
		}
		else if (portalType == PortalType.Return) {
//			levelManager.SetReturnPortal(level.number, Level.NO_LEVEL);
			level.Load(destination, true, level.number); 
		}
	}
}
