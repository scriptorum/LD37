using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Spewnity;

public class LevelManager : MonoBehaviour
{
	public Text levelImprint;
	public LevelDefinition[] standardLevels;
	public LevelDefinition[] dynamicLevels;
	public LevelDefinition defaultLevel;
	public int dynamicRangeMin;

	[HideInInspector] // last loaded number
	public int room = Level.NO_LEVEL;

	public void Awake()
	{
		levelImprint.ThrowIfNull();
	}

	public LevelDefinition GetLevelDefinition(int levelNo)
	{
		// Look for standard level
		foreach(LevelDefinition data in standardLevels)
			if(data.room == levelNo)
				return data;
		
		// Provide dynamic level
		int min = dynamicRangeMin;
		foreach(LevelDefinition data in dynamicLevels)
		{
			if(levelNo >= min && levelNo <= data.room)
				return data;

			min = data.room + 1;			
		}

		return defaultLevel;
	}

	public void SetReturnPortal(int onLevel, int toNumber)
	{
		LevelDefinition data = GetLevelDefinition(onLevel);
		for(int i = 0; i < data.items.Length; i++)
		{
			if(data.items[i].type == ItemType.Portal && data.items[i].portalType == PortalType.Return)
			{
				data.items[i].number = toNumber;
				return;
			}
		}
	}

	public void OnValidate()
	{
		foreach(LevelDefinition data in standardLevels)
			ValidateLevel(data);
		foreach(LevelDefinition data in dynamicLevels)
			ValidateLevel(data);
		ValidateLevel(defaultLevel);
	}

	private void ValidateLevel(LevelDefinition data)
	{	
		data.name = "Room " + data.room.ToString();
		int numReturns = 0;
		for(int i = 0; i < data.items.Length; i++)
		{
			data.items[i].name = data.items[i].type.ToString();
			if(data.items[i].type == ItemType.Portal) data.items[i].name += " " + data.items[i].portalType;
			if(data.items[i].type == ItemType.Tool) data.items[i].name += " " + data.items[i].toolType;
			data.items[i].name += " " + data.items[i].number;
			if(data.items[i].type == ItemType.Portal && data.items[i].portalType == PortalType.Return) numReturns++;
		}
		if(numReturns != 1) Debug.Log("WARNING: Level " + data.room + " has " + numReturns + " return portals!");
	}

	public void SetImprint(int level)
	{
		room = level;
		levelImprint.text = level.ToString();
	}

	public LevelDefinition Load(int level)
	{
		SetImprint(level);
		return GetLevelDefinition(level);
	}
		
	// Changes the item in question
	// Places this item at the point on on the level "number" it contains
	public void ChangeItem(int roomNo, Item item)
	{
		LevelDefinition data = GetLevelDefinition(roomNo);
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
public class LevelDefinition
{
	[HideInInspector]
	public string name;

	public int room;
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
	public ToolType toolType;

	[HeaderAttribute("If this is a portal")]
	public PortalType portalType;
	public ToolType installedTool;
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
	Spawner,
	Map
}

public enum ToolType
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
