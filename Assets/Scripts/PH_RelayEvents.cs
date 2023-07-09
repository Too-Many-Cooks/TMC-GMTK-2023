using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PH_RelayEvents : MonoBehaviour
{
    public UnityEvent shiftshock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnShiftUp()
    {
        shiftshock.Invoke();
    }

    public void OnShiftDown()
    {
        shiftshock.Invoke();
    }

    public void OnShiftLeft()
    {
        shiftshock.Invoke();
    }

    public void OnShiftRight()
    {
        shiftshock.Invoke();
    }
}
