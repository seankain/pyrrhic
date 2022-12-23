using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PyrrhicPlayer : NetworkBehaviour, INetworkSerializable
{
    [SerializeField]
    private NetworkVariable<string> Name = new NetworkVariable<string>("Monkey Nuts", NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    [SerializeField]
    private NetworkVariable<PyrrhicTeam> Team = new NetworkVariable<PyrrhicTeam>(PyrrhicTeam.Spectator, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField]
    private NetworkVariable<float> Health = new NetworkVariable<float>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<bool> IsDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private enum PlayerState { Joining, Playing, Spectating }
    private PlayerState PlayerCurrentState = PlayerState.Joining;
    public Camera playerCamera;

    [SerializeField]
    GameObject playerAvatar;

    [SerializeField]
    private PyrrhicUI ui;

    private PyrServerInfo serverInfo;

    private void Awake()
    {
        ui = FindObjectOfType<PyrrhicUI>();
        serverInfo = FindObjectOfType<PyrServerInfo>();

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Team.OnValueChanged += (prev,next) => { Debug.Log($"{gameObject.name} changed team"); };
        Health.OnValueChanged += (prev, next) => { Debug.Log($"{gameObject.name} took damage down to {next} health"); };
        Name.OnValueChanged += (prev, next) => { Debug.Log($"{gameObject.name} changed player name from {prev} to {next}"); };
        OnStartLocalPlayer();

    }

    public void OnStartLocalPlayer()
    {
        //base.OnStartLocalPlayer();
        ui.JoinTeamBootButton.onClick.AddListener(() => { Debug.Log("Request To join boot"); SendTeamJoinRequestServerRpc(PyrrhicTeam.Boot); });
        ui.JoinTeamStratButton.onClick.AddListener(() => { Debug.Log("Request to join strategist"); SendTeamJoinRequestServerRpc(PyrrhicTeam.Strategist); });

        Debug.Log($"Enabling {playerCamera.name}");
        playerCamera.gameObject.SetActive(true);

        var cams = FindObjectsOfType<Camera>();
        foreach (var cam in cams) { if (cam.name == "MainCamera") { Debug.Log($"Disabling {cam.name}"); cam.enabled = false; } }
        //camera = gameObject.GetComponentInChildren<Camera>(true);
        ui.TeamSelectPanel.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer && Team.Value == PyrrhicTeam.Boot)
        {
            playerAvatar.SetActive(true);
            return;
        }
        OnStartLocalPlayer();


    }


    // Update is called once per frame
    void Update()
    {
    }

    private void SetCursorState(bool onUi)
    {
        //TODO: it needs to be handled differently for RTS versus FPS player types
        //CursorLockMode.Confined;
        Debug.Log($"Cursor state {onUi}");
        if (onUi)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            return;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    [ClientRpc]
    private void SetTeamClientRpc(ulong targetId,PyrrhicTeam assignedTeam)
    {
        Debug.Log($"Client set to {assignedTeam}");

        ui.HideCanvas();
        SetCursorState(false);
        HandleTeamChange(assignedTeam);
    }

    [ClientRpc]
    public void HandleHitClientRpc(ulong clientId,float damage)
    {
        if (OwnerClientId == clientId)
        {
            Health.Value -= damage;
            Debug.Log($"{Name} hit for {damage} damage");
            if (Health.Value <= 0) { Die(); }
        }
    }

    public void Die()
    {
        Debug.Log($"{Name.Value} died");
        IsDead.Value = true;
    }

    private void EnableSpectateMotion() 
    {
        gameObject.GetComponent<PyrrhicPlayerMovement>().enabled = false;
        //gameObject.GetComponent<HeadBob>().enabled = false;
        gameObject.GetComponent<PyrWeaponHandling>().enabled = false;
        gameObject.GetComponent<PyrStrategist>().enabled = false;
        gameObject.GetComponentInChildren<Animator>().enabled = false;
        //Flycam related components:
        gameObject.GetComponent<ExtendedFlycam>().enabled = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

    }
    private void EnableFPS() 
    {
        gameObject.GetComponent<PyrrhicPlayerMovement>().enabled = true;
        //gameObject.GetComponent<HeadBob>().enabled = true;
        gameObject.GetComponent<PyrWeaponHandling>().enabled = true;
        gameObject.GetComponent<PyrStrategist>().enabled = false;
        gameObject.GetComponentInChildren<Animator>().enabled = true;
        //Flycam related components:
        gameObject.GetComponent<ExtendedFlycam>().enabled = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void EnableRTS()
    {
        gameObject.GetComponent<PyrrhicPlayerMovement>().enabled = false;
        //gameObject.GetComponent<HeadBob>().enabled = false;
        gameObject.GetComponent<PyrWeaponHandling>().enabled = false;
        gameObject.GetComponentInChildren<Animator>().enabled = false;
        //Flycam related components:
        gameObject.GetComponent<ExtendedFlycam>().enabled = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<PyrStrategist>().enabled = true;
    }

    private void HandleTeamChange(PyrrhicTeam assignedTeam)
    {
        if (assignedTeam == PyrrhicTeam.Boot)
        {
            EnableFPS();
        }
        if (assignedTeam == PyrrhicTeam.Strategist)
        {
            EnableRTS();
        }

    }

    [ServerRpc]
    public void PromptTeamSelectServerRpc()
    {
        SetCursorState(true);
        ui.ShowTeamSelect();
    }

    [ServerRpc]
    private void SendTeamJoinRequestServerRpc(PyrrhicTeam team)
    {
        //Default is spectator
        //TODO: handle when player selects full team
        Debug.Log("Server side team join request");
        var gameInfo = FindObjectOfType<PyrrhicGame>();
        if (team == PyrrhicTeam.Boot)
        {
            if (serverInfo.CurrentBootTeamSize < serverInfo.MaxBootTeamSize)
            {
                team = PyrrhicTeam.Boot;
                Debug.Log($"{Name} Joined boot");
                gameInfo.AddPlayerToTeamServerRpc(this.OwnerClientId, Name.Value, PyrrhicTeam.Boot);
                SetTeamClientRpc(this.OwnerClientId, PyrrhicTeam.Boot);
            }
        }
        else if (team == PyrrhicTeam.Strategist)
        {
            if (serverInfo.CurrentStrategistTeamSize < serverInfo.MaxStrategistTeamSize)
            {
                Debug.Log($" {Name} Joined Strategist");
                team = PyrrhicTeam.Strategist;
                gameInfo.AddPlayerToTeamServerRpc(this.OwnerClientId, Name.Value, PyrrhicTeam.Strategist);
                SetTeamClientRpc(this.OwnerClientId, PyrrhicTeam.Strategist);
            }

        }

    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        throw new System.NotImplementedException();
    }
}
