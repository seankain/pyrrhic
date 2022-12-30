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

public class BallisticArcPoint
{
    public float Time;
    public Vector3 Place;
    public Vector3 Velocity;
    //Not going to deal with wind and such, this is going to be 1D
    public float AngleDelta;
    public float TimeDelta;
}

public class ProjectileInfo
{
    public float MassGrams;
    public float RadiusMillimeters;
    public float BallisticCoefficientG1;
    public float MuzzleVelocity;
}
public class BallisticArc
{
    public List<BallisticArcPoint> Points;
    public ProjectileInfo ProjectileInfo;

    public BallisticArc(Transform startingTransform, ProjectileInfo projectile, float startTime, int steps, float stepResolution = 0.001f)
    {
        this.Points = new List<BallisticArcPoint>();
        this.ProjectileInfo = projectile;
        var prevPosition = startingTransform.position;
        var prevVelocity = startingTransform.forward * projectile.MuzzleVelocity;
        for (var i = 0; i < steps; i++)
        {
            //TODO change bullet data to projectile struct
            IntegrationMethods.Heuns(stepResolution, prevPosition, prevVelocity, Vector3.zero,
                new BulletData
                {
                    muzzleVelocity = projectile.MuzzleVelocity,
                    m = projectile.MassGrams / 1000,
                    C_d = projectile.BallisticCoefficientG1,
                    r = projectile.RadiusMillimeters / 1000,
                    rho = 1.204f
                }, out var nextPosition, out var nextVelocity, out var angleDelta);
            prevPosition = nextPosition;
            prevVelocity = nextVelocity;
            this.Points.Add(new BallisticArcPoint { Place = nextPosition, Time = startTime + stepResolution * i });
        }
    }

    public BallisticArc(Transform startingTransform, ProjectileInfo projectile, float startTime, float distance, float stepResolution = 0.001f)
    {
        this.Points = new List<BallisticArcPoint>();
        this.ProjectileInfo = projectile;
        var prevPosition = startingTransform.position;
        var prevVelocity = startingTransform.forward * projectile.MuzzleVelocity;
        var iteration = 0;
        while (Vector3.Distance(startingTransform.position, prevPosition) <= distance && prevVelocity != Vector3.zero)
        {
            //TODO change bullet data to projectile struct
            IntegrationMethods.Heuns(stepResolution, prevPosition, prevVelocity, Vector3.zero,
                new BulletData
                {
                    muzzleVelocity = projectile.MuzzleVelocity,
                    m = projectile.MassGrams / 1000,
                    C_d = projectile.BallisticCoefficientG1,
                    r = projectile.RadiusMillimeters / 1000,
                    rho = 1.204f
                }, out var nextPosition, out var nextVelocity, out var angleDelta);
            prevPosition = nextPosition;
            prevVelocity = nextVelocity;
            this.Points.Add(new BallisticArcPoint { Place = nextPosition, Time = startTime + stepResolution * iteration, AngleDelta =angleDelta });
            iteration++;
        }
    }
}

public static class BallisticsMethods
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dragCoefficient">Defaults to drag coefficient of a bullet</param>
    /// <param name="airDensity">Defaults to the density of 20 degrees celsius and 101.325 kilopascals (from wikiepedia) </param>
    /// <param name="crossSectionalArea">Defaults to cross sectional area of .308 (7.8mm)</param>
    /// <param name="massDensity">Defaults to the mass density of air at sea level</param>
    /// <returns></returns>
    public static float DragForce(float dragCoefficient = 0.295f, float airDensity = 1.204f, float crossSectionalArea = 47.78f, float massDensity = 1.2f)
    {
        return -0.5f * (massDensity * dragCoefficient * airDensity * crossSectionalArea);
    }
}



//A collection of integration methods for ballistics physics from https://github.com/Habrador/Unity-Ballistics-Tutorial credit Erik Nordeus
public static class IntegrationMethods
{
    private static Vector3 gravityVec = new Vector3(0f, -9.81f, 0f);



    //Integration method 1
    public static void BackwardEuler(float timeStep, Vector3 currentPos, Vector3 currentVel, out Vector3 newPos, out Vector3 newVel)
    {
        //Add all factors that affects the acceleration
        //Gravity
        Vector3 accFactor = gravityVec;


        //Calculate the new velocity and position
        //y_k+1 = y_k + timeStep * f(t_k+1, y_k+1)

        //This assumes the acceleration is the same next time step
        newVel = currentVel + timeStep * accFactor;

        newPos = currentPos + timeStep * newVel;
    }



    //Integration method 2
    public static void ForwardEuler(float timeStep, Vector3 currentPos, Vector3 currentVel, out Vector3 newPos, out Vector3 newVel)
    {
        //Add all factors that affects the acceleration
        //Gravity
        Vector3 accFactor = gravityVec;


        //Calculate the new velocity and position
        //y_k+1 = y_k + timeStep * f(t_k, y_k)

        newVel = currentVel + timeStep * accFactor;

        newPos = currentPos + timeStep * currentVel;
    }



