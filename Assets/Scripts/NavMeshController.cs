using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshController
{
    public static Vector3 GetRandomPointOnNavmesh(string areaName, Vector3 relativePos)
    {
        return GetRandomPointOnNavmesh(1 << NavMesh.GetAreaFromName(areaName), relativePos);
    }

    public static Vector3 GetRandomPointOnNavmesh(int areaBit, Vector3 relativePos)
    {
        NavMeshHit hit;
        bool validPoint = false;
        int breakTimer = 1000;
        var navMeshPos = Vector3.zero;

        while (!validPoint && breakTimer > 0)
        {
            validPoint = NavMesh.SamplePosition(relativePos, out hit, 10000, areaBit);
            if (validPoint) navMeshPos = hit.position;
            breakTimer--;
        }
        return navMeshPos;
    }
}
