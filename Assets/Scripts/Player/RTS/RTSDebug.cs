using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RTSDebug : MonoBehaviour
{
    public GameObject RtsHud;

    public SquareDrawer SelectDrawer;

    public Button AttackButton;
    public Button SelectButton;
    public Button MoveButton;
    [SerializeField]
    private Camera playerCamera;
    private bool m_HitDetect = false;
    private RaycastHit m_Hit;
    private float m_MaxDistance = 50;
    private Vector3 currentSelectMin;
    private Vector3 currentSelectMax;
    private bool selectEnabled = false;
    private bool flyEnabled = true;
    private OfflineFlycam flyCam;
    private List<Selectable> currentSelected = new List<Selectable>();
    public GameObject debugCube;
    private UnitCommands CurrentUnitCommand = UnitCommands.Move;

    // Start is called before the first frame update
    void Start()
    {
        SelectButton.onClick.AddListener(() => { selectEnabled = !selectEnabled; flyEnabled = !flyEnabled; EnableSelect(!selectEnabled); EnableFly(!flyEnabled); });
        SelectDrawer.OnSquareDrawn += HandleSelectDrawn;
        flyCam = GetComponent<OfflineFlycam>();
        EnableFly(true);
    }


    private void EnableSelect(bool enable=true) {
        SelectDrawer.gameObject.SetActive(enable); SelectDrawer.enabled = enable;
    }
    private void EnableFly(bool enable=true) 
    {
        flyCam.enabled = enable;
    }

    private void HandleSelectDrawn(Vector2 min, Vector2 max)
    {
        currentSelectMin = min;
        currentSelectMax = max;
        var allSelectable = FindObjectsOfType<Selectable>();
        currentSelected = new List<Selectable>();
        foreach (var result in allSelectable) {
            var entScreenPos = playerCamera.WorldToScreenPoint(result.transform.position);
            Debug.Log(entScreenPos);
            if(entScreenPos.x > min.x && entScreenPos.x < max.x && entScreenPos.y > min.y && entScreenPos.y < max.y)
            {
                result.SetSelected(true);
                currentSelected.Add(result);
            }
            else
            {
                result.SetSelected(false);
            }
        }
    }

    private void IssueCommand(UnitCommands command,Vector3 location, GameObject commandObject) 
    { 
        if(command == UnitCommands.Move)
        {
            foreach(var selected in currentSelected)
            {
                //TODO handle air moble capability
                var cap = selected.GetComponent<GroundMobileCapability>();
                
                cap.NavigateTo(location);
            }
        }
    }

    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var center = (currentSelectMin + currentSelectMax) / 2;
        //Check if there has been a hit yet
        if (m_HitDetect)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(center, playerCamera.transform.forward * m_Hit.distance);
            var dist = Vector3.Distance(currentSelectMin, currentSelectMax);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(center,new Vector3(dist,dist,dist));
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * m_MaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(playerCamera.transform.position + playerCamera.transform.forward * m_MaxDistance, playerCamera.transform.localScale);
        }
    }

    private bool TryGetClickLocation(out Vector3 location,out GameObject hitObject)
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(playerCamera.transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(playerCamera.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            location = hit.point;
            hitObject = hit.transform.gameObject;
            return true;
        }
        //else
        //{
        //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        //    Debug.Log("Did not Hit");
        //}
        location = Vector3.zero;
        hitObject = null;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EnableSelect(false);
            EnableFly(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EnableFly(false);
            EnableSelect(true);
        }
        if(Input.GetAxis("Fire1") > 0)
        {
            Debug.Log("FireFire");
            if (TryGetClickLocation(out var hitPoint,out var hitObject))
            {
                IssueCommand(CurrentUnitCommand, hitPoint,hitObject);
            }
            
        }
        
    }
}

public enum UnitCommands 
{ 
    Move,
    Attack,
    Board,
    Deboard,
    Deploy
}
