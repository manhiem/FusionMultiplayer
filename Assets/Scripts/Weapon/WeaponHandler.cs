using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WeaponHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnFireChanged))]
    public bool isFiring { get; set; }
    float lastTimeFired = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Fire(Vector3 aimForwardVector)
    {

    }

    static void OnFireChanged(Changed<WeaponHandler> changed)
    {

    }

    void OnFireRemote()
    {

    }
}
