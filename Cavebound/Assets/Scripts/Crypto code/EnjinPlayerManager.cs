using Enjin.SDK.Events;
using Enjin.SDK;
using Enjin.SDK.Graphql;
using Enjin.SDK.Models;
using Enjin.SDK.PlayerSchema;
using Enjin.SDK.Shared;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnjinPlayerManager : MonoBehaviour
{
    [Header("Player Vars")]
    public string ethAddress;
    public TextMeshProUGUI nameDisplay;

    [Header("Balances!")]
    Dictionary<string, int> fungibleBalances;
    Dictionary<string, ISet<string>> nonFungibleBalances;

    // Client and event service
    PlayerClient client;
    PusherEventService eventService;

    // Mutexes
    object mutex = new object();

    // ---------------------------------------------------
    // For retreiving player data from enjin services
    // ---------------------------------------------------

    void RetrievePlayerData()
    {
        GetPlayer req = new GetPlayer().WithWallet().WithWalletBalance();

        client.GetPlayer(req).ContinueWith(task =>
        {
            if (!task.IsCompletedSuccessfully)
            {
                // Handle unsuccessful task
                return;
            }

            GraphqlResponse<Player> res = task.Result;
            if (!res.IsSuccess)
            {
                // Handle unsuccessful response
                return;
            }

            Player player = res.Result;
            Wallet wallet = player.Wallet;

            if (wallet == null)
            {
                // Handle unlinked player
                return;
            }

            lock (mutex)
            {
                ethAddress = wallet.EthAddress;
                eventService.SubscribeToWallet(ethAddress);

                foreach (Balance balance in wallet.Balances)
                {
                    string id = balance.Id;
                    string index = balance.Index;
                    int? value = balance.Value;

                    if (index.Equals("0000000000000000"))
                    {
                        AddBalanceImpl(id, value.Value);
                    }
                    else
                    {
                        AddBalanceImpl(id, index);
                    }
                }
            }
        });
    }

    // ---------------------------------------------------
    // adding ballances for Fungible assets
    // ---------------------------------------------------

    public void AddBalance(string id, int value)
    {
        lock (mutex)
        {
            AddBalanceImpl(id, value);
        }
    }

    void AddBalanceImpl(string id, int value)
    {
        if (fungibleBalances.ContainsKey(id))
        {
            fungibleBalances[id] += value;
        }
        else
        {
            fungibleBalances.Add(id, value);
        }
    }

    // ---------------------------------------------------
    // adding ballances for non-Fungible assets
    // ---------------------------------------------------

    public void AddBalance(string id, string index)
    {
        lock (mutex)
        {
            AddBalanceImpl(id, index);
        }
    }

    void AddBalanceImpl(string id, string index)
    {
        ISet<string> indices;

        if (nonFungibleBalances.ContainsKey(id))
        {
            indices = nonFungibleBalances[id];
        }
        else
        {
            indices = new HashSet<string>();
            nonFungibleBalances.Add(id, indices);
        }

        indices.Add(index);
    }

    // ---------------------------------------------------
    // Subtracting ballances for Fungible assets
    // ---------------------------------------------------

    public void SubtractBalance(string id, int value)
    {
        lock (mutex)
        {
            if (!fungibleBalances.ContainsKey(id))
            {
                return;
            }

            int newValue = fungibleBalances[id] - value;

            if (newValue > 0)
            {
                fungibleBalances[id] = newValue;
            }
            else
            {
                fungibleBalances.Remove(id);
            }
        }
    }

    // ---------------------------------------------------
    // Subtracting ballances for non-Fungible assets
    // ---------------------------------------------------

    public void SubtractBalance(string id, string index)
    {
        lock (mutex)
        {
            if (nonFungibleBalances.ContainsKey(id))
            {
                nonFungibleBalances[id].Remove(index);
            }
        }
    }
}
