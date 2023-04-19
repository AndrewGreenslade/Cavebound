using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSlider : MonoBehaviour
{
    public Slider customizationSlider;
    public TextMeshProUGUI sliderValueText;

    // Update is called once per frame
    void Update()
    {
        sliderValueText.text = customizationSlider.value + " / 100";        
    }
}
