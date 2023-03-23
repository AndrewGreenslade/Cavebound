using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUII : MonoBehaviour
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
        slider.fillAmount = PlayerScript.instance.health / 100.0f;
    }
}
