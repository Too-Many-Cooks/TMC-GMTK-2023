using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverChildTest : LeverChild
{
    public override void ActivateLever()
    {
        print("Activated");
    }

    public override void DeactivateLever()
    {
        print("Deactivated");
    }
}
