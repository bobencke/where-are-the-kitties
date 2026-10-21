using System.Collections.Generic;
using UnityEngine;

public class PositionHistoryRecorder : MonoBehaviour
{
    public static PositionHistoryRecorder Instance;

    [SerializeField] private float recordSpacing = 0.1f;
    [SerializeField] private int maxRecords = 500;

    private List<Vector3> positionHistory = new List<Vector3>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        positionHistory.Add(transform.position);
    }

    void Update()
    {
        if (PauseController.IsGamePaused || LoadingScreenController.IsTransitioning)
        {
            return;
        }

        if (Vector3.Distance(transform.position, positionHistory[0]) >= recordSpacing)
        {
            positionHistory.Insert(0, transform.position);

            if (positionHistory.Count > maxRecords)
            {
                positionHistory.RemoveAt(positionHistory.Count - 1);
            }
        }
    }

    public Vector3 GetPositionAtIndex(int index)
    {
        if (positionHistory.Count == 0)
        {
            return transform.position;
        }

        index = Mathf.Clamp(index, 0, positionHistory.Count - 1);
        return positionHistory[index];
    }

    public void ResetHistory(Vector3 newPosition)
    {
        positionHistory.Clear();

        for (int i = 0; i < maxRecords; i++)
        {
            positionHistory.Add(newPosition);
        }
    }
}