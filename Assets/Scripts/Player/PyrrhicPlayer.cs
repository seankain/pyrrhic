using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyrrhicPlayer : NetworkBehaviour
{
    [SyncVar]
    public string Name = "Monkey Nuts";
    [SyncVar]
    public PyrrhicTeam Team = PyrrhicTeam.Spectator;
    [SyncVar]
    public float Health = 100;
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

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        ui.JoinTeamBootButton.onClick.AddListener(() => { Debug.Log("Request To join boot"); SendTeamJoinRequest(PyrrhicTeam.Boot); });
        ui.JoinTeamStratButton.onClick.AddListener(() => { Debug.Log("Request to join strategist"); SendTeamJoinRequest(PyrrhicTeam.Strategist); });

        Debug.Log($"Enabling {playerCamera.name}");
        playerCamera.gameObject.SetActive(true);

        var cams = FindObjectsOfType<Camera>();
        foreach (var cam in cams) { if (cam.name == "MainCamera") { Debug.Log($"Disabling {cam.name}"); cam.enabled = false; } }
        //camera = gameObject.GetComponentInChildren<Camera>(true);

    }

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer && Team == PyrrhicTeam.Boot)
        {
            playerAvatar.SetActive(true);
        }


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

    [TargetRpc]
    private void SetTeam(NetworkConnection target, PyrrhicTeam assignedTeam)
    {
        Debug.Log($"Client set to {assignedTeam}");

        ui.HideCanvas();
        SetCursorState(false);
        HandleTeamChange(assignedTeam);
    }

    [TargetRpc]
    public void HandleHit(NetworkConnection target, float damage)
    {

        Health -= damage;
        Debug.Log($"{Name} hit for {damage} damage");
        if (Health <= 0) { Die(); }
    }

    public void Die()
    {
        Debug.Log($"{Name} died");
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

    [TargetRpc]
    public void PromptTeamSelect(NetworkConnection target)
    {
        SetCursorState(true);
        ui.ShowTeamSelect();
    }

    [Command]
    private void SendTeamJoinRequest(PyrrhicTeam team)
    {
        //Default is spectator
        //TODO: handle when player selects full team
        Debug.Log("Server side team join request");
        if (team == PyrrhicTeam.Boot)
        {
            if (serverInfo.CurrentBootTeamSize < serverInfo.MaxBootTeamSize)
            {
                team = PyrrhicTeam.Boot;
                Debug.Log($"{Name} Joined boot");
                SetTeam(this.connectionToClient, PyrrhicTeam.Boot);
            }
        }
        else if (team == PyrrhicTeam.Strategist)
        {
            if (serverInfo.CurrentStrategistTeamSize < serverInfo.MaxStrategistTeamSize)
            {
                Debug.Log($" {Name} Joined Strategist");
                team = PyrrhicTeam.Strategist;
                SetTeam(this.connectionToClient, PyrrhicTeam.Strategist);
            }

        }

    }


}
