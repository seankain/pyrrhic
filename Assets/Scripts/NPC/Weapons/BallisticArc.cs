using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*
 I'm not totally sure if I need to do all this or just use the server side physics, im going to try and just use normal rigidbodies and if that 
 doesnt work come back to this
*/

public struct BallisticArcPoint
{
    public float Time;
    public Vector3 Place;
}
public class BallisticArc
{
    public List<BallisticArcPoint> Points;

}

