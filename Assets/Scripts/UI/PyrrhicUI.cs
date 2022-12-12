using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public enum PyrrhicTeam { Boot,Strategist,Spectator }
public class PyrrhicUI : MonoBehaviour
{
    public delegate void TeamSelected(PyrrhicTeam selection);
    public event TeamSelected OnTeamSelected;

    public Button HostButton;
    public Button JoinButton;
    public ActiveServerList ActiveServerScroll;
    //public PyrricNetworkManager NetMgr;
    public PyrricNetworkDiscovery NetDisc;
    public Camera MainMenuCamera;
    public GameObject TeamSelectPanel;
    public GameObject MainPanel;
    public Button JoinTeamBootButton;
    public Button JoinTeamStratButton;
    

    private Canvas guiCanvas;

    private void Awake()
    {
        guiCanvas = gameObject.GetComponentInChildren<Canvas>();
        ActiveServerScroll = GetComponentInChildren<ActiveServerList>();
    }
    // Start is called before the first frame update
    void Start()
    {
        HostButton.onClick.AddListener(HandleHost);
        JoinButton.onClick.AddListener(HandleJoin);
        //NetDisc.HostFound += HandleHostFound;
        //NetDisc.BroadcastDiscoveryRequest();

    }

    private void HandleHostFound(DiscoveryResponse response)
    {
        Debug.Log("Host found!");
        ActiveServerScroll.AddServer(response);   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActiveServerScroll.AddServer(new DiscoveryResponse { ServerUri = new System.Uri("tcp:\\\\127.0.0.1"), ServerName = "Localhost" });
        }
    }

    public void ShowTeamSelect() {
        MainPanel.SetActive(false);
        TeamSelectPanel.SetActive(true);
    }

    public void HideCanvas() {
        TeamSelectPanel.SetActive(false);
        MainPanel.SetActive(false);
        guiCanvas.enabled = false;
    }

    public void ShowCanvas() {
        guiCanvas.enabled = true;
        MainPanel.SetActive(true);
    }

    void HandleJoin() 
    {
        //This is the Mirror version of the networking, remove when ported to unity netcode
        //NetDisc.StopDiscovery();
        //NetMgr.StartClient(ActiveServerScroll.SelectedItem.ServerUri);
        NetworkManager.Singleton.StartClient();

    }

    void HandleHost() {
        //NetDisc.StopDiscovery();
        //NetMgr.StartHost();
        Debug.Log("Disabling main ui panel");
        NetworkManager.Singleton.StartHost();
        MainPanel.SetActive(false);
        MainMenuCamera.gameObject.SetActive(false);
    }

    void HandleHostStarted() { }
}
