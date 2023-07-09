using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Connected Scripts:")]
    [SerializeField] LeverChild[] linkedScripts;

    [Header("Configuration")]
    [SerializeField] [Range(-90, 90)] float intialXRotation;
    [SerializeField] [Range(0.001f, 5)] float leverAnimDuration = 0.5f;

    [Header("References:")]
    [SerializeField] Transform myLeverStick;

    #region Self-Initiated Properties

    Timer _leverAnimTimer;
    Timer LeverAnimTimer
    {
        get
        {
            if (_leverAnimTimer == null)
                _leverAnimTimer = new Timer(leverAnimDuration);

            return _leverAnimTimer;
        }
    }

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ToggleLever();

        if ((isLeverActivated && LeverAnimTimer.IsComplete) || (!isLeverActivated && LeverAnimTimer.Time == 0))
            return;

        if (isLeverActivated)
            LeverAnimTimer.Update();
        else
            LeverAnimTimer.NegativeUpdate();

        float easedPercentageComplete = LeverAnimTimer.PercentageComplete;

        if(easedPercentageComplete < 0.5f)
        {
            easedPercentageComplete = Mathf.Lerp(0, 0.5f,
                EasingFunctions.ApplyEase(easedPercentageComplete * 2, EasingFunctions.Functions.InCirc));
        }
        else
        {
            easedPercentageComplete = Mathf.Lerp(0.5f, 1, EasingFunctions.
                ApplyEase((easedPercentageComplete - 0.5f) * 2, EasingFunctions.Functions.OutCirc));
        }

        myLeverStick.localRotation =
            Quaternion.Euler(Mathf.Lerp(intialXRotation, -intialXRotation, easedPercentageComplete), 0, 0);

        print(Mathf.Lerp(intialXRotation, -intialXRotation, easedPercentageComplete));
    }

    bool isLeverActivated = false;

    void SetLeverState(bool isActivated)
    {
        if (linkedScripts.Length == 0)
            return;

        if (isActivated)
        {
            foreach (LeverChild leverInterfaceScript in linkedScripts)
                leverInterfaceScript.ActivateLever();
        }
        else
        {
            foreach (LeverChild leverInterfaceScript in linkedScripts)
                leverInterfaceScript.DeactivateLever();
        }
    }

    public void ToggleLever()
    {
        isLeverActivated = !isLeverActivated;

        SetLeverState(isLeverActivated);
    }
}
