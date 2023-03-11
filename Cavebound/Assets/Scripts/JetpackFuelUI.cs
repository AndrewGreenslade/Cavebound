using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JetpackFuelUI : MonoBehaviour
{
    private Image slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.fillAmount = playerMove.instance.jetPackFuel / 100.0f;
    }
}
