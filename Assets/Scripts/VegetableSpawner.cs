using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VegetableType { grain, corn }

public class VegetableSpawner : MonoBehaviour
{
    public float grainSpawnTimer;
    public float maxGrainNumber;
    public float currentGrainNumber;
    public float regenateRate;

    public VegetableType vegetableType;
    public GameObject grainPrefab;

    private CSVExport csv;

    public bool drawHTSlider = true;

    void Start()
    {
        csv = new CSVExport("Grain", AnimalType.None);
        csv.AddContent("System Time:");
        csv.AddContent("Simulation Time:");
        csv.AddContent("VegetableType:");
        csv.AddContent("CurrentNumber:");

        csv.file.WriteLine(csv.content);
        csv.file.Flush();
        csv.content = "";

        StartCoroutine(WaiterSpawn());
    }

    IEnumerator WaiterSpawn()
    {
        yield return new WaitForSeconds(grainSpawnTimer); //wait grainSpawn to spawn
        switch (vegetableType)
        {
            case VegetableType.grain:
                SpawnGrain();
                string systemTime = System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second;
                //string simTime = Time.frameCount.ToString();
                string simTime = Time.timeSinceLevelLoad.ToString();
                csv.AddContent(systemTime);
                csv.AddContent(simTime);
                csv.AddContent("Grain");
                csv.AddContent(currentGrainNumber);

                csv.file.WriteLine(csv.content);
                csv.file.Flush();
                csv.content = "";

                break;
            case VegetableType.corn:
                SpawnCorn();
                break;
        }
        //SpawnGrain();
        StartCoroutine(WaiterSpawn()); //recall WaiterSpawn() to spawn again
    }

    public void SpawnGrain()
    {
        currentGrainNumber = transform.childCount;
        if (currentGrainNumber > maxGrainNumber) return;
        var randompos = TerrainController.GetRandomPosOnTerrainSet();
        if (Vector3.Equals(randompos, Vector3.negativeInfinity)) return;
        var randomNavPos = NavMeshController.GetRandomPointOnNavmesh(1, randompos);

        GameObject grain = Instantiate(grainPrefab, randomNavPos, Quaternion.identity);

        var ACR = grain.GetComponent<AnimalComponentReferences>();
        var ASC = ACR.animalStatusController;

        ASC.HungerBackground.SetActive(drawHTSlider);
        ASC.HungerText.gameObject.SetActive(!drawHTSlider);

        ACR.livingCreature.vegetableRegenRate = regenateRate;

        grain.transform.SetParent(this.gameObject.transform);
    }

    void SpawnCorn()
    {
        //spawn corn
    }

    public string GetStats()
    {
        string s = "Vegetation :" + System.Environment.NewLine + System.Environment.NewLine;
        s += "Grain" + " -> ";
        s += "Current: " + currentGrainNumber + " | Limit: " + maxGrainNumber + " | SpawnTime(sec): " + grainSpawnTimer + " | RegenateRate(%/sec): " + regenateRate;
        s += System.Environment.NewLine;
        return s;
    }
}
