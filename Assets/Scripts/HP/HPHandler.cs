using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class HPHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    byte HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    bool isInitialized = false;
    const byte startingHP = 5;
    public Color uiOnHitColor;
    public Image uiOnHitImage;

    public SkinnedMeshRenderer bodyMeshRenderer;
    Color defaultMeshBodyColor;

    public GameObject _playerModel;
    public GameObject _deathGameObject; //Use object pooling

    HitboxRoot hitboxRoot;
    CharacterMovementHandler characterMovementHandler;
    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;
    private void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        hitboxRoot = GetComponentInChildren<HitboxRoot>();
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        networkPlayer = GetComponent<NetworkPlayer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        HP = startingHP;
        isDead = false;
        defaultMeshBodyColor = bodyMeshRenderer.material.color;

        isInitialized = true;
    }

    public void OnTakeDamage(string playerName)
    {
        if (isDead)
            return;

        HP -= 1;
        Debug.Log($"{Time.time} {transform.name} took damage got {HP} left");

        if (HP <= 0)
        {
            networkInGameMessages.SendInGameRPCMessages(playerName, $"Killed <b>{networkPlayer.nickName.ToString()}</b>");
            Debug.Log($"{Time.time} {transform.name} died");
            StartCoroutine(ServerReviveCO());
            isDead = true;
        }
    }

    IEnumerator OnHitCO()
    {
        bodyMeshRenderer.material.color = Color.white;

        if (Object.HasInputAuthority)
            uiOnHitImage.color = uiOnHitColor;

        yield return new WaitForSeconds(0.2f);

        bodyMeshRenderer.material.color = defaultMeshBodyColor;

        if (Object.HasInputAuthority && !isDead)
            uiOnHitImage.color = new Color(0, 0, 0, 0);
    }

    IEnumerator ServerReviveCO()
    {
        yield return new WaitForSeconds(2.0f);

        characterMovementHandler.RequestSpawn();
    }

    static void OnHPChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");
        byte newHP = changed.Behaviour.HP;

        changed.LoadOld();

        byte oldHP = changed.Behaviour.HP;

        if (newHP < oldHP)
        {
            changed.Behaviour.OnHPReduced();
        }
    }

    private void OnHPReduced()
    {
        if (!isInitialized)
            return;

        StartCoroutine(OnHitCO());
    }

    static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.HP}");

        bool isDeadCurrent = changed.Behaviour.isDead;

        changed.LoadOld();

        bool isDeadOld = changed.Behaviour.isDead;

        if (isDeadCurrent)
            changed.Behaviour.OnDeath();
        else if (!isDeadCurrent && isDeadOld)
            changed.Behaviour.OnRevive();
    }

    private void OnDeath()
    {
        Debug.Log($"{Time.time} onDeath");

        _playerModel.SetActive(false);
        hitboxRoot.HitboxRootActive = false;
        characterMovementHandler.SetCharacterControllerEnabled(false);

        Instantiate(_deathGameObject,
        transform.position,
        Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))
        );
    }

    private void OnRevive()
    {
        Debug.Log($"{Time.time} onRevive");

        if (Object.HasInputAuthority)
            uiOnHitImage.color = new Color(0, 0, 0, 0);

        _playerModel.SetActive(true);
        hitboxRoot.HitboxRootActive = true;
        characterMovementHandler.SetCharacterControllerEnabled(true);
    }

    public void OnRespawned()
    {
        HP = startingHP;
        isDead = false;
    }
}
