using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TerrainGenerator : MonoBehaviour
{
    public Texture2D hMap;

    public GameObject water;

    public float[,] originalHeights;
    public float[,] modifiedHeights;

    public Vector3 tDataSizeOriginal;

    public float sizeScaleModifier;
    public float pointScalingModifier;
    public float pointDirectIncreaseModifier;
    public float maximumHeightModifier;

    public Material grey;
    public Material wireframe;

    private bool wireframeBool = false;

    public bool mapLoaded = false;

    TerrainData tData;
    void Start()
    {
        tData = this.GetComponent<Terrain>().terrainData;
        TerrainController.generatedTerrainData = tData;
        originalHeights = new float[1, 1];
        modifiedHeights = new float[1, 1];
        tData.size = new Vector3(100, 5f, 100);

        GetComponent<Terrain>().enabled = false;
        water.GetComponent<MeshRenderer>().enabled = false;
    }

    Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }

    public void GenerateHeightMap(Texture2D hMapIn)
    {
        mapLoaded = true;

        var pow2width = Mathf.ClosestPowerOfTwo(hMapIn.width);
        var pow2height = Mathf.ClosestPowerOfTwo(hMapIn.height);
        ////
        //hMapIn.Resize(pow2width, pow2height);
        //hMapIn.Apply();

        this.hMap = Resize(hMapIn, pow2width, pow2height);
        //this.hMap = hMapIn;

        originalHeights = new float[hMap.width, hMap.height];
        modifiedHeights = new float[hMap.width, hMap.height];

        tData.size = new Vector3(hMap.width, 1f, hMap.height);

        for (int x = 0; x < hMap.width; x++)
        {
            for (int y = 0; y < hMap.height; y++)
            {
                originalHeights[x, y] = hMap.GetPixel(y, x).grayscale;
                modifiedHeights[x, y] = originalHeights[x, y] * pointScalingModifier + Increase(originalHeights[x, y], pointDirectIncreaseModifier);
            }
        }

        tData.heightmapResolution = Mathf.Max(hMap.height, hMap.width) + 1;
        tData.baseMapResolution = Mathf.Max(hMap.height, hMap.width) + 1;

        tData.SetHeights(0, 0, modifiedHeights);

        //tData.size = new Vector3(hMap.width, 1f, hMap.height);

        tDataSizeOriginal = new Vector3(hMap.width, 1, hMap.height);

        //tData.size = new Vector3(tDataSizeOriginal.x * sizeModifier, tData.size.y, tDataSizeOriginal.z * sizeModifier);

        //tData.size = new Vector3(tData.size.x, heightModifier, tData.size.z);

        tData.size = new Vector3(tDataSizeOriginal.x * sizeScaleModifier, maximumHeightModifier, tDataSizeOriginal.z * sizeScaleModifier);

        CorrigateWater();

        GetComponent<Terrain>().enabled = true;
        water.GetComponent<MeshRenderer>().enabled = true;
    }

    /// <summary>
    /// Only rendering, not working for random terraindata sampling, USELESS, font use
    /// </summary>
    public void CutUnderWaterTerain()
    {
        return;
        float[,] heights = tData.GetHeights(0, 0, hMap.width, hMap.height);
        bool[,] holes = new bool[hMap.width, hMap.height];

        for (int i = 0; i < hMap.width; i++)
        {
            for (int j = 0; j < hMap.height; j++)
            {
                if (heights[i, j] < water.transform.position.y / 30f)
                {
                    holes[i, j] = false;
                }
                else
                {
                    holes[i, j] = true;
                }
            }
        }

        tData.SetHoles(0, 0, holes);

        //tData.SetHeights(0, 0, newheights);

    }

    /// <summary>
    /// Only rendering, not working for random terraindata sampling, USELESS, dont use
    /// </summary>
    public void DisableUnderWaterCut()
    {
        return;
        float[,] heights = tData.GetHeights(0, 0, hMap.width, hMap.height);
        bool[,] holes = new bool[hMap.width, hMap.height];

        for (int i = 0; i < hMap.width; i++)
        {
            for (int j = 0; j < hMap.height; j++)
            {
                holes[i, j] = true;
            }
        }

        tData.SetHoles(0, 0, holes);
    }

    void CorrigateWater()
    {
        var position = new Vector3(tData.size.x / 2, water.transform.position.y, tData.size.z / 2);
        var localScale = tData.size / 10;
        water.GetComponent<MapGeneratorWaterController>().SetTransform(position, localScale);
    }

    public void RenderNewHeight()
    {
        for (int i = 0; i < originalHeights.GetLength(0); i++)
        {
            for (int j = 0; j < originalHeights.GetLength(1); j++)
            {
                modifiedHeights[i, j] = originalHeights[i, j] * pointScalingModifier + Increase(originalHeights[i, j], pointDirectIncreaseModifier);
            }
        }
        tData.SetHeights(0, 0, modifiedHeights);
    }

    public void SetSize(float value)
    {
        sizeScaleModifier = value;
        tData.size = new Vector3(tDataSizeOriginal.x * sizeScaleModifier, tDataSizeOriginal.y * sizeScaleModifier * maximumHeightModifier, tDataSizeOriginal.z * sizeScaleModifier);
        CorrigateWater();
    }

    public void SetPointModifier(float value)
    {
        pointScalingModifier = value;
        RenderNewHeight();
    }

    public void SetPointIncrease(float value)
    {
        pointDirectIncreaseModifier = value;
        RenderNewHeight();
    }

    float Increase(float value, float increase)
    {
        if (value < 0.1) return 0;
        else return increase;
    }

    public void SetHeight(float value)
    {
        maximumHeightModifier = value;
        tData.size = new Vector3(tData.size.x, maximumHeightModifier, tData.size.z);
    }

    public void WireFrame()
    {
        if (wireframeBool) GetComponent<Terrain>().materialTemplate = wireframe;
        else GetComponent<Terrain>().materialTemplate = grey;
        wireframeBool = !wireframeBool;
    }

}
