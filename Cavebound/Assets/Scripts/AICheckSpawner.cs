using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICheckSpawner : MonoBehaviour
{
    public LayerMask groundLayer;
    public bool shouldSpawn = false;
    public float timeToSpawn = 5;

    float timeElapsed = 0;
    bool isTouchingGround = false;
    public static AICheckSpawner instance;

    private void Start()
    {
        instance = this;
        ChangePos();
    }

    private void Update()
    {
        if (!isTouchingGround)
        {
            timeElapsed += Time.deltaTime;
        } 

        if(timeElapsed > timeToSpawn)
        {
            timeElapsed = timeToSpawn;
            shouldSpawn = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            isTouchingGround = true;
            ChangePos();
        }
    }

    private void ChangePos()
    {
        int offset = 10;

        int randX = Random.Range(offset, MapGenerator.instance.MapWidth - offset);
        int randY = Random.Range(offset, MapGenerator.instance.MapHieght - offset);

        transform.position = new Vector3(randX, randY, 0);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isTouchingGround = false;
    }
}
