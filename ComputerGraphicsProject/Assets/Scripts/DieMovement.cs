using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieMovement : Rollable
{
    public override int GetValue()
    {
        int returnInt = 0;
        Vector3 angle = rigidBody.rotation.eulerAngles;
        if (IsDoneMoving() )
        {
            
            if (260.0 < angle.x && angle.x < 280.0)
            {
                returnInt = 1;
            }
            else if (80.0 < angle.x && angle.x < 100.0)
            {
                returnInt = 6;
            }
            else if (350 < angle.x || angle.x < 10.0)
            {
                if (260.0 < angle.z && angle.z < 280.0)
                {
                    returnInt = 5;
                }
                else if (80.0 < angle.z && angle.z < 100.0)
                {
                    returnInt = 2;
                }
                else if (350 < angle.z || angle.z < 10.0)
                {
                    returnInt = 4;
                }
                else if (170.0 < angle.z && angle.z < 190.0)
                {
                    returnInt = 3;
                }
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
        Vector3 angle = rigidBody.rotation.eulerAngles;
        double velocityMagnitude = rigidBody.angularVelocity.magnitude;
        if (velocityMagnitude > 0.5)
        {
            isMoving = true;
            hasStartedMoving = true;
        }
        else if (velocityMagnitude < 0.01 && hasStartedMoving
            && ((260.0 < angle.x && angle.x < 280.0)
                || (80.0 < angle.x && angle.x < 100.0)
                || (350 < angle.x || angle.x < 10.0)))
        {
            isMoving = false;
            isFinishedMoving = true;
        }
    }
}
