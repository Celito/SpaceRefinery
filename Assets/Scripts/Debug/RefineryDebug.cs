using UnityEngine;
using System.Collections;

public class RefineryDebug 
{
    public static bool ShowBasicLogs
    {
        get { return PlayerPrefs.GetInt("ShowBasicLogs", 0) == 1; }
        set { PlayerPrefs.SetInt("ShowBasicLogs", value ? 1 : 0); }
    }

    public static bool ShowErrorLogs
    {
        get { return PlayerPrefs.GetInt("ShowErrorLogs", 1) == 1; }
        set { PlayerPrefs.SetInt("ShowErrorLogs", value ? 1 : 0); }
    }

    public static bool ShowProductsFlowLogs
    {
        get { return PlayerPrefs.GetInt("ShowProductsFlowLogs", 1) == 1; }
        set { PlayerPrefs.SetInt("ShowProductsFlowLogs", value ? 1 : 0); }
    }
}
