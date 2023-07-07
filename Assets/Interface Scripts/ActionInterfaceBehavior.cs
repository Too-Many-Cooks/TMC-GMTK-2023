using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActionInterfaceBehavior : MonoBehaviour
{
    #region Self-Initialized properties

    RectTransform _myRectTransform;
    RectTransform MyRectTransform
    {
        get
        {
            if(_myRectTransform == null)
                _myRectTransform = GetComponent<RectTransform>();

            return _myRectTransform;
        }
    }

    Image _myImage;
    Image MyImage
    {
        get
        {
            if(_myImage == null)
                _myImage = GetComponent<Image>();

            return _myImage;
        }
    }

    Timer _myRegularTimer;
    Timer MyRegularTimer
    {
        get
        {
            if (_myRegularTimer == null)
                _myRegularTimer = new Timer(ObtainDurationOfTimer());

            return _myRegularTimer;
        }
    }

    Timer _myDissapearingTimer;
    Timer MyDissapearingTimer
    {
        get
        {
            if(_myDissapearingTimer == null)
                _myDissapearingTimer = new Timer(MyRegularTimer.TargetTime * dissapearingTimeDistanceMultiplier); 
                                    // We set the duration of our second timer to a multiple of the original.
            
            return _myDissapearingTimer;
        }
    }

    Vector3? _originalLocalPosition;
    Vector3 OriginalLocalPosition
    {
        get
        {
            if (_originalLocalPosition == null)
                _originalLocalPosition = MyRectTransform.localPosition;

            return _originalLocalPosition.Value;
        }
    }

    Vector3? _originalLocalScale;
    Vector3 OriginalLocalScale
    {
        get
        {
            if (_originalLocalScale == null)
                _originalLocalScale = MyRectTransform.localScale;

            return _originalLocalScale.Value;
        }
    }

    #endregion

    private Vector3 InitialPosition { get { return new Vector3(400, OriginalLocalPosition.y, 0); } }
    private Vector3 CentralPosition { get { return new Vector3(0, OriginalLocalPosition.y, 0); } }
    private Vector3 FinalPosition { get { return 
        new Vector3(- InitialPosition.x * dissapearingTimeDistanceMultiplier, OriginalLocalPosition.y, 0); } }

    // Determines the duration of the dissapear animation with regard to the duration of the OG appear animation.
    const float dissapearingTimeDistanceMultiplier = 0.5f;

    // From this % of completition (Regular timer) onwards, the model begins to grow in size.
    const float percentageNeededToBeginScale = 0.9f;

    // From this % of completition (Dissapear timer) onwards, the model dissapears from the screen centre.
    const float percentageNeededToBeginDissapearing = 0.15f;

    // The multiplier of the scale at the high point of the model.
    const float bigScaleMultiplier = 1.1f;
    const float smallScaleMultiplier = 0.8f;


    private void Awake()
    {
        MyRectTransform.position = InitialPosition;
        MyImage.color = ObtainColorForTransparency(0);
    }

    private void Update()
    {
        // We first update our Timers.
        if (!MyRegularTimer.IsComplete)
        {
            MyRegularTimer.Update();
            AppearAnimation(MyRegularTimer.PercentageComplete);

            if (MyRegularTimer.IsComplete)
                MyActionHitEffect.ActivateHitEffect();


            return;
        }
            
        // Else
        MyDissapearingTimer.Update();
        DissapearAnimation(MyDissapearingTimer.PercentageComplete);

        if (MyDissapearingTimer.IsComplete)
            Destroy(this.gameObject);
    }


    void AppearAnimation(float percentageComplete)
    {
        // Visually changes.
        float alpha = EasingFunctions.ApplyEase(percentageComplete, EasingFunctions.Functions.InCirc);
        MyImage.color = ObtainColorForTransparency(alpha);

        // Movement.
        float easedMovement = EasingFunctions.ApplyEase(percentageComplete, EasingFunctions.Functions.InCubic);
        MyRectTransform.localPosition = Vector3.Lerp(InitialPosition, CentralPosition, easedMovement);

        // Scale.
        float scaleEasedPercentage = 0;

        if (percentageComplete > percentageNeededToBeginScale)
        {
            scaleEasedPercentage = (percentageComplete - percentageNeededToBeginScale) 
                                    / (1 - percentageNeededToBeginScale);

            scaleEasedPercentage =
                EasingFunctions.ApplyEase(scaleEasedPercentage, EasingFunctions.Functions.InBack);
        }

        MyRectTransform.localScale =
            Vector3.Lerp(OriginalLocalScale, OriginalLocalScale * bigScaleMultiplier, scaleEasedPercentage);
    }

    void DissapearAnimation(float percentageComplete)
    {
        if (percentageComplete < percentageNeededToBeginDissapearing)
            return;

        float correctedPercentageComplete = 
            (percentageComplete - percentageNeededToBeginDissapearing) / (1 - percentageNeededToBeginDissapearing);
        
        
        // Visually changes.                  
        float alpha =            // ("1 - %" because we are going from visible to invisible)
            EasingFunctions.ApplyEase(1 - correctedPercentageComplete, EasingFunctions.Functions.OutCubic);
        MyImage.color = ObtainColorForTransparency(alpha);

        // Movement.
        float easedMovement = 
            EasingFunctions.ApplyEase(correctedPercentageComplete, EasingFunctions.Functions.OutCubic);
        MyRectTransform.localPosition = Vector3.Lerp(CentralPosition, FinalPosition, easedMovement);

        // Scale
        float scaleEasedPercentage = 0;

        if (correctedPercentageComplete < (1 - (percentageNeededToBeginScale * dissapearingTimeDistanceMultiplier)))
        {
            scaleEasedPercentage = 1 - (correctedPercentageComplete / 
                (1 - percentageNeededToBeginScale * dissapearingTimeDistanceMultiplier));

            scaleEasedPercentage =
                EasingFunctions.ApplyEase(scaleEasedPercentage, EasingFunctions.Functions.OutQuad);
        }

        MyRectTransform.localScale = Vector3.Lerp(OriginalLocalScale * smallScaleMultiplier, 
            OriginalLocalScale * bigScaleMultiplier, scaleEasedPercentage);
    }


    Color ObtainColorForTransparency(float alpha)
    {
        if(alpha < 0 || alpha > 1)
        {
            Debug.LogError("Alpha value must be in the Range[0,1].");
            return Color.white;
        }

        return new Color(1, 1, 1, alpha);
    }

    /// <summary>
    /// This function should return the duration of the animation between the 
    /// creation of the action Sprite and its arrival at the centre of the image.
    /// </summary>
    /// <returns></returns>
    float ObtainDurationOfTimer()
    {
        return 2.5f;
    }


    #region DEBUG

    ActionHitEffect _myActionHitEffect;
    ActionHitEffect MyActionHitEffect
    {
        get
        {
            if(_myActionHitEffect == null)
                _myActionHitEffect = transform.parent.GetComponentInChildren<ActionHitEffect>();

            return _myActionHitEffect;
        }
    }

    #endregion
}
