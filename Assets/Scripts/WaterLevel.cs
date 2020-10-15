using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaterLevel : MonoBehaviour
{
    private Terrain terrain;

    private List<Vector3> aboveWaterVerS;

    public List<Vector3> waterPoints;

    private GameObject TerrainSet;

    public ContactPoint[] cp = new ContactPoint[10000];

    void Awake()
    {
        waterPoints = new List<Vector3>();

    }

    public void TerrainWaterCollosionPointsFindClosestWaterPointToLand()
    {
        foreach (Transform terrain in TerrainController.currentTerrainSetInstance.transform)
        {
            var _terrain = terrain.GetComponent<Terrain>();
            int width = (int)_terrain.terrainData.size.x;
            int height = (int)_terrain.terrainData.size.z;

            var b1 = transform.GetComponent<MeshCollider>().bounds;

            float scanningIteration = 5;

            for (float i = 0; i < width; i += scanningIteration)
            {
                for (float j = 0; j < height; j += scanningIteration)
                {
                    var h = _terrain.SampleHeight(new Vector3(i + _terrain.transform.position.x, 0, j + _terrain.transform.position.z)); //relative height from terrain
                    h += _terrain.transform.position.y; //making it global

                    if (h < transform.position.y) continue; // if poin is underwater skip

                    var pos = new Vector3(i + _terrain.transform.position.x, h, j + _terrain.transform.position.z);

                    Vector3 closestWaterPoint;
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(pos, out hit, scanningIteration, 8))
                    {
                        closestWaterPoint = hit.position;
                        //if (b1.SqrDistance(closestWaterPoint) < 0.2f)
                        var h2 = _terrain.SampleHeight(closestWaterPoint);
                        h2 += _terrain.transform.position.y;
                        if (closestWaterPoint.y > h2) // only adding if water is top
                            waterPoints.Add(closestWaterPoint);
                    }
                    
                }
            }
        }
        Debug.Log("WPC: " + waterPoints.Count);
    }

    public void TerrainWaterCollosionPointsFindClosestLandPointToWater()
    {
        foreach (Transform terrain in TerrainController.currentTerrainSetInstance.transform)
        {
            var _terrain = terrain.GetComponent<Terrain>();
            int width = (int)_terrain.terrainData.size.x;
            int height = (int)_terrain.terrainData.size.z;

            float scanningIteration = 10;
            var b1 = transform.GetComponent<MeshCollider>().bounds;

            if (b1.SqrDistance(_terrain.transform.position) > (_terrain.terrainData.size.x / 2) * (_terrain.terrainData.size.y / 2)) continue;


            for (float i = 0; i < width; i += scanningIteration)
            {
                for (float j = 0; j < height; j += scanningIteration)
                {
                    var h = _terrain.SampleHeight(new Vector3(i + _terrain.transform.position.x, 0, j + _terrain.transform.position.z)); //relative height from terrain
                    h += _terrain.transform.position.y; //making it global

                    if (h < this.transform.position.y) // only if water is top, find the nearest land point
                    {
                        var pos = new Vector3(i + _terrain.transform.position.x, h, j + _terrain.transform.position.z);

                        Vector3 closestValidPoint;
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(pos, out hit, scanningIteration, 1))
                        {
                            closestValidPoint = hit.position;
                            //if (b.SqrDistance(closestValidPoint) < 0.2f)
                                waterPoints.Add(closestValidPoint);
                        }
                    }
                }
            }
        }
        Debug.Log("WPC: " + waterPoints.Count);
    }


    public void TerrainWaterCollosionPointsOLD()
    {
        foreach (Transform terrain in TerrainController.currentTerrainSetInstance.transform)
        {
            var _terrain = terrain.GetComponent<Terrain>();
            int width = (int)_terrain.terrainData.size.x;
            int height = (int)_terrain.terrainData.size.z;

            float scanningIteration = 10;

            for (float i = 0; i < width; i += scanningIteration)
            {
                for (float j = 0; j < height; j += scanningIteration)
                {
                    var h = _terrain.SampleHeight(new Vector3(i + _terrain.transform.position.x, 0, j + _terrain.transform.position.z));
                    h += _terrain.transform.position.y;

                    if ((h < this.transform.position.y + 0.1f) && (h > this.transform.position.y)) //only above + a little pos is good
                    {
                        var pos = new Vector3(i + _terrain.transform.position.x, h, j + _terrain.transform.position.z);

                        waterPoints.Add(pos);
                    }
                }
            }
        }
        Debug.Log("WPC: " + waterPoints.Count);
    }

    public (Vector3, bool) GetClosestBorderPoint(Vector3 origin, float range)
    {
        if (waterPoints.Count < 1) return (Vector3.zero, false);
        Vector3 closestAbovePoint = waterPoints[0];
        bool found = false;

        foreach (var borderpoint in waterPoints)
        {
            var distanceVector = (borderpoint - origin);
            var closestAbovePointVector = (closestAbovePoint - origin);
            if ((distanceVector.sqrMagnitude < closestAbovePointVector.sqrMagnitude) && distanceVector.sqrMagnitude < range * range) //efficient distanceCheck
            {
                found = true;
                closestAbovePoint = borderpoint;
            }
        }

        return (closestAbovePoint, found);
    }

    private void OnDrawGizmos()
    {
        if (waterPoints.Count < 80000)
        {
            foreach (var v in waterPoints)
            {
                Gizmos.DrawSphere(v, 1f);
            }
        }
        else Debug.Log("Too much spheres in WaterLevel.cs, (above 80000) canceling drawing");
    }

}
