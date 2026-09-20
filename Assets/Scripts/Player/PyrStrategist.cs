using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PyrStrategist : NetworkBehaviour
{

    [SerializeField]
    private List<GameObject> TeamEntityPrefabs;
    [SerializeField]
    private Camera cam;
    private int currentSpawnSlot = 0;

    private List<GameObject> activeTeamEntities = new List<GameObject>();
    private bool spawning = false;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>(true);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1") && spawning == false) {
            spawning = true;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray,out var hitInfo,100)) 
            {
                //CmdSpawnEntity(hitInfo.point);
                Debug.Log("TODO: spawn entities with unity netcode");
            }
            
        }
        if(Input.GetButtonUp("Fire1"))
        {
            spawning = false;
        }
        if(Input.GetButtonDown("Fire2")) { }
    }

    //[Command]
    //void CmdSpawnEntity(Vector3 position) {
    //    var prefab = TeamEntityPrefabs[currentSpawnSlot];
    //    var teamEnt = Instantiate(prefab, position, Quaternion.identity, null);
    //    activeTeamEntities.Add(teamEnt);
    //    NetworkServer.Spawn(teamEnt,this.gameObject);
    //}


}
