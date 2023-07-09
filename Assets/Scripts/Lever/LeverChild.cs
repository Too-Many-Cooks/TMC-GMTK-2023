using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LeverChild : MonoBehaviour, ILeverLinked
{
    public abstract void ActivateLever();

    public abstract void DeactivateLever();
}
