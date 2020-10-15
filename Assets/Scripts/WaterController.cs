using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class WaterController
{
    public static GameObject currentWaterSetInstance;

    struct WaterTransform
    {
        public Vector3 pos;
        public Vector3 scale;
    }

    private static WaterTransform generatedWaterTransform;

    public static void SetWater(Vector3 pos, Vector3 scale)
    {
        generatedWaterTransform.pos = pos;
        generatedWaterTransform.scale = scale;
    }

    public static (Vector3, Vector3) GetWater()
    {
        return (generatedWaterTransform.pos, generatedWaterTransform.scale);
    }
}
