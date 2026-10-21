using TMPro;
using UnityEngine;

[System.Serializable]
public class ObjectiveEntry
{
    public NPC npc;
    public TMP_Text label;
}

public class ObjectivesController : MonoBehaviour
{
    [SerializeField] private ObjectiveEntry[] objectives;

    void OnEnable()
    {
        RefreshObjectives();
    }

    private void RefreshObjectives()
    {
        foreach (ObjectiveEntry entry in objectives)
        {
            if (entry.npc == null || entry.label == null) continue;

            if (entry.npc.IsQuestCompleted)
            {
                entry.label.fontStyle |= FontStyles.Strikethrough;
            }
            else
            {
                entry.label.fontStyle &= ~FontStyles.Strikethrough;
            }
        }
    }
}