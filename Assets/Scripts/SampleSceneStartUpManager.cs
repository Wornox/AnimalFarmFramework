using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SampleSceneStartUpManager : MonoBehaviour
{
    public NavMeshSurface surface;

    public GameObject sceneTerrainSet;
    public GameObject sceneWaterSet;
    public InputController inputController;

    public bool drawGrass;

    void Awake()
    {
        TerrainController.currentTerrainSetInstance = sceneTerrainSet;
        WaterController.currentWaterSetInstance = sceneWaterSet;
    }

    void Start()
    {
        //surface.BuildNavMesh();

        TerrainController.DrawGrassOnAllTerrainOnSet(drawGrass);
        //waterSet.GetComponent<Water>();

        //foreach (var t in sceneWaterSet.transform)
        //{
        //}

        foreach (Transform w in sceneWaterSet.transform)
        {
            var WL = w.GetComponent<WaterLevel>();
            if(WL != null)WL.TerrainWaterCollosionPointsFindClosestWaterPointToLand();
        }

        StartCoroutine(SpawnSampleAnimals());
    }

    IEnumerator SpawnSampleAnimals()
    {
        //yield return new WaitForEndOfFrame(); // az első update frameben fogja majd léterhozni őket, hogy mevárja a többi starot mindenképpen h lefussanak
        yield return new WaitForSeconds(1);
        inputController.InterpretCommand("speed 2");
        inputController.InterpretCommand("add chicken 8 random");
        inputController.InterpretCommand("add pig 8 random");
        inputController.InterpretCommand("add cow 8 random");
        inputController.InterpretCommand("add dog 6 random");
        inputController.InterpretCommand("add fox 6 random");
        inputController.InterpretCommand("add lion 4 random");

        yield return null;
    }

}
