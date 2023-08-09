using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform cameraAnchorPoint;

    Vector2 _viewInput;

    float _camRotationX;
    float _camRotationY;

    NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;
    Camera _localCamera;

    private void Awake()
    {
        _localCamera = GetComponent<Camera>();
        _networkCharacterControllerPrototypeCustom = GetComponentInParent<NetworkCharacterControllerPrototypeCustom>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (_localCamera.enabled)
            _localCamera.transform.parent = null;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraAnchorPoint == null)
            return;

        if (!_localCamera.enabled)
            return;

        _localCamera.transform.position = cameraAnchorPoint.position;

        _camRotationX += _viewInput.y * Time.deltaTime * _networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;
        _camRotationX = Mathf.Clamp(_camRotationX, -90, 90);

        _camRotationY += _viewInput.x * Time.deltaTime * _networkCharacterControllerPrototypeCustom.rotationSpeed;

        _localCamera.transform.localRotation = Quaternion.Euler(_camRotationX, _camRotationY, 0);
    }

    public void SetViewInputVector(Vector2 vI)
    {
        _viewInput = vI;
    }
}
