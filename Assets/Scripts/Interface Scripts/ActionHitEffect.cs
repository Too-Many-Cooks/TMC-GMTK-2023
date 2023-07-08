using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActionHitEffect : MonoBehaviour
{
    #region Self-Initialized Properties

    RectTransform _myRectTransform;
    RectTransform MyRectTransform
    {
        get
        {
            if (_myRectTransform == null)
                _myRectTransform = GetComponent<RectTransform>();

            return _myRectTransform;
        }
    }

    Image _myImage;
    Image MyImage
    {
        get
        {
            if (_myImage == null)
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
            {
                _myRegularTimer = new Timer(growingScaleTimeLength);
                _myRegularTimer.Time = _myRegularTimer.TargetTime;
            }
                

            return _myRegularTimer;
        }
    }

    Timer _myDissapearingTimer;
    Timer MyDissapearingTimer
    {
        get
        {
            if (_myDissapearingTimer == null)
            {
                _myDissapearingTimer = new Timer(decreasingScaleTimeLenght);
                _myDissapearingTimer.Time = _myDissapearingTimer.TargetTime;

            }

            return _myDissapearingTimer;
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

    Vector3? _originalColor;
    Vector3 OriginalColor
    {
        get
        {
            if (_originalColor == null)
                _originalColor = new Vector3(MyImage.color.r, MyImage.color.g, MyImage.color.b);

            return _originalColor.Value;
        }
    }

    #endregion

    const float verticalScaleMultiplier = 2f;
    public float growingScaleTimeLength = 0.05f;
    const float decreasingScaleTimeLenght = 1f;
    const float targetSpriteTransparency = 0.7f;

    private void Update()
    {
        // If our hit is about to happen.
        if (!MyRegularTimer.IsComplete)
        {
            // Scale
            MyRegularTimer.Update();

            float easedPercentageComplete =
                EasingFunctions.ApplyEase(MyRegularTimer.PercentageComplete, EasingFunctions.Functions.InQuad);

            MyRectTransform.localScale = Vector3.Lerp(OriginalLocalScale,
                AlteredVerticalScale(OriginalLocalScale, verticalScaleMultiplier), easedPercentageComplete);


            // Transparency
            float alpha = Mathf.Lerp(0, targetSpriteTransparency, MyRegularTimer.PercentageComplete);
            MyImage.color = ObtainColorOfTransparency(OriginalColor, alpha);


            return;
        }

        // After our hit happens and while we decrease our size.
        if (!MyDissapearingTimer.IsComplete)
        {
            // Scale
            MyDissapearingTimer.Update();

            float easedPercentageComplete =
                EasingFunctions.ApplyEase(MyDissapearingTimer.PercentageComplete, EasingFunctions.Functions.OutQuad);

            MyRectTransform.localScale = 
                Vector3.Lerp(AlteredVerticalScale(OriginalLocalScale, verticalScaleMultiplier),
                OriginalLocalScale, easedPercentageComplete);


            // Transparency
            float correctedTransparencyPercentage = EasingFunctions.ApplyEase
                (MyDissapearingTimer.PercentageComplete, EasingFunctions.Functions.OutQuad);
            float dissapearingAlpha = Mathf.Lerp(targetSpriteTransparency, 0, correctedTransparencyPercentage);
            MyImage.color = ObtainColorOfTransparency(OriginalColor, dissapearingAlpha);

            return;
        }
    }


    public void ActivateHitEffect()
    {
        MyRegularTimer.Reset();
        MyDissapearingTimer.Reset();
    }

    /// <summary>
    /// Returns the provided scale with a multiplied vertical axis.
    /// </summary>
    /// <param name="originalScale"></param>
    /// <param name="verticalScaleMultiplier"></param>
    /// <returns></returns>
    Vector3 AlteredVerticalScale(Vector3 originalScale, float verticalScaleMultiplier)
    {
        return new Vector3(originalScale.x, originalScale.y * verticalScaleMultiplier, originalScale.z);
    }

    Color ObtainColorOfTransparency(Vector3 rgb, float alpha)
    {
        return new Color(rgb.x, rgb.y, rgb.z, alpha);
    }
}
