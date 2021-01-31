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
    private OfflineFlycam flyCam;
    public GameObject debugCube;

    // Start is called before the first frame update
    void Start()
    {
        SelectButton.onClick.AddListener(() => { EnableSelect(true); EnableFly(false); });
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
        //var screenStart = playerCamera.WorldToScreenPoint(selectStart);
        //var screenEnd = playerCamera.WorldToScreenPoint(selectEnd);
        //TODO boxcast from camera
        var allSelectable = FindObjectsOfType<Selectable>();
        var selected = new List<Selectable>();
        foreach (var toDeselect in allSelectable) {
            var entScreenPos = playerCamera.WorldToScreenPoint(toDeselect.transform.position);
            Debug.Log(entScreenPos);
            if(entScreenPos.x > min.x && entScreenPos.x < max.x && entScreenPos.y > min.y && entScreenPos.y < max.y)
            {
                toDeselect.SetSelected(true);
            }
            else
            {
                toDeselect.SetSelected(false);
            }
        }

        //var collisions = Physics.OverlapBox((selectStart+selectEnd)/2,new Vector3(1,1,m_MaxDistance),
        //    Quaternion.LookRotation(playerCamera.transform.forward), LayerMask.GetMask(new string[] { "Default", "Selection" }));
        ////----DEBUG CUBE
        //Vector3[] corners = new Vector3[4];
        //playerCamera.CalculateFrustumCorners(Rect.zero,1, Camera.MonoOrStereoscopicEye.Mono, corners);
       
        //debugCube.transform.position = (selectStart + selectEnd) / 2;
        //debugCube.transform.rotation = Quaternion.LookRotation(playerCamera.transform.forward);
        //var dist = Vector3.Distance(currentSelectStart, currentSelectEnd);
        //debugCube.transform.localScale = new Vector3(
        //    Mathf.Abs(selectStart.x-selectEnd.x),
        //    Mathf.Abs(selectStart.y-selectEnd.y),
        //    m_MaxDistance);

        ////----END DEBUG CUBE

        //foreach (var c in collisions)
        //{
        //    Debug.Log($"Hit {c.name}");
        //    var s = c.gameObject.GetComponent<Selectable>();
        //    if(s != null)
        //    {
        //        s.SetSelected(true);
        //    }
        //}
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
        
    }
}
