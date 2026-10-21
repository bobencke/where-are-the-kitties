using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerStatsController : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text catsSavedText;
    [SerializeField] private TMP_Text itemsSavedText;
    [SerializeField] private NPC[] questNpcs;

    private HotBarController hotBarController;

    void Awake()
    {
        hotBarController = FindFirstObjectByType<HotBarController>();
    }

    void OnEnable()
    {
        RefreshStats();
    }

    private void RefreshStats()
    {
        int catsSaved = questNpcs.Count(npc => npc != null && npc.IsQuestCompleted);
        catsSavedText.SetText(catsSaved.ToString());

        int itemsSaved = hotBarController != null ? hotBarController.GetTotalItemCount() : 0;

        itemsSavedText.SetText(itemsSaved > 0 ? itemsSaved.ToString() : "Nenhum");
    }
}