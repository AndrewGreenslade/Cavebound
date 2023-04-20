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

    
    public void AddOreToAmount()
    {
        Debug.Log("Pressed " + oreName + " ore button");

        Inventory playerInv = emptyPlayer.instance.SpawnedPlayer.GetComponent<Inventory>();
        OreRecord record = playerInv.oresRetrieved.Find(x => x.prefab.OreName == oreName);
        int amount = record.amount;

        if (amount >= 100)
        {
            Debug.Log("Depositing " + oreName + " x" + 100.ToString());
            emptyPlayer.instance.GetComponent<StoredInventory>().StoreOreinInventory(oreName, 100);
            record.amount -= 100;
        }
        else
        {
            Debug.Log("Depositing " + oreName + " x" + amount.ToString());
            emptyPlayer.instance.GetComponent<StoredInventory>().StoreOreinInventory(oreName, amount);
            record.amount = 0;
        }

        List<oreHud> hudList = FindObjectsOfType<oreHud>().ToList();
        oreHud hudObj = hudList.Find(x => x.oreName == oreName);
        hudObj.RemoveOreFromAmount(amount);

        oreAmount += amount;
        oreAmountText.text = oreAmount.ToString();
    }
}
