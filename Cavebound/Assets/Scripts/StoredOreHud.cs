using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using FishNet.Object;
using FishNet.Connection;

public class StoredOreHud : MonoBehaviour
{
    public string oreName;
    private int oreAmount;
    private Color oreColor;
    public GameObject OreDepositor;

    public TextMeshProUGUI oreNameText;
    public TextMeshProUGUI oreAmountText;
    public Image oreImage;

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
        oreImage.color = oreColor;
    }

    
    public void StoreOre()
    {
        Debug.Log("Pressed " + oreName + " ore button");
        
        Inventory worldPlayerInv = emptyPlayer.instance.SpawnedPlayer.GetComponent<Inventory>();
        
        int amountOfOres = worldPlayerInv.GetAndResetOreAmount(oreName);

        StoredInventory StoredPlayerInv = emptyPlayer.instance.GetComponent<StoredInventory>();
        StoredPlayerInv.AddOreToAmount(oreName, amountOfOres);

        List<oreHud> hudList = FindObjectsOfType<oreHud>().ToList();
        oreHud hudObj = hudList.Find(x => x.oreName == oreName);
        hudObj.RemoveOreFromAmount(amountOfOres);

        oreAmount += amountOfOres;
        oreAmountText.text = oreAmount.ToString();
    }
}
