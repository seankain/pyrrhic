using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public string UnitName = "Unit";
    [SerializeField]
    private bool Destructable = true;
    [SerializeField]
    private float HitPoints = 100f;
    [SerializeField]
    private bool CanAttack = true;
    [SerializeField]
    private bool IsTransport = false;
    [SerializeField]
    private bool IsStationary = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
