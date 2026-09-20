using UnityEngine;

[CreateAssetMenu(menuName = "Pyrrhic/NpcWeaponData")]
public class NpcWeaponData : ScriptableObject
{
    //How much can be carried in the firearm i.e. magazine/feed size
    public int Capacity = 30;
    //How much more can be carried with the unit (might want to change this to be relative to the unit instead)
    public int SpareCapacity = 180;
    public NpcAmmunitionData Ammunition;
    public GameObject Model;
    public AudioClip FireSound;
    public AudioClip ReloadSound;
}

