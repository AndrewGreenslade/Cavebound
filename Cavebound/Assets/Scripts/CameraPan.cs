using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    Vector3 startPos;
    public int length;

    private void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(startPos, Camera.main.ScreenToWorldPoint(Input.mousePosition)) >= length)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position = new Vector2(Mathf.Clamp(transform.position.x, -(350 / 2), 350 / 2),Mathf.Clamp(transform.position.y, -(200 / 2), 200 / 2));
        }        
    }
}
