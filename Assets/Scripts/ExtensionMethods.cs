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
}