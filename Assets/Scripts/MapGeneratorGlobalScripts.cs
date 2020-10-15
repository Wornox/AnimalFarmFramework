using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGeneratorGlobalScripts : MonoBehaviour
{
    public GameObject water;
    public GameObject terrain;

    public Canvas canvas;

    private void Awake()
    {
        //terrain = GlobalReferences.GeneratedTerrain;
        //water = GlobalReferences.GeneratedWater;
    }
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //Enables and disables canvas
            canvas.enabled = !canvas.enabled;
        }
    }

    public void SetPointModifier(float value)
    {
        terrain.GetComponent<TerrainGenerator>().SetPointModifier(value);
    }

    public void SetPointIncrease(float value)
    {
        terrain.GetComponent<TerrainGenerator>().SetPointIncrease(value);
    }

    public void SetSize(float value)
    {
        terrain.GetComponent<TerrainGenerator>().SetSize(value);
    }

    public void SetHeight(float value)
    {
        terrain.GetComponent<TerrainGenerator>().SetHeight(value);
    }

    public void GenerateHeightMap(Texture2D hMapIn)
    {
        terrain.GetComponent<TerrainGenerator>().GenerateHeightMap(hMapIn);
    }

    public void SetWaterLevel(float value)
    {
        water.GetComponent<MapGeneratorWaterController>().SetHeigh(value);
    }
}
