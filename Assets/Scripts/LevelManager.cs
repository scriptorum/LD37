using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

public class LevelManager : MonoBehaviour
{
	public Text levelImprint;
	public LevelData[] levels;
	public Dictionary<int,LevelData> levelCache = new Dictionary<int,LevelData>();

	[HeaderAttribute("Prefab refs")]
	public Sprite portalOpen;
	public Sprite portalClosed;
	public Sprite portalReturn;

	[HideInInspector]
	public int number = -99999; // last loaded number
	[HideInInspector]
	public bool cacheNeedsUpdate = false;

	public void Awake()
	{
		levelImprint.ThrowIfNull();
		UpdateCache();
	}

	public void OnValidate()
	{
		cacheNeedsUpdate = true;
	}

	public void UpdateCache()
	{
		levelCache = new Dictionary<int,LevelData>();
		foreach(LevelData level in levels)
			levelCache[level.number] = level;
	}

	public void SetImprint(int level)
	{
		number = level;
		levelImprint.text = level.ToString();
	}

	public LevelData Load(int level)
	{
		if(cacheNeedsUpdate) UpdateCache();
		
		SetImprint(level);

		if(levelCache.ContainsKey(level)) return levelCache[level];

		throw new UnityException("Cannot load level " + level);
	}

//	public void Save()
//	{
//		if(levelCache.ContainsKey(level.number)) throw new UnityException("Found duplicate level map number:" + level.number);
//	}
}

[System.Serializable]
public class LevelData
{
	[HideInInspector]
	public string name;

	public int number;
	public Item[] items;
}

[System.Serializable]
public struct Item
{
	public ItemType type;
	public int number;
	public  Point point;

	[HeaderAttribute("Not applicable to all items")]
	public ToolOperation toolOperation;
	public bool toolNumberIsDynamic;
	public PortalType portalType;
}

public enum ItemType
{
	Portal,
	Orb,
	Hammer,
	Tool,
	Slingshot,
	FreezeTurret,
	BigGun,
	SpeedBoots,
	Teleporter,
	Map
}

public enum ToolOperation
{
	None,
	Add,
	Subtract,
	Multiply,
	Divide
}

public enum PortalType
{
	Open,
	Closed,
	Return
}