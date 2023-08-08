using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 _movementInput;
    public float _rotationInput;
    public NetworkBool isJumpPressed;
}
