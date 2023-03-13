using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class playerMove : NetworkBehaviour
{
    public float speed = 10.0f;
    public Transform raycastPosition;
    public float jumpRayCastDistance;
    public LayerMask groundLayer;

    [SyncVar]
    private float movementX;
    public float JumpForce;
    public float jetPackFuel;
    public float FuelSpeed;
    private CinemachineVirtualCamera myCam;

    private float jumpInput;
    public ParticleSystem JumpParticleSys;
    public SpriteRenderer playerSprite;
    public GameObject hud;

    public static playerMove instance;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner)
        {
            Destroy(GetComponent<PlayerInput>());
            return;
        }

        if (instance == null)
        {
            instance = this;
        }

        Instantiate(hud);

        myCam = FindObjectOfType<CinemachineVirtualCamera>();
        myCam.Follow = transform;
    }

    private void Update()
    { 
        Debug.DrawRay(raycastPosition.position, Vector2.down * jumpRayCastDistance, Color.red);

        if(movementX > 0)
        {
            playerSprite.flipX = false;
        }
        else if (movementX < 0)
        {
            playerSprite.flipX = true;
        }

        if (!base.IsOwner)
        {
            return;
        }
        
        transform.Translate(new Vector3(movementX, 0, 0) * Time.deltaTime * speed);
        
        if(jumpInput > 0)
        {
            if (isGrounded())
            {
                JumpParticleSys.enableEmission = true;
            }

            if (jetPackFuel > 0)
            {
                transform.position += new Vector3(0, jumpInput, 0) * Time.deltaTime * JumpForce;
                jetPackFuel -= Time.deltaTime * FuelSpeed;
            }
        }

        if (isGrounded() && jumpInput == 0)
        {
            JumpParticleSys.enableEmission = false;

            jetPackFuel += Time.deltaTime * FuelSpeed;
        }
        else if (jetPackFuel > 100)
        {
            jetPackFuel = 100;
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

    [ServerRpc]
    public void updateMovmentOnServerRPC(Vector2 input)
    {
        movementX = input.x;
    }

    void OnMove(InputValue t_movementVal)
    {
        Vector2 moveVec = t_movementVal.Get<Vector2>();
        updateMovmentOnServerRPC(moveVec);
    }

    void OnJump(InputValue t_jumpVal)
    {
        jumpInput = t_jumpVal.Get<float>();
    }
}
