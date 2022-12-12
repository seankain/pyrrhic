//using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PyrWeaponHandling : NetworkBehaviour
{
    [SerializeField]
    private GameObject CharacterRightHand;
    [SerializeField]
    private GameObject CharacterLeftHand;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private GameObject WeaponHoldLocation;
    [SerializeField]
    private GameObject CurrentWeapon;
    //TODO refactor to be generic weapon interface
    private FPPistol _pistol;
    private Vector3 originalWeaponHoldPosition;


    // Start is called before the first frame update
    void Start()
    {
        originalWeaponHoldPosition = WeaponHoldLocation.transform.localPosition;
    }

    private void OnEnable()
    {
        WeaponHoldLocation.SetActive(true);
        //Enable hands if not visible
        _pistol = CurrentWeapon.GetComponent<FPPistol>();
        _pistol.Draw();
    }

    private void OnDisable()
    {
        WeaponHoldLocation.SetActive(false);
        //Enable hands if not visible
        _pistol = CurrentWeapon.GetComponent<FPPistol>();

    }

    // Update is called once per frame
    void Update()
    {
        //TODO: this probably needs to be on
        //if (!isLocalPlayer) { return; }
        var firing = Input.GetAxis("Fire1");
        var reloading = Input.GetButtonDown("Reload");
        var sighting = Input.GetButton("Fire2");
        ToggleSighting(sighting);
        _pistol.ToggleDownSight(sighting);
        if (firing == 1)
        {
            _pistol.Fire();
        }
        else if (reloading)
        {
            Debug.Log("Reloading");
            _pistol.Reload();
        }

    }

    private void ToggleSighting(bool sighting)
    {
        if (sighting && WeaponHoldLocation.transform.localPosition != _pistol.IronSightPosition)
        {
            WeaponHoldLocation.transform.localPosition = Vector3.Lerp(WeaponHoldLocation.transform.localPosition, _pistol.IronSightPosition, Time.deltaTime * 10);
        }
        else
        {
            if (WeaponHoldLocation.transform.localPosition != originalWeaponHoldPosition)
            {
                WeaponHoldLocation.transform.localPosition = Vector3.Lerp(WeaponHoldLocation.transform.localPosition, originalWeaponHoldPosition, Time.deltaTime * 10);
            }
        }

    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!anim) { return; }
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.RightHand, WeaponHoldLocation.transform.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(0, 0, 0));

        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, WeaponHoldLocation.transform.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(0, 0, 0));

    }

}
