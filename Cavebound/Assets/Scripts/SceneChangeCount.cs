using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Managing.Scened;
using FishNet.Object.Synchronizing;

public class SceneChangeCount : NetworkBehaviour
{
    [SyncVar]
    public int totalPlayersLoadedScene = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        SceneManager.OnLoadEnd += SceneManager_OnClientLoadedStartScenes;
    }

    private void SceneManager_OnClientLoadedStartScenes(SceneLoadEndEventArgs args)
    {
        if (args.LoadedScenes.Length > 0)
        {
            if (args.LoadedScenes[0].name == "Game")
            {
                increaseLoadedCount();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void increaseLoadedCount()
    {
        totalPlayersLoadedScene++;
    }
}
