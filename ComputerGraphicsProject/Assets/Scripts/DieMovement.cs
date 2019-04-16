using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieMovement : Rollable
{
    public override int GetValue()
    {
        int returnInt = 0;
        if (IsDoneMoving())
        {
            Vector3 angle = rigidBody.rotation.eulerAngles;
            if (265.0 < angle.x && angle.x < 275.0)
            {
                returnInt = 1;
            }
            else if (80.0 < angle.x && angle.x < 100.0)
            {
                returnInt = 6;
            }
            else if (355 < angle.x || angle.x < 5.0)
            {
                if (265.0 < angle.z && angle.z < 275.0)
                {
                    returnInt = 5;
                }
                else if (85.0 < angle.z && angle.z < 95.0)
                {
                    returnInt = 2;
                }
                else if (355 < angle.z || angle.z < 5.0)
                {
                    returnInt = 4;
                }
                else if (175.0 < angle.z && angle.z < 185.0)
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
        double velocityMagnitude = rigidBody.angularVelocity.magnitude;
        if (velocityMagnitude > 0.5)
        {
            isMoving = true;
            hasStartedMoving = true;
        }
        else if (velocityMagnitude < 0.01 && hasStartedMoving)
        {
            isMoving = false;
            isFinishedMoving = true;
        }
    }
}
