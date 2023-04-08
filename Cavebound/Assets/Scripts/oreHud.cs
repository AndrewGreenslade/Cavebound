using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class oreHud : MonoBehaviour
{
    public string oreName;
    private int oreAmount;
    private Color oreColor;

    public TextMeshProUGUI oreNameText;
    public TextMeshProUGUI oreAmountText;
    public Image oreImage;
    public Image oreFillMGImage;

    public void setVars(string t_name, Color t_oreColor)
    {
        oreName = t_name;
        oreAmount = 0;
        oreColor = t_oreColor;

        setText();
    }

    public void setText()
    {
        oreAmountText.text = oreAmount.ToString();
        oreNameText.text = oreName;
        oreImage.color= oreColor;
    }
    
    public void AddOreToAmount()
    {
        oreAmount++;
        oreAmountText.text = oreAmount.ToString();
    }

    public void RemoveOreFromAmount(int t_amount)
    {
        oreAmount -= t_amount;
        oreAmountText.text = oreAmount.ToString();
    }

    public void Update()
    {
        oreFillMGImage.fillAmount = oreAmount / 1000.0f; 
    }
}
