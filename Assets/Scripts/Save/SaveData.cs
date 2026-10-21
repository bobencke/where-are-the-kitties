using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public string mapBoundary;
    public List<InventorySaveData> hotbarSaveData;
    public List<ChestSaveData> chestSaveData;
    public List<NpcSaveData> npcSaveData;
}

[System.Serializable]
public class ChestSaveData
{
    public string chestID;
    public bool isOpened;
}

[System.Serializable]
public class NpcFlag
{
    public string key;
    public bool value;
}

[System.Serializable]
public class NpcSaveData
{
    public string npcID;
    public Vector3 position;
    public List<NpcFlag> flags;
}