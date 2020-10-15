using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float speed = 0.5f;
    [SerializeField] float sensitivity = 0.5f;

    Camera cam;

    public FollowObject followObject;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        if (Time.timeScale >= 1) Move(Time.timeScale, 1);
    }

    void Update()
    {
        if (Time.timeScale < 1) Move(1, 1);
    }

    void Move(float timeScale, float deltaTime)
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            move += Vector3.forward * speed / timeScale / deltaTime;
        if (Input.GetKey(KeyCode.S))
            move -= Vector3.forward * speed / timeScale / deltaTime;
        if (Input.GetKey(KeyCode.D))
            move += Vector3.right * speed / timeScale / deltaTime;
        if (Input.GetKey(KeyCode.A))
            move -= Vector3.right * speed / timeScale / deltaTime;
        if (Input.GetKey(KeyCode.E))
            move += Vector3.up * speed / timeScale / deltaTime;
        if (Input.GetKey(KeyCode.Q))
            move -= Vector3.up * speed / timeScale / deltaTime;
        if (Input.GetKey(KeyCode.LeftShift))
            move *= 3;
        if (followObject != null && followObject.Following() && move != Vector3.zero) followObject.StopFollowing();
        transform.Translate(move);
    }
}
