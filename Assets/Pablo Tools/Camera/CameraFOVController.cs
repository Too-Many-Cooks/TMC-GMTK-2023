using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFOVController : MonoBehaviour
{
    [Header("Transition times")]    // Used to calculate transition times based on the lenght of the transition.
    [SerializeField] [Range(0.01f, 5)] float minDuration = 0.1f;
    [SerializeField] [Range(0.01f, 5)] float maxDuration = 0.5f;


    [Header("FOV limits")]          // The upper and lower limits of our FOV.
                                    // Used to calculate the transition times.
    [SerializeField] [Range(0.01f, 100)] float minFOV = 5;
    [SerializeField] [Range(0.01f, 100)] float maxFOV = 20;


    void CheckForMinMaxInconsistences()
    {
        if (minDuration >= maxDuration)
            Debug.LogWarning("minDuration should always be smaller than maxDuration.");

        if (minFOV >= maxFOV)
            Debug.LogWarning("minFOV should always be smaller than maxFOV.");
    }


    const int maxNumberOfSimultaneousCommands = 10;


    // _ _ _ This script DEFAULTS to Unity Cinemachine if possible, otherwise it tries to find a Unity Camera. _ _ _

    bool? usingCinemachine = null;

    // This function checks sets our usingCinemachine bool to its correct value.
    void CheckWhichCameraTypeIsUsing()
    {
        if (TryGetComponent<CinemachineVirtualCamera>(out CinemachineVirtualCamera camera))
            usingCinemachine = true;
        else
            usingCinemachine = false;
    }


    // This property initializes our Cinemachine Virtual Camera Reference
    CinemachineVirtualCamera _myCinemachineCamera;
    CinemachineVirtualCamera MyCinemachineCamera
    {
        get
        {
            if (_myCinemachineCamera == null)
            {
                if (TryGetComponent<CinemachineVirtualCamera>(out CinemachineVirtualCamera camera))
                    _myCinemachineCamera = camera;

                else
                    Debug.LogError("Could not find a CinemachineVirtualCamera attached to " 
                        + transform.gameObject.name + ".");
            }

            return _myCinemachineCamera;
        }
    }


    // This property initializes our regular Unity Camera Reference
    Camera _myUnityCamera;
    Camera MyUnityCamera
    {
        get
        {
            if (_myUnityCamera == null)
            {
                if (TryGetComponent<Camera>(out Camera camera))
                    _myUnityCamera = camera;

                else
                    Debug.LogError("Could not find a Unity Camera attached to "
                        + transform.gameObject.name + ".");
            }

            return _myUnityCamera;
        }
    }


    bool isOrthographic { get { return Camera.main.orthographic; } }


    // Property that uses the isOrthographic bool to get and set the correct value for FieldOfView / OrthoSize.
    public float CameraFOVValue
    {
        get
        {
            if (usingCinemachine == null)
                CheckWhichCameraTypeIsUsing();
            
            // If our Main.Camera isOrthographic, we access a different value than if its not.
            // Same for wether we are using a Cinemachine Camera or a regular one.

            if (usingCinemachine.Value)
            {
                // If our Main.Camera isOrthographic, we access a different value than if its not.
                if (isOrthographic)
                    return MyCinemachineCamera.m_Lens.OrthographicSize;
                else
                    return MyCinemachineCamera.m_Lens.FieldOfView;
            }
            else
            {
                if (isOrthographic)
                    return MyUnityCamera.orthographicSize;
                else
                    return MyUnityCamera.fieldOfView;
            }
        }

        set
        {
            if (usingCinemachine == null)
                CheckWhichCameraTypeIsUsing();

            if (usingCinemachine.Value)
            {
                if (isOrthographic)
                    MyCinemachineCamera.m_Lens.OrthographicSize = value;
                else
                    MyCinemachineCamera.m_Lens.FieldOfView = value;
            }
            else
            {
                if (isOrthographic)
                    MyUnityCamera.orthographicSize = value;
                else
                    MyUnityCamera.fieldOfView = value;
            }
        }
    }

    // This property keeps track of the target FOV to have. Needs to be initialized in Awake().
    public float CurrentTargetFOV
    {
        get
        {
            // With no commands, our targetFOV is = to our current FOV.
            if (myCommands.Count == 0)
                return CameraFOVValue;

            // With commands, we take our last FOV command's targetFOV.
            else
                return myCommands[myCommands.Count - 1].targetFOV;
        }
    }


    class FOVCommand
    {
        // Storing values to perform the Lerp between different FOV values.
        public float initialFOV;
        public float targetFOV;
        public float durationOfTransition;
        public float transitionTimer = 0;

        public FOVCommand(float FOV_initial, float FOV_target, float duration)
        {
            initialFOV = FOV_initial;
            targetFOV = FOV_target;
            durationOfTransition = duration;
        }


        // This function updates the transitionTimer.
        // It also clamps it so that it does not exceed the value of durationOfTransition.
        public void UpdateTimer()
        {
            transitionTimer += Time.deltaTime;

            if (transitionTimer > durationOfTransition)
                transitionTimer = durationOfTransition;
        }


        // This function will return the current FOV according to this order.
        public float ObtainFOV()
        {
            float percentageOfCompletion =     // We calculate the percentage and apply an easing function to it.
                EasingFunctions.ApplyEase(transitionTimer / durationOfTransition, EasingFunctions.Functions.InOutSine);

            return Mathf.Lerp(initialFOV, targetFOV, percentageOfCompletion);
        }


        // This function is used to prematurely end a command.
        public void EndCommand()
        {
            transitionTimer = durationOfTransition;
        }
    }

    // We also need a list of commands to keep track of them.
    List<FOVCommand> myCommands = new List<FOVCommand>();

    private void Start()
    {
        CheckForMinMaxInconsistences();
    }


    private void LateUpdate()
    {
        // If our command list is empty, we are NOT transitioning.
        if (myCommands.Count == 0)
            return;

        CheckForFinishedFunctions();
        UpdateAllTimers();
        SetFOV(CalculateFOVValue());
    }


    // This function calculates the time it will take to change from one FOV to another.
    float CalculateTimeToTransition(float initialFOV, float targetFOV)
    {
        float maxFOVChange = maxFOV - minFOV;

        float currentChange = Mathf.Abs(initialFOV - targetFOV);

        float result = Mathf.Lerp(minDuration, maxDuration,
                       EasingFunctions.ApplyEase(currentChange / maxFOVChange, EasingFunctions.Functions.OutCubic));

        return result;
    }


    // This function is used to log a FOV command in our command List.
    public void ChangeFOV(float newTargetFOV)
    {
        // In case our INITIAL FOV == FINAL FOV.
        if (CurrentTargetFOV == newTargetFOV)
            return;


        // We then check for command overloads (we can't handle infinite commands).
        // If there is overload, we delete all previous commands, and set the initial FOV to our current FOV.
        if (myCommands.Count > maxNumberOfSimultaneousCommands)
            myCommands.Clear();


        // We store the lenght of the transition.
        float transitionLenght = CalculateTimeToTransition(CurrentTargetFOV, newTargetFOV);

        // We create the command.
        FOVCommand command = new FOVCommand(CurrentTargetFOV, newTargetFOV, transitionLenght);

        // And we add this command to our commands list.
        myCommands.Add(command);
    }


    // This function is used to change the FOV without a transition. It clears the commands history.
    public void ImmediatlyChangeFOV(float newTargetFOV)
    {
        // Checks just in case.
        if(newTargetFOV <= 0)
        {
            Debug.LogWarning("Cannot set Camera FOV to [" + newTargetFOV 
                + "]. Camera FOV value needs to be a natural number.");
            return;
        }

        
        // We clear the commands list.
        if (myCommands.Count > 0)
            myCommands.Clear();

        CameraFOVValue = newTargetFOV;
    }


    // This function checks all commands for finished ones. If it finds one, it clears all previous commands.
    void CheckForFinishedFunctions()
    {
        // We need to create an int to check if we need to eliminate any functions
        int commandsToEliminate = 0;

        for (int i = 0; i < myCommands.Count; i++)
        {
            // If the timer is filled.
            if (myCommands[i].transitionTimer == myCommands[i].durationOfTransition)
            {
                commandsToEliminate = i + 1;  // We store that we have to eliminate i +1 commands (because i starts at 0).
            }
        }


        // If we find NO commands to ELIMINATE, we STOP.
        if (commandsToEliminate == 0)
            return;

        // We remove the found commands.
        for (int i = commandsToEliminate; i > 0; i--)
        {
            myCommands.RemoveAt(0);
        }
    }


    // This function updates all of the timers in our commands.
    void UpdateAllTimers()
    {
        foreach (FOVCommand command in myCommands)
        {
            command.UpdateTimer();
        }
    }


    // This function calculates the value of the FOV, by lerping between all the different commands.
    float CalculateFOVValue()
    {
        // If we don't have commands, we don't change.
        if(myCommands.Count == 0)
            return CameraFOVValue;

        // If we only have 1 command, the change is easy.
        if (myCommands.Count == 1)
            return myCommands[0].ObtainFOV();


            // If we have more, we need to lerp between values.
        // We first create a variable to hold the last lerped value and give it the first command's value.
        float lastLerpedValue = myCommands[0].ObtainFOV();

        // We set up a loop that will lerp all values in order of command.
        for (int i = 0; i + 1 < myCommands.Count; i++)
        {
            // We first store both commands.
            FOVCommand formerCommand = myCommands[i];
            FOVCommand latterCommand = myCommands[i + 1];


            // At each loop, we need to define the common passedTime and finishingTime.
            // In other words, how much time has already passed and how much time it takes to transition.

            // Commands are ordered by arrival, so the earliest command will always be higher in the index.
            // Thus, we take our time from the second command.
            float timePassed = latterCommand.transitionTimer;


            // The finishing time will be the smaller duration of the transition between both commands.
            float finishingTime;

            if (formerCommand.durationOfTransition < latterCommand.durationOfTransition)
                finishingTime = formerCommand.durationOfTransition;
            else
                finishingTime = latterCommand.durationOfTransition;

            float lerpEasedTime = 
                EasingFunctions.ApplyEase(timePassed / finishingTime, EasingFunctions.Functions.InOutSine);

            lastLerpedValue = Mathf.Lerp(lastLerpedValue, latterCommand.ObtainFOV(), lerpEasedTime);
        }

        // The result is the lerped curve, which produces a smooth transition between orders.
        return lastLerpedValue;
    }


    // This function sets the FOV value to a concrete value.
    void SetFOV(float newFOV)
    {
        CameraFOVValue = newFOV;
    }
}
