using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class playerMove : NetworkBehaviour
{
    public float speed = 10.0f;
    public float jumpForce = 100.0f;
    public Transform raycastPosition;
    public float jumpRayCastDistance;
    public LayerMask groundLayer;

    private float movementX;
    private Vector2 origionalScale;
    private CinemachineVirtualCamera myCam;
    private Rigidbody2D rb;

    private float fireInput;
    private float jumpInput;
    public ParticleSystem JumpParticleSys;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        origionalScale = transform.localScale;

        if (!isLocalPlayer)
        {
            Destroy(GetComponent<PlayerInput>());
            this.enabled = false;
            return;
        }

        myCam = FindObjectOfType<CinemachineVirtualCamera>();
        myCam.Follow = transform;
    }

    private void Update()
    {
        Debug.DrawRay(raycastPosition.position, Vector2.down * jumpRayCastDistance, Color.red);

        if(movementX > 0)
        {
            transform.localScale = new Vector2(origionalScale.x,origionalScale.y);
        }
        else if (movementX < 0)
        {
            transform.localScale = new Vector2(-origionalScale.x, origionalScale.y);
        }


        if (!isLocalPlayer)
        {

            return;
        }
        
        transform.position += new Vector3(movementX, 0, 0) * Time.deltaTime * speed;
        
        if(jumpInput > 0)
        {
            if (isGrounded())
            {
                JumpParticleSys.enableEmission = true;
                rb.AddForce(Vector2.up * jumpForce,ForceMode2D.Impulse);
            }
        }

        if (isGrounded() && jumpInput == 0)
        {
            JumpParticleSys.enableEmission = false;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D ray = Physics2D.Raycast(raycastPosition.position, Vector2.down, jumpRayCastDistance, groundLayer);
        if(ray.collider != null)
        {
            return true;
        }

        return false;
    }

    void OnFire(InputValue t_fireVal)
    {
        fireInput = t_fireVal.Get<float>();
    }

    void OnMove(InputValue t_movementVal)
    {
        Vector2 moveVec = t_movementVal.Get<Vector2>();
        movementX = moveVec.x;
    }

    void OnJump(InputValue t_jumpVal)
    {
        jumpInput= t_jumpVal.Get<float>();
    }
}
