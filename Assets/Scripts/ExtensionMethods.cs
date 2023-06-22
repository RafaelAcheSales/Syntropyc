using System;
using System.ComponentModel.Design;
using UnityEditor;
using UnityEngine;
public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static int MapFloatToIntInterval(this float value, float from1, float to1, int from2, int to2)
    {
        return Mathf.FloorToInt((value - from1) / (to1 - from1) * (to2 - from2) + from2);
    }
    //creates editor action to print object to console
    [UnityEditor.MenuItem("Debug/PrintObject")]
    public static void DumpBranchToConsole()
    {
        var obj = Selection.activeObject as GameObject;
        if (obj == null || !obj.TryGetComponent(out Branch branch))
        {
            Debug.Log("No branch selected");
            return;
        }
        Debug.Log("BranchLightScore: " + branch.lightScore);
        //Debug.Log(branch.
    }
}