using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMovementMeassurement : MonoBehaviour
{
    Vector3? _pastLevelPosition;
    Vector3 PastLevelPosition
    {
        get
        {
            if(_pastLevelPosition == null)
                _pastLevelPosition = transform.localPosition;

            return _pastLevelPosition.Value;
        }

        set { _pastLevelPosition = value; }
    }

    Vector3 currentLocalPosition;
    
    public Vector3 LevelMovementDelta
    {
        get
        {
            return currentLocalPosition - PastLevelPosition;
        }
    }

    private void Update()
    {
        PastLevelPosition = currentLocalPosition;
        currentLocalPosition = transform.localPosition;
    }
}
