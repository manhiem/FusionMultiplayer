using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer local { get; set; }
    public Transform playerModel;

    public TextMeshProUGUI playerNickName;

    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName { get; set; }
    public bool isPublicJoinMessageSent = false;
    NetworkInGameMessages networkInGameMessages;

    public LocalCameraHandler localCameraHandler;
    public GameObject localUI;

    private void Awake()
    {
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
    }
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

            RPC_SetNickname(PlayerPrefs.GetString("PlayerNickname"));

            Debug.Log("Spawned local player");
        }
        else
        {
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;
            localUI.SetActive(false);

            Debug.Log("Spawned remote player");
        }

        Runner.SetPlayerObject(Object.InputAuthority, Object);

        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Object.HasStateAuthority)
        {
            if (Runner.TryGetPlayerObject(player, out NetworkObject playerLeftNetworkObject))
            {
                if (playerLeftNetworkObject == Object)
                {
                    local.GetComponent<NetworkInGameMessages>().
                        SendInGameRPCMessages(
                        playerLeftNetworkObject.
                        GetComponent<NetworkPlayer>().nickName.ToString(),
                        "left"
                        );
                }
            }

        }

        if (player == Object.HasInputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    private void OnNickNameChanged()
    {
        Debug.Log($"Nickname changed for the player to {nickName} for player {gameObject.name}");

        playerNickName.text = nickName.ToString();
    }

    static void OnNickNameChanged(Changed<NetworkPlayer> changed)
    {
        Debug.Log($"{Time.time} OnNickNameChanged value {changed.Behaviour.nickName}");

        changed.Behaviour.OnNickNameChanged();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickname(string nickName, RpcInfo info = default)
    {
        Debug.Log($"[RPC] SetNickname {nickName}");
        this.nickName = nickName;

        if (!isPublicJoinMessageSent)
        {
            networkInGameMessages.SendInGameRPCMessages(nickName, "Joined!");

            isPublicJoinMessageSent = true;
        }
    }

}
