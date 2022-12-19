using UnityEngine;

[CreateAssetMenu(menuName = "Pyrrhic/NpcAmmunitionData")]

public class NpcAmmunitionData : ScriptableObject
{
    public float ProjectileMassGrams;
    public float MuzzleVelocity;
    //For rpg,rocket etc, 0 for the rest
    public float SelfPropelledVelocity;
    //Usually one
    public int ProjectileCount;
    //Mainly for multi projectile spread, will probably just use it to calculate a larger sphere instead of individual instances
    public Vector3 Perturbation;
    // The whole unfired i.e. casing and bullet
    public GameObject UnfiredModel;
    // What gets fired
    public GameObject Model;
    //Does it have brass, some kind of spent artifact
    public GameObject SpentModel;

}

