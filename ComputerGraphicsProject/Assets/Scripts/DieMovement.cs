using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieMovement : Rollable
{
    private int stopCheck;

    private double[,] d6Angles =
        {
        {270.0, 0.0},
        {0.0, 90.0},
        {0.0, 180.0},
        {0.0, 0.0},
        {0.0, 270.0},
        {90.0, 0.0}
        };

    private double[,] d20Angles =
        {
        {0.0, 37.4}, 
        {35.3, 13.3},
        {20.9, 328.8},
        {339.1, 328.3},
        {324.0, 13.7},

        {324.7, 103.3},
        {35.3, 103.3},
        {69.1, 238.3},
        {0.0, 258.6},
        {290.9, 238.3},

        {0.0, 79.2},
        {69.1, 58.3},
        {35.3, 283.3},
        {324.7, 283.3},
        {290.9, 58.3},

        {339.1, 148.3},
        {20.9, 148.3},
        {35.2, 193.3},
        {358.3, 218.0},
        {324.7, 193.3}
        };

    public override int GetValue()
    {
        int returnInt = 0;
        //Debug.Log("\t\t\t Die type: " + rigidBody.name);
        if (IsDoneMoving())
        {
            Vector3 angle = rigidBody.rotation.eulerAngles;
            //D6 value handling
            if (rigidBody.name.CompareTo("d6(Clone)") == 0)
            {
                Debug.Log("\t\t\t Die angles: " + angle);

                double bestDiff = 360.0;
                double newDiff;
                for (int i = 0; i < 6; i++)
                {
                    newDiff = AngleDiff(angle.x, angle.z, d6Angles[i, 0], d6Angles[i, 1]);
                    if (newDiff < bestDiff)
                    {
                        bestDiff = newDiff;
                        returnInt = i + 1;
                    }
                }
            }
            //D20 Value handling
            else if (rigidBody.name.CompareTo("d20(Clone)") == 0)
            {
                //Debug.Log("\t\t\t Die angles: " + angle);

                double bestDiff = 360.0;
                double newDiff;
                for (int i = 0; i<20; i++)
                {
                    newDiff = AngleDiff(angle.x, angle.z, d20Angles[i, 0], d20Angles[i, 1]);
                    if (newDiff < bestDiff)
                    {
                        bestDiff = newDiff;
                        returnInt = i + 1;
                    }
                }
            }
            //Bottle Value Handling
            else if (rigidBody.name.CompareTo("bottle(Clone)") == 0)
            {
                Debug.Log("\t\t\t Die angles: " + angle);

                returnInt = 1;
                if (AngleDiff(angle.x, angle.z, 0.0, 0.0) < 10.0)
                    returnInt = 10;
                if (AngleDiff(angle.x, angle.z, 0.0, 180.0) < 10.0)
                    returnInt = 20;
            }
            SetHasReturned(true);
        }
        
        return returnInt;
    }

    public override bool IsDoneMoving()
    {
        return isFinishedMoving;
    }

    public override bool HasReturnedValue()
    {
        return hasReturnedValue;
    }

    public override void SetHasReturned(bool returned)
    {
        hasReturnedValue = true;
    }

    public override void UpdateMovement()
    {
        double velocityMagnitude = rigidBody.angularVelocity.magnitude;
        if (velocityMagnitude > 0.5)
        {
            isMoving = true;
            hasStartedMoving = true;
            stopCheck = 20;
        }
        else if (velocityMagnitude < 0.01 && hasStartedMoving && !isFinishedMoving)
        {
            if (stopCheck-- < 0)
            {
                isMoving = false;
                isFinishedMoving = true;
            }
        }
    }

    //Returns the larger of the two angle differences between the given x angles and z angles
    private double AngleDiff(double inX, double inZ, double tarX, double tarZ)
    {
        double xdiff = 180 - System.Math.Abs(System.Math.Abs(inX - tarX) - 180);
        double zdiff = 180 - System.Math.Abs(System.Math.Abs(inZ - tarZ) - 180);

        return System.Math.Max(xdiff, zdiff);
    }
}
