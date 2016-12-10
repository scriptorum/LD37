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
	public LevelManager levelManager;
	public int number;

	[HeaderAttribute("Prefabs")]
	public GameObject portalPrefab;
	public GameObject hammerPrefab;
	public GameObject orbPrefab;
	public GameObject playerPrefab;

	public void Awake()
	{
		levelManager.ThrowIfNull();
		Clear();
	}

	public void OnValidate()
	{
		if(levelManager.number != number) levelManager.SetImprint(number);
	}

	public void Load(int levelNo)
	{
		number = levelNo;
		GameObject go;
		Clear();

		LevelData data = levelManager.Load(levelNo);
		foreach(Item item in data.items)
		{
			switch(item.type)
			{
				case ItemType.Portal:
					go = Create(portalPrefab);
					Portal portal = go.GetComponent<Portal>();
					portal.Init(levelManager, item);
					break;

				case ItemType.Hammer:
					go = Create(hammerPrefab);
					break;

				case ItemType.Orb:
					go = Create(orbPrefab);
					Orb orb = go.GetComponent<Orb>();
					orb.Init(levelManager, item);
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
		if(Application.isPlaying)
			return Instantiate(prefab);
		else return PrefabUtility.InstantiatePrefab(prefab) as GameObject;
	}

	public void Clear()
	{
		if(Application.isPlaying) transform.DestroyChildren();
		else transform.DestroyChildrenImmediately();
	}

	#if UNITY_EDITOR
	[MenuItem("CONTEXT/Level/Write Level")]
	public static void WriteLevel(MenuCommand cmd)
	{
		Level level = (Level) cmd.context;
//		LevelManager mgr = level.levelManager;
		foreach(Transform child in level.transform)
		{
			Debug.Log("« over " + child.name);
		}
	}

	[MenuItem("CONTEXT/Level/Read Level")]
	public static void  ReadLevel(MenuCommand cmd)
	{
		Level level = (Level) cmd.context;		
		level.Load(level.number);
	}
	#endif
}
