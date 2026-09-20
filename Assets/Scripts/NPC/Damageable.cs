
using System.Collections;
using UnityEngine;


//Not well thought out yet but I need it for the state machine

public class Damageable : UnitCapability
{

    public void HandleHit(float damage)
    {
        unitBase.HitPoints -= damage;
        if (unitBase.HitPoints <= 0)
        {
            StartCoroutine("Die");
        }
    }

    public IEnumerator Die()
    {
        anim.Play("Die");
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            yield return null;
        }
        //if (deathGibPrefab != null)
        //{
        //    Instantiate(deathGibPrefab, gameObject.transform.position, Quaternion.identity, null);
        //}
        //Destroy(gameObject);

    }

    protected override void OnStarting()
    {

    }
}

