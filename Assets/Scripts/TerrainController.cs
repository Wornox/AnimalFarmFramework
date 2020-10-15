using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TerrainController
{

    public static GameObject currentTerrainSetInstance;

    public static TerrainData generatedTerrainData;

    /// <summary>
    /// A random pos, or negativeInfinity if no terrain
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetRandomPosOnTerrainSet()
    {
        return GetRandomPosOnTerrain(GetRandomTerrainFromTerrainSet());
    }

    public static void AddTerrain(GameObject terrain)
    {
        if (currentTerrainSetInstance != null) terrain.transform.parent = currentTerrainSetInstance.transform;
    }

    /// <summary>
    /// Get a random valid postion on the map
    /// </summary>
    /// <param name="terrain"></param>
    /// <returns>A random pos, or negativeInfinity if no terrain</returns>
    public static Vector3 GetRandomPosOnTerrain(GameObject terrain)
    {
        Vector3 randomPosition = Vector3.negativeInfinity;
        if (terrain != null)
        {
            var asd = terrain.GetComponent<Terrain>();
            var tData = terrain.GetComponent<Terrain>().terrainData;
            //Get the corner pos of the terrain
            //var randomPosition = terrainGO.GetComponent<Terrain>().terrainData.bounds.center;

            //only get Above water position
            int breakTimer = 1000;
            while(breakTimer > 0)
            {
                randomPosition = terrain.GetComponent<Terrain>().GetPosition();

                Vector3 waterPos;
                Vector3 waterScale;
                (waterPos, waterScale) = WaterController.GetWater();

                //Creating difference from the train middle point -- default terrain size is 100 with 1 scale -- only half is needed in one direction -- allowing only 90% to avid outer areas
                randomPosition.x += Random.Range(0, terrain.GetComponent<Terrain>().terrainData.bounds.max.x);
                randomPosition.z += Random.Range(0, terrain.GetComponent<Terrain>().terrainData.bounds.max.z);

                //Getting the current height from Terrain depading on x,z coords
                float y = terrain.GetComponent<Terrain>().SampleHeight(randomPosition);
                randomPosition.y = y;

                if (y > waterPos.y) // check if above water
                {
                    break;  //if above, break -> valid point
                }
                breakTimer--;
            }


            
        }
        return randomPosition;
    }

    /// <summary>
    /// Get a random terrain child from terrainset
    /// </summary>
    /// <returns>A gameobject child of the set, null if no child</returns>
    public static GameObject GetRandomTerrainFromTerrainSet()
    {
        GameObject terrain = null;
        if(currentTerrainSetInstance.transform.childCount > 0)
        {
            int randomChildIndex = Random.Range(0, currentTerrainSetInstance.transform.childCount);
            terrain = currentTerrainSetInstance.transform.GetChild(randomChildIndex).gameObject;
        }
        return terrain;
    }

    public static void DrawGrassOnAllTerrainOnSet(bool draw)
    {
        int children = currentTerrainSetInstance.transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            currentTerrainSetInstance.transform.GetChild(i).GetComponent<Terrain>().drawTreesAndFoliage = draw;
        }
    }

}