    //Integration method 3
    //upVec is a vector perpendicular (in the upwards direction) to the direction the bullet is travelling in
    //is only needed if we calculate the lift force
    public static void Heuns(float timeStep, Vector3 currentPos, Vector3 currentVel, Vector3 upVec, BulletData bulletData, out Vector3 newPos, out Vector3 newVel, out float angleDelta)
    {
        //Add all factors that affects the acceleration
        //Gravity
        Vector3 accFactorEuler = gravityVec;
        //Drag
        accFactorEuler += BulletPhysics.CalculateBulletDragAcc(currentVel, bulletData);
        //Lift 
        accFactorEuler += BulletPhysics.CalculateBulletLiftAcc(currentVel, bulletData, upVec);


        //Calculate the new velocity and position
        //y_k+1 = y_k + timeStep * 0.5 * (f(t_k, y_k) + f(t_k+1, y_k+1))
        //Where f(t_k+1, y_k+1) is calculated with Forward Euler: y_k+1 = y_k + timeStep * f(t_k, y_k)

        //Step 1. Find new pos and new vel with Forward Euler
        Vector3 newVelEuler = currentVel + timeStep * accFactorEuler;

        //New position with Forward Euler (is not needed here)
        //Vector3 newPosEuler = currentPos + timeStep * currentVel;


        //Step 2. Heuns method's final step
        //If we take drag into account, then acceleration is not constant - it also depends on the velocity
        //So we have to calculate another acceleration factor
        //Gravity
        Vector3 accFactorHeuns = gravityVec;
        //Drag
        //This assumes that windspeed is constant between the steps, which it should be because wind doesnt change that often
        accFactorHeuns += BulletPhysics.CalculateBulletDragAcc(newVelEuler, bulletData);
        //Lift 
        accFactorHeuns += BulletPhysics.CalculateBulletLiftAcc(newVelEuler, bulletData, upVec);

        newVel = currentVel + timeStep * 0.5f * (accFactorEuler + accFactorHeuns);

        newPos = currentPos + timeStep * 0.5f * (currentVel + newVelEuler);
        angleDelta = Vector3.Angle(currentPos, newPos);
    }


    //Integration method 3
    //upVec is a vector perpendicular (in the upwards direction) to the direction the bullet is travelling in
    //is only needed if we calculate the lift force
    public static void Heuns(float timeStep, Vector3 currentPos, Vector3 currentVel, Vector3 upVec, BulletData bulletData, out BallisticArcPoint nextPoint)
    {
        //Add all factors that affects the acceleration
        //Gravity
        Vector3 accFactorEuler = gravityVec;
        //Drag
        accFactorEuler += BulletPhysics.CalculateBulletDragAcc(currentVel, bulletData);
        //Lift 
        accFactorEuler += BulletPhysics.CalculateBulletLiftAcc(currentVel, bulletData, upVec);


        //Calculate the new velocity and position
        //y_k+1 = y_k + timeStep * 0.5 * (f(t_k, y_k) + f(t_k+1, y_k+1))
        //Where f(t_k+1, y_k+1) is calculated with Forward Euler: y_k+1 = y_k + timeStep * f(t_k, y_k)

        //Step 1. Find new pos and new vel with Forward Euler
        Vector3 newVelEuler = currentVel + timeStep * accFactorEuler;

        //New position with Forward Euler (is not needed here)
        //Vector3 newPosEuler = currentPos + timeStep * currentVel;


        //Step 2. Heuns method's final step
        //If we take drag into account, then acceleration is not constant - it also depends on the velocity
        //So we have to calculate another acceleration factor
        //Gravity
        Vector3 accFactorHeuns = gravityVec;
        //Drag
        //This assumes that windspeed is constant between the steps, which it should be because wind doesnt change that often
        accFactorHeuns += BulletPhysics.CalculateBulletDragAcc(newVelEuler, bulletData);
        //Lift 
        accFactorHeuns += BulletPhysics.CalculateBulletLiftAcc(newVelEuler, bulletData, upVec);
        var nextVel = currentVel + timeStep * 0.5f * (accFactorEuler + accFactorHeuns);

        var nextPos = currentPos + timeStep * 0.5f * (currentVel + newVelEuler);
        nextPoint = new BallisticArcPoint
        {
            Place = nextPos,
            Velocity = nextVel,
            AngleDelta = Vector3.Angle(currentPos, nextPos),
            Time = timeStep
    };

    }


