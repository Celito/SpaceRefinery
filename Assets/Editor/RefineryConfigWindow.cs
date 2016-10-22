using UnityEngine;
using UnityEditor;

public class RefineryConfigWindow : EditorWindow
{
    [MenuItem("Window/Refinery Config Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(RefineryConfigWindow), false, "Ooka Dev Options");
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        RefineryDebug.ShowBasicLogs = GUILayout.Toggle(RefineryDebug.ShowBasicLogs, "Show Basic Logs");
        RefineryDebug.ShowErrorLogs = GUILayout.Toggle(RefineryDebug.ShowErrorLogs, "Show Error Logs");
        RefineryDebug.ShowProductsFlowLogs = GUILayout.Toggle(RefineryDebug.ShowProductsFlowLogs, "Show Procts Flow Logs");
    }
}
