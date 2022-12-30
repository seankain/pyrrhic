using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BenchGun : MonoBehaviour
{

    /// <summary>
    /// Mass grams
    /// </summary>
    public float ProjectileMass;
    /// <summary>
    /// Meters per second
    /// </summary>
    public float MuzzleVelocity;

    /// <summary>
    /// Millimeters
    /// </summary>
    public float ProjectileRadius;

    /// <summary>
    /// G1
    /// </summary>
    public float BallisticCoefficient;

    [SerializeField]
    private Transform Turret;
    [SerializeField]
    private Transform Muzzle;
    private LineRenderer lineRenderer;

    public float TurretAngle
    {
        get { return turretAngle; }
        set
        {
            if (value < 0)
            {
                turretAngle = 0;
            }
            else if (value > 90)
            {
                turretAngle = 90;
            }
            else
            {
                turretAngle = value;
            }
            Turret.transform.SetPositionAndRotation(Turret.transform.position, Quaternion.Euler(turretAngle, 0, 0));
        }
    }

    private float turretAngle = 90;

    public void Fire(ProjectileInfo projectileInfo, int steps = 100, float stepResolution = 0.01f)
    {
        var arc = new BallisticArc(Muzzle, projectileInfo, Time.realtimeSinceStartup, steps, stepResolution: stepResolution);
        lineRenderer.positionCount = steps;
        lineRenderer.SetPositions(arc.Points.Select(x => x.Place).ToArray());
        FindObjectOfType<HitChecker>().QueuedRounds.Enqueue(arc);
        //FireForwardRay();
    }

    private void FireForwardRay()
    {
        lineRenderer.SetPositions(new Vector3[] { Muzzle.position, Muzzle.forward * 1000 });
    }

    // Start is called before the first frame update
    void Start()
    {
        Turret.transform.Rotate(Turret.position, turretAngle);
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
