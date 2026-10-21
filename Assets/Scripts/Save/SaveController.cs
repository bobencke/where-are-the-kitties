using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using System;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private HotBarController hotBarController;
    private Chest[] chests;
    private NPC[] npcs;

    public static event Action OnGameLoaded;

    void Start()
    {
        InitializeComponents();
        LoadGame();
    }

    private void InitializeComponents()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        hotBarController = FindFirstObjectByType<HotBarController>();
        chests = FindObjectsByType<Chest>(FindObjectsSortMode.None);
        npcs = FindObjectsByType<NPC>(FindObjectsSortMode.None);
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundary = FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D.gameObject.name,
            hotbarSaveData = hotBarController.GetHotbarItems(),
            chestSaveData = GetChestsState(),
            npcSaveData = GetNpcStates()
        };
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
        Debug.Log(saveLocation);
    }

    private List<ChestSaveData> GetChestsState()
    {
        List<ChestSaveData> chestStates = new List<ChestSaveData>();

        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = new ChestSaveData
            {
                chestID = chest.ChestID,
                isOpened = chest.IsOpened
            };
            chestStates.Add(chestSaveData);
        }

        return chestStates;
    }

    private List<NpcSaveData> GetNpcStates()
    {
        List<NpcSaveData> npcStates = new List<NpcSaveData>();

        foreach (NPC npc in npcs)
        {
            npcStates.Add(new NpcSaveData
            {
                npcID = npc.NpcID,
                position = npc.GetSavePosition(),
                flags = npc.GetSaveFlags()
            });
        }

        return npcStates;
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
            PositionHistoryRecorder.Instance?.ResetHistory(saveData.playerPosition);

            FindFirstObjectByType<CinemachineConfiner2D>()
                .BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();

            hotBarController.SetHotbarItems(saveData.hotbarSaveData);

            LoadChestStates(saveData.chestSaveData);
            LoadNpcStates(saveData.npcSaveData);
        }
        else
        {
            SaveGame();

            hotBarController.SetHotbarItems(new List<InventorySaveData>());
        }

        OnGameLoaded?.Invoke();
    }

    private void LoadChestStates(List<ChestSaveData> chestStates)
    {
        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = chestStates.FirstOrDefault(c => c.chestID == chest.ChestID);

            if (chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.isOpened);
            }
        }
    }

    private void LoadNpcStates(List<NpcSaveData> npcStates)
    {
        if (npcStates == null) return;

        foreach (NPC npc in npcs)
        {
            NpcSaveData npcSaveData = npcStates.FirstOrDefault(n => n.npcID == npc.NpcID);

            if (npcSaveData != null)
            {
                npc.ApplySavePosition(npcSaveData.position);
                npc.ApplySaveFlags(npcSaveData.flags);
            }
        }
    }
}