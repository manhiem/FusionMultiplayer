using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkInGameMessages : NetworkBehaviour
{
    MessageUIHandler messageUIHandler;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void SendInGameRPCMessages(string username, string message)
    {
        RPC_InGameMessages($"<b>{username}</b> {message}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_InGameMessages(string message, RpcInfo info = default)
    {
        Debug.Log($"[RPC] InGameMessage {message}");

        if (messageUIHandler == null)
        {
            messageUIHandler = NetworkPlayer.local.localCameraHandler.GetComponentInChildren<MessageUIHandler>();
        }

        if (messageUIHandler != null)
        {
            messageUIHandler.OnGameMessageRecieved(message);
        }
    }

}
