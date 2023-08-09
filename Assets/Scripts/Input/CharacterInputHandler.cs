using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector3 _moveInputVector = Vector2.zero;
    Vector2 _viewInputVector = Vector2.zero;
    bool _isJumpPressed = false;
    bool _isFirePressed = false;

    LocalCameraHandler localCameraHandler;
    CharacterMovementHandler characterMovementHandler;
    // Start is called before the first frame update
    void Awake()
    {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>() >;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (!characterMovementHandler.Object.HasAuthority)
            return;

        _viewInputVector.x = Input.GetAxis("Mouse X");
        _viewInputVector.y = Input.GetAxis("Mouse Y") * -1;

        _moveInputVector.x = Input.GetAxis("Horizontal");
        _moveInputVector.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
            _isJumpPressed = true;

        if (Input.GetButtonDown("Fire1"))
            _isFirePressed = true;

        localCameraHandler.SetViewInputVector(_viewInputVector);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData._aimForwardVector = localCameraHandler.transform.forward;

        networkInputData._movementInput = _moveInputVector;

        networkInputData.isJumpPressed = _isJumpPressed;
        networkInputData.isFireInputPressed = _isFirePressed;
        _isJumpPressed = false;
        _isFirePressed = false;

        return networkInputData;
    }
}
