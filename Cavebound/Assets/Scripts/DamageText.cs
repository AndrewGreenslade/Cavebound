using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : NetworkBehaviour
{
    private Vector3 GoToPos;
    public float moveSpeed = 0.15f;
    public float colorChangeSpeed = 0.1f;
    public TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        float randomPos = Random.Range(-1.0f, 1.0f);
        GoToPos = transform.position + new Vector3(randomPos, 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, GoToPos, moveSpeed * Time.fixedDeltaTime);
        text.color = Color.Lerp(text.color, new Color(1, 0, 0, 0), colorChangeSpeed);
    }
}
