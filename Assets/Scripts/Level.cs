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
	public GameObject playerPrefab;
	public Game game;

	public void Awake()
	{
		game.levelManager.ThrowIfNull();
		Clear();
	}

	public void Load(int levelNo)
	{
		priorLevel = number;
		number = levelNo;
		GameObject go;
		Clear();

		LevelData data = game.levelManager.Load(levelNo);
		foreach(Item item in data.items)
		{
			switch(item.type)
			{
				case ItemType.Portal:
					go = Create(portalPrefab);
					Portal portal = go.GetComponent<Portal>();
					portal.Init(game, item);
					if(item.portalType == PortalType.Return)
						game.player.ArriveAt(portal);
					break;

				case ItemType.Hammer:
					go = Create(hammerPrefab);
					break;

				case ItemType.Orb:
					go = Create(orbPrefab);
					Orb orb = go.GetComponent<Orb>();
					orb.Init(game.levelManager, item);
					break;

				default:
					throw new UnityException("Not implemented:" + item.type);
			}

			// Set parent and position
			go.transform.parent = transform;
			go.transform.localPosition = new Vector3(item.point.x, item.point.y, 0);
		}

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
		level.Load(level.number);
	}

	public void Reload()
	{
		if(game.levelManager.IsValidLevel(number))
		{
			Clear();
			Load(number);
			game.levelManager.SetImprint(number);
		}
		else Debug.Log("Not valid level:" + number);
	}

	public void OnValidate()
	{
		if(game.levelManager.number != number)
			Invoke("Reload", 0.25f);
	}
	#endif
}
