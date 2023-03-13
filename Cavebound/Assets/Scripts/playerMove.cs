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
    public float movementX;
    public float JumpForce;
    public float jetPackFuel;
    public float FuelSpeed;
    private CinemachineVirtualCamera myCam;

    private float jumpInput;
    public ParticleSystem JumpParticleSys;
    public SpriteRenderer playerSprite;
    public GameObject hud;
    private Rigidbody2D rb;

    public static playerMove instance;
    public Animator animController;

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
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<Animator>();
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

        // transform.Translate(new Vector3(movementX, 0, 0) * Time.deltaTime * speed);
        Vector3 direction = transform.right * movementX;
        direction.x *= speed;
        direction.y = rb.velocity.y;
        rb.velocity = direction;

        animController.SetBool("IsGrounded", isGrounded());
        
        if (isGrounded())
        {
            JumpParticleSys.enableEmission = true;
        }

        if (jumpInput > 0)
        {
            if (jetPackFuel > 0)
            {
                rb.AddForce(Vector2.up * jumpInput * Time.deltaTime * JumpForce, ForceMode2D.Impulse);

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
    public void updateMovmentOnServerRPC(float inputX)
    {
        movementX = inputX;
    }

    void OnMove(InputValue t_movementVal)
    {
        Vector2 moveVec = t_movementVal.Get<Vector2>();
        updateMovmentOnServerRPC(moveVec.x);
    }

    void OnJump(InputValue t_jumpVal)
    {
        jumpInput = t_jumpVal.Get<float>();
    }
}
