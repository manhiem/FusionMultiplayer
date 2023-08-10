using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    bool isRespawnRequested = false;

    NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;
    HPHandler hPHandler;
    public Camera _localCamera;
    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;
    private void Awake()
    {
        _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        hPHandler = GetComponent<HPHandler>();
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        networkPlayer = GetComponent<NetworkPlayer>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (isRespawnRequested)
            {
                Respawn();
                return;
            }

            if (hPHandler.isDead)
                return;
        }

        if (GetInput(out NetworkInputData networkInputData))
        {
            transform.forward = networkInputData._aimForwardVector;

            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;

            Vector3 _moveDirection = transform.forward * networkInputData._movementInput.y + transform.right * networkInputData._movementInput.x;
            _moveDirection.Normalize();

            _networkCharacterControllerPrototypeCustom.Move(_moveDirection);

            if (networkInputData.isJumpPressed)
                _networkCharacterControllerPrototypeCustom.Jump();

            CheckFallRespawn();
        }
    }

    void CheckFallRespawn()
    {
        if (transform.position.y < -12)
        {
            if (Object.HasStateAuthority)
            {
                Debug.Log($"{Time.time} Respawn due to fall outside of map at position {transform.position}");
                networkInGameMessages.SendInGameRPCMessages(networkPlayer.nickName.ToString(), "fell off the world");
                Respawn();
            }
        }
    }

    public void RequestSpawn()
    {
        isRespawnRequested = true;
    }

    void Respawn()
    {
        _networkCharacterControllerPrototypeCustom.TeleportToPosition(Utils.GetRandomSpawnPoint());
        hPHandler.OnRespawned();
        isRespawnRequested = false;
    }

    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        _networkCharacterControllerPrototypeCustom.Controller.enabled = isEnabled;
    }
}
