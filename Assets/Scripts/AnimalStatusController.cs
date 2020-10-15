using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalStatusController : MonoBehaviour
{
    public TextMesh PackText;
    public TextMesh AgeText;
    public TextMesh MatingText;
    public TextMesh StatusText;

    public Slider Hunger;
    public GameObject HungerBackground;
    public TextMesh HungerText;

    public Slider Thirst;
    public GameObject ThirstBackground;
    public TextMesh ThirstText;

    private Camera camera;
    private Transform cameraTransform;

    public Canvas canvas;
    public CanvasScaler canvasScaler;
    void Awake()
    {
        camera = Camera.main;
        cameraTransform = camera.transform;
    }

    void Update()
    {
        transform.LookAt(cameraTransform);
    }

    public void SetPack(string text)
    {
        if (PackText != null) PackText.text = text;
    }

    public void SetAge(string text)
    {
        if (AgeText != null) PackText.text = text;
    }

    public void SetMating(string text)
    {
        if (MatingText != null) PackText.text = text;
    }

    public void SetStatus(string text)
    {
        if (StatusText != null) PackText.text = text;
    }

    public void SetHunger(float value)
    {
        if (Hunger != null)
        {
            Hunger.value = value / 100f;
            HungerText.text = System.Math.Round(value, 2).ToString();
        }
    }

    public void SetThirst(float value)
    {
        if (Thirst != null)
        {
            Thirst.value = value / 100f;
            ThirstText.text = System.Math.Round(value, 2).ToString();
        }
    }

    /// <summary>
    /// Turning it to FALSE will result more fps while running simualtions with a lot(e.g. 1000) animals.
    /// </summary>
    public void EnableHungerThirstSlider(bool enable)
    {
        if (HungerBackground != null)
        {
            HungerBackground.SetActive(enable);
            HungerText.gameObject.SetActive(!enable);
        }
        if (ThirstBackground != null)
        {
            ThirstBackground.SetActive(enable);
            ThirstText.gameObject.SetActive(!enable);
        }
        canvas.enabled = enable;
        canvasScaler.enabled = enable;
    }
}
