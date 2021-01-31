using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeployableUnit
{
    public GameObject UnitPrefab;
    public int Cost = 1;
    public int DeployerUpgradeThreshold = 1;
    public float DeployTimeSeconds = 1f;
}

public class DeployProgress
{
    public DeployProgress(DeployableUnit unit)
    {
        Unit = unit;
        SceneTimeStartSeconds = Time.time;
    }
    public DeployableUnit Unit;
    public float Progress = 0;
    public float SceneTimeStartSeconds = 0;
}

public class Deployer : MonoBehaviour
{
    public List<DeployableUnit> DeployableUnits;
    [SerializeField]
    private float MaxUnitSpawnDistance = 10f;
    [SerializeField]
    private float SpawnScanAngle = 360f;
    [SerializeField]
    private int SpawnAngleStep = 10;
    private List<DeployProgress> deploying;

    private Vector3 size = Vector3.one;
    public void BeginDeploy(int selection)
    {
        var unit = DeployableUnits[selection];
        deploying.Add(new DeployProgress(unit));
    }

    private void SpawnUnit(DeployableUnit unit) 
    {
        Instantiate(unit.UnitPrefab, GetSpawnLocation(unit), Quaternion.identity, null);
    }

    private Vector3 GetSpawnLocation(DeployableUnit unit)
    {
        // TODO use unit.UnitPrefab.GetComponent<Collider>().bounds to find more info about space needs
        var stepCount = Mathf.RoundToInt(SpawnScanAngle / SpawnAngleStep);
        for (var i = 0; i < stepCount; i++) 
        {
            // Does the ray intersect any objects excluding the player layer
            var angle = transform.eulerAngles.y - SpawnScanAngle / 2 + SpawnAngleStep * i;
            var dir = DirectionFromAngle(angle, true);
            var hits = Physics.RaycastAll(gameObject.transform.position, dir, MaxUnitSpawnDistance);
            bool open = false;
            foreach(var hit in hits)
            {
                //Hit anything that wasnt deployable itself
                if(hit.collider.gameObject != gameObject)
                {
                    open = false;
                    break;
                }
            }
            if (open)
            {
                return transform.position + dir + new Vector3(size.x,0,size.z);
            }
        }
        //TODO make a TryGet and then report if the entire area is full of stuff
        return transform.position + new Vector3(size.x, 10f, size.z);

    }

    /// <summary>
    /// Boosted from Sebastien Lague https://github.com/SebLague/Field-of-View/blob/master/Episode%2002/Scripts/FieldOfView.cs
    /// because he is the man
    /// </summary>
    /// <param name="angleInDegrees"></param>
    /// <param name="angleIsGlobal"></param>
    /// <returns></returns>
    private Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    // Start is called before the first frame update
    void Start()
    {
        size = GetComponent<Collider>().bounds.max;
    }

    // Update is called once per frame
    void Update()
    {
        for(var i = deploying.Count; i >= 0; i--)
        {
            deploying[i].Progress = (Time.time - deploying[i].SceneTimeStartSeconds) / deploying[i].Unit.DeployTimeSeconds;
            if (deploying[i].Progress >= 1f)
            {
                deploying.RemoveAt(i);
            }
        }
    }
}
