using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTransition : MonoBehaviour
{
    [SerializeField] PolygonCollider2D mapBoundry;
    CinemachineConfiner2D confiner;
    [SerializeField] Direction direction;
    [SerializeField] Transform teleportTargetPosition;
    [SerializeField] float addPos = 2f;
    [SerializeField] bool useLoadingScreen = true;

    [Header("Trigger de Final de Jogo")]
    [SerializeField] private bool isEndingTrigger = false;
    [SerializeField] private NPC[] questNpcs;
    [SerializeField] private string endingCutsceneScene = "FinalCutscene";

    enum Direction { Up, Down, Left, Right, Teleport }

    public static event Action<Vector3> OnPlayerAreaTransition;

    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (isEndingTrigger && AllQuestsCompleted())
        {
            TriggerEnding();
            return;
        }

        GameObject player = collision.gameObject;

        if (useLoadingScreen && LoadingScreenController.Instance != null)
        {
            LoadingScreenController.Instance.RunTransition(() => DoTransition(player));
        }
        else
        {
            DoTransition(player);
        }
    }

    private bool AllQuestsCompleted()
    {
        if (questNpcs == null || questNpcs.Length == 0)
        {
            return false;
        }

        foreach (NPC npc in questNpcs)
        {
            if (npc == null || !npc.IsQuestCompleted)
            {
                return false;
            }
        }
        return true;
    }

    private void TriggerEnding()
    {
        if (LoadingScreenController.Instance != null)
        {
            LoadingScreenController.Instance.LoadSceneWithTransition(endingCutsceneScene);
        }
        else
        {
            SceneManager.LoadScene(endingCutsceneScene);
        }
    }

    private void DoTransition(GameObject player)
    {
        confiner.BoundingShape2D = mapBoundry;

        Vector3 newPos = ComputeNewPosition(player);
        player.transform.position = newPos;

        PositionHistoryRecorder.Instance?.ResetHistory(newPos);

        OnPlayerAreaTransition?.Invoke(newPos);
    }

    private Vector3 ComputeNewPosition(GameObject player)
    {
        if (direction == Direction.Teleport)
        {
            return teleportTargetPosition.position;
        }

        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.Up: newPos.y += addPos; break;
            case Direction.Down: newPos.y -= addPos; break;
            case Direction.Left: newPos.x -= addPos; break;
            case Direction.Right: newPos.x += addPos; break;
        }

        return newPos;
    }
}