    //Integration method 3.1
    //No external bullet forces except gravity
    //Makes it easier to see if the external forces are working if we display this trajectory
    public static void HeunsNoExternalForces(float timeStep, Vector3 currentPos, Vector3 currentVel, out Vector3 newPos, out Vector3 newVel)
    {
        //Add all factors that affects the acceleration
        //Gravity
        Vector3 accFactor = gravityVec;


        //Calculate the new velocity and position
        //y_k+1 = y_k + timeStep * 0.5 * (f(t_k, y_k) + f(t_k+1, y_k+1))
        //Where f(t_k+1, y_k+1) is calculated with Forward Euler: y_k+1 = y_k + timeStep * f(t_k, y_k)

        //Step 1. Find new pos and new vel with Forward Euler
        Vector3 newVelEuler = currentVel + timeStep * accFactor;

        //New position with Forward Euler (is not needed)
        //Vector3 newPosEuler = currentPos + timeStep * currentVel;

        //Step 2. Heuns method's final step if acceleration is constant
        newVel = currentVel + timeStep * 0.5f * (accFactor + accFactor);

        newPos = currentPos + timeStep * 0.5f * (currentVel + newVelEuler);
    }
}

//A collection of methods for bullet physics from https://github.com/Habrador/Unity-Ballistics-Tutorial credit Erik Nordeus
public static class BulletPhysics
{
    //Calculate the bullet's drag acceleration
    public static Vector3 CalculateBulletDragAcc(Vector3 bulletVel, BulletData bulletData)
    {
        //If you have a wind speed in your game, you can take that into account here:
        //https://www.youtube.com/watch?v=lGg7wNf1w-k
        Vector3 bulletVelRelativeToWindVel = bulletVel - bulletData.windSpeedVector;

        //Step 1. Calculate the bullet's drag force [N]
        //https://en.wikipedia.org/wiki/Drag_equation
        //F_drag = 0.5 * rho * v^2 * C_d * A 

        //The velocity of the bullet [m/s]
        float v = bulletVelRelativeToWindVel.magnitude;
        //The bullet's cross section area [m^2]
        float A = Mathf.PI * bulletData.r * bulletData.r;

        float dragForce = 0.5f * bulletData.rho * v * v * bulletData.C_d * A;


        //Step 2. We need to add an acceleration, not a force, in the integration method [m/s^2]
        //Drag acceleration F = m * a -> a = F / m
        float dragAcc = dragForce / bulletData.m;

        //SHould be in a direction opposite of the bullet's velocity vector
        Vector3 dragVec = dragAcc * bulletVelRelativeToWindVel.normalized * -1f;


        return dragVec;
    }



    //Calculate the bullet's lift acceleration
    public static Vector3 CalculateBulletLiftAcc(Vector3 bulletVel, BulletData bulletData, Vector3 bulletUpDir)
    {
        //If you have a wind speed in your game, you can take that into account here:
        //https://www.youtube.com/watch?v=lGg7wNf1w-k
        Vector3 bulletVelRelativeToWindVel = bulletVel - bulletData.windSpeedVector;

        //Step 1. Calculate the bullet's lift force [N]
        //https://en.wikipedia.org/wiki/Lift_(force)
        //F_lift = 0.5 * rho * v^2 * S * C_l 

        //The velocity of the bullet [m/s]
        float v = bulletVelRelativeToWindVel.magnitude;
        //Planform (projected) wing area, which is assumed to be the same as the cross section area [m^2]
        float S = Mathf.PI * bulletData.r * bulletData.r;

        float liftForce = 0.5f * bulletData.rho * v * v * S * bulletData.C_l;

        //Step 2. We need to add an acceleration, not a force, in the integration method [m/s^2]
        //Drag acceleration F = m * a -> a = F / m
        float liftAcc = liftForce / bulletData.m;

        //The lift force acts in the up-direction = perpendicular to the velocity direction it travels in
        Vector3 liftVec = liftAcc * bulletUpDir;


        return liftVec;
    }
}

/// <summary>
/// from https://github.com/Habrador/Unity-Ballistics-Tutorial credit Erik Nordeus
/// </summary>
public class BulletData : MonoBehaviour
{
    //Data belonging to this bullet type
    //The initial speed [m/s]
    /// <summary>
    /// initial velocity in meters per second
    /// </summary>
    public float muzzleVelocity = 10f;
    /// <summary>
    /// Mass in kilograms
    /// </summary>
    public float m = 0.2f;
    /// <summary>
    /// Radius meters
    /// </summary>
    public float r = 0.05f;

    /// <summary>
    ///Coefficients, which is a value you can't calculate - you have to simulate it in a wind tunnel
    /// and they also depends on the speed, so we pick some average value
    ///Drag coefficient (Tesla Model S has the drag coefficient 0.24)  0
    /// </summary>
    public float C_d = 0.5f;

    /// <summary>
    /// Lift coefficient
    /// </summary>
    public float C_l = 0.5f;


    //External data (shouldn't maybe be here but is easier in this tutorial)
    //Wind speed [m/s]
    public Vector3 windSpeedVector = new Vector3(0f, 0f, 0f);
    //The density of the medium the bullet is travelling in, which in this case is air at 15 degrees [kg/m^3]
    public float rho = 1.225f;
}

