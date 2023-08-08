using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector3 _moveInputVector = Vector2.zero;
    Vector2 _viewInputVector = Vector2.zero;
    bool _isJumpPressed = false;

    CharacterMovementHandler characterMovementHandler;
    // Start is called before the first frame update
    void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
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
        _viewInputVector.x = Input.GetAxis("Mouse X");
        _viewInputVector.y = Input.GetAxis("Mouse Y") * -1;

        characterMovementHandler.SetViewInputVector(_viewInputVector);

        _moveInputVector.x = Input.GetAxis("Horizontal");
        _moveInputVector.y = Input.GetAxis("Vertical");

        _isJumpPressed = Input.GetButtonDown("Jump");
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData._rotationInput = _viewInputVector.x;

        networkInputData._movementInput = _moveInputVector;

        networkInputData.isJumpPressed = _isJumpPressed;

        return networkInputData;
    }
}
