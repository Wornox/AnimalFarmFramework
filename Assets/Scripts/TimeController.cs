using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public float maxTimeSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetScaleTime(float f)
    {
        if (CheckMaxSpeed(f)) return;
        Time.timeScale = f;
    }

    public void Increase()
    {
        if (CheckMaxSpeed(Time.timeScale)) return;
        if (Time.timeScale > 1) Time.timeScale += 1;
        else Time.timeScale = Time.timeScale * 2;
    }

    public void Decrease()
    {
        if (Time.timeScale > 1) Time.timeScale -= 1;
        else Time.timeScale = Time.timeScale / 2;
    }

    private bool CheckMaxSpeed(float speed)
    {
        bool overMax;
        if (speed >= maxTimeSpeed) overMax = true;
        else overMax = false;
        return overMax;
    }
}
