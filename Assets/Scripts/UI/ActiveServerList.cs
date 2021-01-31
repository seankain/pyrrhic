using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActiveServerList : MonoBehaviour
{

    [SerializeField]
    private GameObject textAreaPrefab;
    private ScrollRect activeServerRect;
    private List<DiscoveryResponse> serverInfos = new List<DiscoveryResponse>();
    private List<GameObject> listItems = new List<GameObject>();
    public DiscoveryResponse SelectedItem;

    public void AddServer(DiscoveryResponse serverInfo)
    {
        serverInfos.Add(serverInfo);
        var y = serverInfos.Count * 5;
        var serverTextEntry = Instantiate(textAreaPrefab, activeServerRect.content.transform, false);
        var content = serverTextEntry.GetComponent<Text>();
        var btn = serverTextEntry.GetComponent<Button>();
    
        btn.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = $"{serverInfo.ServerUri} - {serverInfo.ServerName}";
        btn.onClick.AddListener(()=> { HandleServerSelected(serverInfo); });
        //var entry = serverTextEntry.GetComponent<ActiveServerEntry>();
        //entry.ServerInfo = serverInfo;
        //content.text = serverInfo.ServerUri.ToString();
        listItems.Add(serverTextEntry);
        
    }

    void HandleServerSelected(DiscoveryResponse serverInfo)
    {
        Debug.Log(serverInfo);
        SelectedItem = serverInfo;
    }


    private void RefreshServers()
    {
        //TODO
    }

    private void Awake()
    {
        activeServerRect = GetComponent<ScrollRect>();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
