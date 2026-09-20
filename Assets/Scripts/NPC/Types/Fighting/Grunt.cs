using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grunt : ScriptableObject
{
    public float HitPoints = 100f;
    public float MoveSpeed = 3f;
    public GameObject CharacterModel;
    public Animator Animator;
    public GameObject[] Weapons;
}
