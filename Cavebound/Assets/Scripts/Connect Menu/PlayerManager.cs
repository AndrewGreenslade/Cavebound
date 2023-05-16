using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using UnityEngine.UI;
using System.Linq;

public class PlayerManager : NetworkBehaviour
{
    public Button startGameButton;
    public List<PlayerMenuComponent> allPlayerButtons = new List<PlayerMenuComponent>();

    public int readyCount = 0;
    public int TotalPlayers = 0;

    public override void OnStartServer()
    {
        base.OnStartServer();
        startGameButton.onClick.AddListener(StartGame);
        startGameButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (IsServer)
        {
            allPlayerButtons = FindObjectsOfType<PlayerMenuComponent>().ToList();

            TotalPlayers = allPlayerButtons.Count;
            readyCount= 0;

            foreach (var item in allPlayerButtons)
            {
                if (item.isReady)
                {
                    readyCount++;
                }
            }

            if(readyCount == TotalPlayers)
            {
                startGameButton.gameObject.SetActive(true);
            }
            else
            {
                startGameButton.gameObject.SetActive(false);
            }
        }
        else
        {
            startGameButton.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        Debug.Log("Starting Game");
    }
}
