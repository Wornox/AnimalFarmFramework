using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomStartUp : MonoBehaviour
{
    public NavMeshSurface surface;

    public GameObject sceneTerrainSet;
    public GameObject sceneTerrain;
    public GameObject sceneWaterSet;
    public GameObject sceneWater;
    public InputController inputController;

    public bool drawGrass;


    void Awake()
    {
        TerrainController.currentTerrainSetInstance = sceneTerrainSet;
        WaterController.currentWaterSetInstance = sceneWaterSet;
    }
    void Start()
    {
        sceneTerrain.GetComponent<Terrain>().terrainData = TerrainController.generatedTerrainData;
        sceneTerrain.GetComponent<TerrainCollider>().terrainData = TerrainController.generatedTerrainData;

        (sceneWater.transform.position, sceneWater.transform.localScale) = WaterController.GetWater();

        surface.BuildNavMesh();

        TerrainController.DrawGrassOnAllTerrainOnSet(drawGrass);

        sceneWater.GetComponent<WaterLevel>().TerrainWaterCollosionPointsFindClosestLandPointToWater();

    }
}
