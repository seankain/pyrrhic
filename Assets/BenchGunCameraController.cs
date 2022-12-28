using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchGunCameraController : MonoBehaviour
{

    public OfflineFlycam flyCam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            flyCam.enabled = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            flyCam.enabled = false;
        }
    }
}
