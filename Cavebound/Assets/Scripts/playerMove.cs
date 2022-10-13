using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMove : MonoBehaviour
{
    public float speed = 10.0f;

    private float movementX;
    private float movementY;

    private Camera cam;

    private float fireInput;
    private float camSizeDefault;
    public float camSizeUnzoomed;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
        camSizeDefault = cam.orthographicSize;
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(movementX, movementY, 0);
    }

    private void Update()
    {
        camZoom();
    }

    private void camZoom()
    {
        if (fireInput > 0)
        {
            cam.orthographicSize = camSizeUnzoomed;
        }
        else
        {
            cam.orthographicSize = camSizeDefault;
        }
    }

    void OnFire(InputValue t_fireVal)
    {
        fireInput = t_fireVal.Get<float>();
    }

    void OnMove(InputValue t_movementVal)
    {
        Vector2 moveVec = t_movementVal.Get<Vector2>();
        movementX = moveVec.x;
        movementY = moveVec.y;
    }
}
