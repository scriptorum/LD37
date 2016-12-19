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

	[HideInInspector] // last loaded number
	public int number = Level.NO_LEVEL;

	[HideInInspector]
	public bool cacheNeedsUpdate = false;

	public void Awake()
	{
		levelImprint.ThrowIfNull();
		UpdateCache();
	}

	public void SetReturnPortal(int onLevel, int toNumber)
	{
		LevelData data = levelCache[onLevel];
		for(int i = 0; i < data.items.Length; i++)
		{
			if(data.items[i].type == ItemType.Portal && data.items[i].portalType == PortalType.Return)
			{
				data.items[i].number = toNumber;
				return;
			}
		}
	}

	public bool IsValidLevel(int levelNo)
	{
		return levelCache.ContainsKey(levelNo);
	}

	public void OnValidate()
	{
		cacheNeedsUpdate = true;
		foreach(LevelData data in levels)
		{
			data.name = "Room " + data.number.ToString();
			int numReturns = 0;
			for(int i = 0; i < data.items.Length; i++)
			{
				data.items[i].name = data.items[i].type.ToString();
				if(data.items[i].type == ItemType.Portal) data.items[i].name += " " + data.items[i].portalType;
				if(data.items[i].type == ItemType.Tool) data.items[i].name += " " + data.items[i].toolOperation;
				data.items[i].name += " " + data.items[i].number;
				if(data.items[i].type == ItemType.Portal && data.items[i].portalType == PortalType.Return) numReturns++;
			}
			if(numReturns != 1) Debug.Log("WARNING: Level " + data.number + " has " + numReturns + " return portals!");
		}
	}

	public void UpdateCache()
	{
		levelCache = new Dictionary<int,LevelData>();
		foreach(LevelData level in levels) levelCache[level.number] = level;
		cacheNeedsUpdate = false;
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
		
	// Changes the item in question
	// Places this item at the point on on the level "number" it contains
	public void ChangeItem(int roomNo, Item item)
	{
		if(!levelCache.ContainsKey(roomNo))
			throw new UnityException("LevelManager cannot find room " + roomNo);

		LevelData data = levelCache[roomNo];
		for(int i = 0; i < data.items.Length; i++)
		{
			if(data.items[i].point == item.point)
			{
				data.items[i] = item;
				return;
			}
		}

		throw new UnityException("LevelManager cannot find item at " + item.point + " in room " + roomNo);
	}
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
	[HideInInspector]
	public string name;

	public ItemType type;
	public int number;
	public Point point;

	[HeaderAttribute("If this is a tool")]
	public ToolOperation toolOperation;

	[HeaderAttribute("If this is a portal")]
	public PortalType portalType;
	public ToolOperation installedTool;
	public int installedNumber;
}

public enum ItemType
{
	None,
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
	None,
	Open,
	Closed,
	Return
}