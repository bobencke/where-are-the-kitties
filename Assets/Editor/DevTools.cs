#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class DevTools
{
    [MenuItem("Dev/Reset Cutscene Flag")]
    static void ResetCutsceneFlag()
    {
        PlayerPrefs.DeleteKey("InitialCutscene");
        Debug.Log("Flag resetada!");
    }
}
#endif