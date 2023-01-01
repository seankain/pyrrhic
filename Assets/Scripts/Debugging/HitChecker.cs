using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class HitChecker : MonoBehaviour
{
    public GameObject ProjectileBasePrefab;

    //public ConcurrentQueue<BallisticArc> QueuedRounds;
    public int PoolSize = 100;
    public List<PyrProjectileOffline> InactivePool;
    public List<PyrProjectileOffline> ActivePool;
    public float CheckMaxDistance = 0.1f;
    public bool UseJobs = false;
    // Start is called before the first frame update
    void Start()
    {
        //QueuedRounds = new ConcurrentQueue<BallisticArc>();
        StartCoroutine(FillPool());
    }

    // Update is called once per frame
    void Update()
    {
        if (UseJobs)
        {
            RunChecksJobs();
        }
        else
        {
            RunChecks();
        }
    }

    public void Add(ProjectileInfo info)
    {
        ActivePool.Add(GetFromInactivePool(info));
    }
    private IEnumerator FillPool()
    {
        for (var i = 0; i < PoolSize; i++)
        {
            var projectile = Instantiate(ProjectileBasePrefab, null);
            var pyrProj = projectile.GetComponent<PyrProjectileOffline>();
            pyrProj.gameObject.SetActive(false);
            InactivePool.Add(pyrProj);
            yield return null;
        }
    }

    private PyrProjectileOffline GetFromInactivePool(ProjectileInfo info)
    {
        var inactiveProj = InactivePool.FirstOrDefault();
        //Grow it if we request more than what is in inactive
        if (inactiveProj == null)
        {
            Debug.Log("Growing hitchecker projectile pool size, initial pool size exceeded");
            var projectile = Instantiate(ProjectileBasePrefab, null);
            inactiveProj = projectile.GetComponent<PyrProjectileOffline>();
        }
        else
        {
            InactivePool.Remove(inactiveProj);
        }
        //reset lifetime and info
        inactiveProj.gameObject.SetActive(true);
        inactiveProj.elapsed = 0;
        inactiveProj.Info = info;
        inactiveProj.currentVelocity = inactiveProj.transform.forward * info.MuzzleVelocity;
        //TODO not being put in the right place, need to set it to muzzle position
        return inactiveProj;
    }

    public void RunChecks()
    {
        foreach(var round in ActivePool)
        {
            round.elapsed += Time.deltaTime;
            IntegrationMethods.Heuns(round.elapsed, transform.position, round.currentVelocity, Vector3.zero,
                new BulletData
                {
                    muzzleVelocity = round.Info.MuzzleVelocity,
                    m = round.Info.MassGrams / 1000,
                    C_d = round.Info.BallisticCoefficientG1,
                    r = round.Info.RadiusMillimeters / 1000,
                    rho = 1.204f
                },
                out var nextPosition,
                out var nextVelocity,
                out var angleDelta);
            round.currentVelocity = nextVelocity;
            if (Physics.Raycast(round.transform.position, round.transform.forward, out var hit, Vector3.Distance(round.transform.position, nextPosition), ~0, QueryTriggerInteraction.Ignore))
            {

                Debug.Log($"hit {hit.collider.name} after {round.elapsed} seconds");
                var hittable = hit.collider.gameObject.GetComponent<IHittable>();
                if (hittable != null)
                {
                    hittable.HandleHit(round.Damage, hit.point);
                }
                round.transform.position = nextPosition;
                //Move to inactive
                round.gameObject.SetActive(false);
                ActivePool.Remove(round);
                InactivePool.Add(round);
            }
            round.transform.position = nextPosition;
        }
    }

    private void RunChecksJobs()
    {
        NativeArray<RaycastCommand> raycastCommands = new NativeArray<RaycastCommand>(ActivePool.Count,Allocator.TempJob);
        NativeArray<RaycastHit> raycastHits = new NativeArray<RaycastHit>(ActivePool.Count, Allocator.TempJob);
        for(var i = 0; i < ActivePool.Count; i++)
        {
            
            IntegrationMethods.Heuns(ActivePool[i].elapsed, transform.position, ActivePool[i].currentVelocity, Vector3.zero,
                new BulletData
                {
                    muzzleVelocity = ActivePool[i].Info.MuzzleVelocity,
                    m = ActivePool[i].Info.MassGrams / 1000,
                    C_d = ActivePool[i].Info.BallisticCoefficientG1,
                    r = ActivePool[i].Info.RadiusMillimeters / 1000,
                    rho = 1.204f
                },
                out var nextPosition,
                out var nextVelocity,
                out var angleDelta);

            raycastCommands[i] = new RaycastCommand(
                ActivePool[i].gameObject.transform.position,
                ActivePool[i].gameObject.transform.forward,
                Vector3.Distance(transform.position, nextPosition), ~0);
            ActivePool[i].transform.position = nextPosition;
            ActivePool[i].currentVelocity = nextVelocity;
        }

        JobHandle castJobs = RaycastCommand.ScheduleBatch(raycastCommands, raycastHits, 1);
        castJobs.Complete();
        for(var i = 0; i < ActivePool.Count; i++)
        {
            if(raycastHits[i].collider != null)
            {
                Debug.Log($"hit {raycastHits[i].collider.name} after {ActivePool[i].elapsed} seconds");
                var hittable = raycastHits[i].collider.gameObject.GetComponent<IHittable>();
                if (hittable != null)
                {
                    hittable.HandleHit(ActivePool[i].Damage, raycastHits[i].point);
                }
            }
        }
        raycastCommands.Dispose();
        raycastHits.Dispose();
    }

    private bool Within(float a, float b, float maxDistance = 0.001f)
    {
        return Mathf.Abs(a - b) <= maxDistance;
    }

}
