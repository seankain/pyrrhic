using UnityEngine;

/// <summary>
/// TODO: moveout of npc scripts since this will probably be how all ammunition data is stored
/// </summary>
[CreateAssetMenu(menuName = "Pyrrhic/NpcAmmunitionData")]
public class NpcAmmunitionData : ScriptableObject
{
    public string Name;
    public float ProjectileMassGrams;
    public float MuzzleVelocity;
    public float RadiusMillimeters;
    public float BallisticCoefficientG1;
    public float Damage = 10f;
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

    //Generate inputs for trajectory calculations
    public ProjectileInfo Info
    {
        get {
            return new ProjectileInfo
            {
                MassGrams = ProjectileMassGrams,
                BallisticCoefficientG1 = BallisticCoefficientG1,
                RadiusMillimeters = RadiusMillimeters,
                MuzzleVelocity = MuzzleVelocity
            };
        }
    }
}

