using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer local { get; set; }
    public Transform playerModel;
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            local = this;

            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            Debug.Log("Spawned local player");
        }
        else
        {
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            Debug.Log("Spawned remote player");
        }

        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.HasInputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

}
