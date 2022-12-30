using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitChecker : MonoBehaviour
{
    public GameObject ProjectileBasePrefab;

    public ConcurrentQueue<BallisticArc> QueuedRounds;
    public List<GameObject> ActiveRounds;
    public float CheckMaxDistance = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        QueuedRounds = new ConcurrentQueue<BallisticArc>();
    }

    // Update is called once per frame
    void Update()
    {
        if(QueuedRounds.TryDequeue(out var arc))
        {
            var projectile = Instantiate(ProjectileBasePrefab, arc.Points[0].Place, Quaternion.identity, null);
            var pyrProjectile = projectile.GetComponent<PyrProjectileOffline>();
            pyrProjectile.Info = arc.ProjectileInfo;
            pyrProjectile.Send();
        }
    }

    public IEnumerator RunHitCheckCoroutine(BallisticArc arc, float maxLifeTimeSeconds = 10f)
    {
        float elapsed = 0;
        var projectile = Instantiate(ProjectileBasePrefab, arc.Points[0].Place, Quaternion.identity, null);
        while (elapsed <= maxLifeTimeSeconds)
        {
            // If for some reason the arc exists in the past throw away the past positions, if there are none remaining then just quit
            var currentPoints = arc.Points.Where(x => x.Time > Time.realtimeSinceStartup).ToList();
            if (currentPoints.Count < 1) { yield break; }
            var pyrProjectile = projectile.GetComponent<PyrProjectileOffline>();
            foreach (var c in currentPoints)
            {
                if (Within(c.Time, Time.realtimeSinceStartup,CheckMaxDistance))
                {
                    //Physics.Raycast(c.Place,Vector3.AngleBetween(c.place,))
                    projectile.transform.SetPositionAndRotation(c.Place, Quaternion.identity);
                    if(pyrProjectile.CurrentContacts.Count > 0)
                    {
                        Debug.Log($"Hit checker has a hit on {pyrProjectile.CurrentContacts[0].name}");
                    }

                }
            }
            elapsed += Time.deltaTime;
        }
    }

    private bool Within(float a, float b, float maxDistance = 0.001f)
    {
        return Mathf.Abs(a - b) <= maxDistance;
    }

}
