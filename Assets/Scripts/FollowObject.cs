using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FollowObject : MonoBehaviour
{
    Camera cam;

    private bool following = false;
    private LayerMask followable;
    public LayerMask unFollowable;
    public Transform followed;
    public float followspeed = 0.125f;
    public Vector3 followOffset;

    public CameraRotation camRot;

    // Start is called before the first frame update
    void Awake()
    {
        cam = GetComponent<Camera>();
        followable = ~unFollowable.value;
    }

    void FixedUpdate()
    {
        //click follow target
        if (!following && Input.GetMouseButtonDown(0))
        {
            //raycast
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 60, followable))
            {
                followed = hit.transform;
                following = true;
                Vector3 distanceVector = new Vector3(10f, 7, 10f);
                followOffset =  cam.transform.position - followed.position;
                Debug.Log("following :" + hit.transform.name);
            }
        }
    }

    void LateUpdate()
    {
        if (following && followed != null)
        {
            FollowAnimal(followed.position);
        }
    }

    void FollowAnimal(Vector3 followPos)
    {

        //Look around with Left Mouse
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; //Not allowing camrera rotaion if mouse draggin is on a UI element

            //camRot.yaw += camRot.lookSpeedH * Input.GetAxisRaw("Mouse X");
            //camRot.pitch -= camRot.lookSpeedV * Input.GetAxisRaw("Mouse Y");

            followOffset = Quaternion.AngleAxis(Input.GetAxisRaw("Mouse X") * 4f, Vector3.up) * followOffset;
            followOffset = Quaternion.AngleAxis(Input.GetAxisRaw("Mouse Y") * 4f, Vector3.right) * followOffset;

            //Debug.Log(Quaternion.AngleAxis(Input.GetAxisRaw("Mouse X") * 4f, Vector3.up).ToString());
            //Debug.Log(Quaternion.AngleAxis(Input.GetAxisRaw("Mouse Y") * 4f, Vector3.right).ToString());
            
        }

        //this.transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * 2f, Space.Self);
        //followOffset = Vector3.MoveTowards(followOffset, followed.position, Input.GetAxis("Mouse ScrollWheel") * 15f); //görgő zoom
        var mouseW = Input.GetAxis("Mouse ScrollWheel");
        if(mouseW != 0)
        {
            var ratio = followOffset.x / followOffset.z;
            followOffset.x += ratio * mouseW * 4.2111111f;
            followOffset.z += 1 * mouseW * 4.2111111f;

        }
        

        Vector3 velocity = Vector3.zero;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, followPos + followOffset, ref velocity, followspeed);
        transform.position = smoothedPosition;
        transform.LookAt(followPos);

    }

    public void StopFollowing()
    {
        following = false;
        followOffset = new Vector3(0, 0, 0);
    }

    public bool Following()
    {
        return following;
    }
}
