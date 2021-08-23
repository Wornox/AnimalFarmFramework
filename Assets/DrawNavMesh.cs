using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DrawNavMesh : MonoBehaviour
{
    Color walkableColor = Color.blue;
    Color nonWalkableColor = Color.red;
    Color unknownColor = Color.gray;

    NavMeshTriangulation triangulation;

    Material material;

    bool isCalcualted = false; 
    bool isEnabled = false; 


    public void ShowNavmesh()
    {
        isEnabled = !isEnabled;
        if (!isCalcualted)
        {
            triangulation = NavMesh.CalculateTriangulation();
            isCalcualted = true;
        }
    }

    void OnPostRender()
    {
        if (material == null || !isEnabled)
        {
            return;
        }

        GL.PushMatrix();

        material.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        for (int i = 0; i < triangulation.indices.Length; i += 3)
        {
            var triangleIndex = i / 3;
            var i1 = triangulation.indices[i];
            var i2 = triangulation.indices[i + 1];
            var i3 = triangulation.indices[i + 2];
            var p1 = triangulation.vertices[i1];
            var p2 = triangulation.vertices[i2];
            var p3 = triangulation.vertices[i3];
            var areaIndex = triangulation.areas[triangleIndex];
            Color color;
            switch (areaIndex)
            {
                case 0:
                    color = walkableColor; break;
                case 1:
                    color = nonWalkableColor; break;
                default:
                    color = unknownColor; break;
            }
            GL.Color(color);
            GL.Vertex(p1);
            GL.Vertex(p2);
            GL.Vertex(p3);
        }
        GL.End();

        GL.PopMatrix();
    }
}
