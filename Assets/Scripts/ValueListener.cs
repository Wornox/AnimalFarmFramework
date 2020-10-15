using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueListener : MonoBehaviour
{
    private void Start()
    {
        SetValueToText(GetComponentInParent<Slider>().value);
    }
    public void SetValueToText(float value)
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = Math.Round(value, 2).ToString();
    }
}
