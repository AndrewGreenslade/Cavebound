using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public struct CreateCharacterMessage : NetworkMessage
{

}

public class MyNetworkManager : NetworkRoomManager
{
    public List<GameObject> RoomPlayerObjects= new List<GameObject>();
    public List<GameObject> ActivePlayerObjects = new List<GameObject>();

    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        RoomPlayerObjects.Add(roomPlayer);
        ActivePlayerObjects.Add(gamePlayer);

        roomPlayer.name = "Room player: " + conn.connectionId;
        gamePlayer.name = "Game player: " + conn.connectionId;
        
        return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
    }
}
