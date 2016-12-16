using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Level : MonoBehaviour
{
	public static int NO_LEVEL = -99999;

	public int number = NO_LEVEL;
	public int priorLevel = NO_LEVEL;

	[HeaderAttribute("Prefabs")]
	public GameObject portalPrefab;
	public GameObject hammerPrefab;
	public GameObject orbPrefab;
	public GameObject toolPrefab;
	public GameObject playerPrefab;
	public Sprite adderSprite;
	public Sprite subtracterSprite;
	public Sprite multiplierSprite;
	public Sprite dividerSprite;
	public Game game;

	public void Awake()
	{
		game.levelManager.ThrowIfNull();
		Clear();
	}

	public void Load(int levelNo, bool returning, int portalNo)
	{
		priorLevel = number;
		number = levelNo;
		GameObject go;
		Clear();

		LevelData data = game.levelManager.Load(levelNo);
		Portal arrivalPortal = null;

		foreach(Item item in data.items)
		{
			switch(item.type)
			{
				case ItemType.Portal:
					go = Create(portalPrefab);
					Portal portal = go.GetComponent<Portal>();
					portal.Init(game, item);

					if(returning) // returning to sending portal 
					{
						if(item.portalType == PortalType.Open && item.number == portalNo) arrivalPortal = portal;
					}
					else // arriving at return portal
					{
						if(item.portalType == PortalType.Return) arrivalPortal = portal;
					}

					break;

				case ItemType.Hammer:
					go = Create(hammerPrefab);
					go.GetComponent<Hammer>().Init(game);
					break;

				case ItemType.Orb:
					go = Create(orbPrefab);
					Orb orb = go.GetComponent<Orb>();
					orb.Init(game, item);
					break;

				case ItemType.Tool:
					go = Create(toolPrefab);
					Tool tool = go.GetComponent<Tool>();
					tool.Init(game, item);
					break;

				default:
					throw new UnityException("Not implemented:" + item.type);
			}

			// Set parent and position
			go.transform.parent = transform;
			go.transform.localPosition = new Vector3(item.point.x, item.point.y, 0);
		}

		arrivalPortal.ThrowIfNull();
		game.player.ArriveAt(arrivalPortal);
	}

	public void ChangeOrbToPortal(Orb orb)
	{
		// Change LevelManager item from orb to portal so it respawns correctly when returning to this room
		orb.item.type = ItemType.Portal;
		orb.item.portalType = PortalType.Open;
		game.levelManager.ChangeItem(number, orb.item);

		// Create Portal
		GameObject go = Create(portalPrefab);
		go.transform.parent = orb.transform.parent;
		go.transform.position = orb.transform.position;
		go.GetComponent<Portal>().Init(game, orb.item);

		// Destroy Orb
		Object.Destroy(orb.gameObject);
	}

	private GameObject Create(GameObject prefab)
	{
		if(Application.isPlaying) return Instantiate(prefab);
		else return PrefabUtility.InstantiatePrefab(prefab) as GameObject;
	}

	public void Clear()
	{
		if(Application.isPlaying) transform.DestroyChildren();
		else transform.DestroyChildrenImmediately();
	}

	#if UNITY_EDITOR
	[MenuItem("CONTEXT/Level/Read Level")]
	public static void  ReadLevel(MenuCommand cmd)
	{
		Level level = (Level) cmd.context;		
		level.Load(level.number, false, 0);
	}

	public void OnValidate()
	{
		if(game.levelManager.number != number) Invoke("Reload", 0.25f);
	}
	#endif
}
