using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTarget : MonoBehaviour, IHittable
{
    public GameObject HitDecal;

    public void HandleHit(float damage, Vector3 location)
    {
        Instantiate(HitDecal, location, Quaternion.identity, null);
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
