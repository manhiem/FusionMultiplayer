using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WeaponHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnFireChanged))]
    public bool isFiring { get; set; }

    float lastTimeFired = 0;
    public ParticleSystem _fireParticleSystem;
    public Transform _aimPoint;
    public LayerMask _collisionLayers;

    HPHandler hPHandler;
    NetworkPlayer networkPlayer;

    private void Awake()
    {
        hPHandler = GetComponent<HPHandler>();
        networkPlayer = GetComponent<NetworkPlayer>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (hPHandler.isDead)
            return;

        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFireInputPressed)
            {
                Fire(networkInputData._aimForwardVector);
            }
        }
    }

    void Fire(Vector3 aimForwardVector)
    {
        if (Time.time - lastTimeFired < .15f)
            return;
        StartCoroutine(FireEffectCO());

        Runner.LagCompensation.Raycast(
            _aimPoint.position,
            aimForwardVector,
            100,
            Object.InputAuthority,
            out var hitInfo,
            _collisionLayers,
            HitOptions.IgnoreInputAuthority
            );
        float hitDistance = 100;
        bool isHitOtherPlayer = false;

        if (hitDistance > 0)
            hitDistance = hitInfo.Distance;

        if (hitInfo.Hitbox != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit hitbox {hitInfo.Hitbox.transform.root.name}");

            if (Object.HasStateAuthority)
                hitInfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTakeDamage(networkPlayer.nickName.ToString());
        }
        else if (hitInfo.Collider != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit PhysX collider {hitInfo.Hitbox.transform.root.name}");
        }

        if (isHitOtherPlayer)
            Debug.DrawRay(_aimPoint.position, aimForwardVector * hitDistance, Color.red, 1);
        else
            Debug.DrawRay(_aimPoint.position, aimForwardVector * hitDistance, Color.green, 1);

        lastTimeFired = Time.time;
    }

    IEnumerator FireEffectCO()
    {
        isFiring = true;
        _fireParticleSystem.Play();
        yield return new WaitForSeconds(0.09f);

        isFiring = false;
    }

    static void OnFireChanged(Changed<WeaponHandler> changed)
    {
        // Debug.Log($"{Time.time} OnFireChanged value {changed.Behaviour.isFiring}");

        bool isFiringCurrent = changed.Behaviour.isFiring;
        changed.LoadOld();

        bool isFiringOld = changed.Behaviour.isFiring;

        if (isFiringCurrent && !isFiringOld)
        {
            changed.Behaviour.OnFireRemote();
        }
    }

    void OnFireRemote()
    {
        if (!Object.HasInputAuthority)
            _fireParticleSystem.Play();
    }
}
