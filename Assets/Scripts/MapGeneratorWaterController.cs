using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGeneratorWaterController : MonoBehaviour
{
    public GameObject cutter;
    void Awake()
    {
        WaterController.SetWater(transform.position, transform.localScale);
    }

    public void SetHeigh(float value)
    {
        this.transform.position = new Vector3(transform.position.x, value, transform.position.z);
        WaterController.SetWater(transform.position, transform.localScale);
    }

    public void SetTransform(Vector3 pos, Vector3 scale)
    {
        transform.position = pos;
        transform.localScale = scale;
        WaterController.SetWater(transform.position, transform.localScale);
    }
}
