using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;
    public Camera _localCamera;

    private void Awake()
    {
        _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void FixedUpdateNetwork()
    {
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
            transform.position = Utils.GetRandomSpawnPoint();
        }
    }
}
