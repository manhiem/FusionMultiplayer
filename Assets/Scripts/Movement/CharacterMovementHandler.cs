using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    Vector2 _viewInput;
    float _camRotationX = 0;
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

    // Update is called once per frame
    void Update()
    {
        _camRotationX += _viewInput.x * Time.deltaTime * _networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;
        _camRotationX = Mathf.Clamp(_camRotationX, -90, 90);

        _localCamera.transform.localRotation = Quaternion.Euler(_camRotationX, 0, 0);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            _networkCharacterControllerPrototypeCustom.Rotate(networkInputData._rotationInput);

            Vector3 _moveDirection = transform.forward * networkInputData._movementInput.y + transform.right * networkInputData._movementInput.x;
            _moveDirection.Normalize();

            _networkCharacterControllerPrototypeCustom.Move(_moveDirection);

            if (networkInputData.isJumpPressed)
                _networkCharacterControllerPrototypeCustom.Jump();
        }
    }

    public void SetViewInputVector(Vector2 vI)
    {
        _viewInput = vI;
    }
}
