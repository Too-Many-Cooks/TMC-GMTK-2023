using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeLevel : MonoBehaviour
{
    const int mainMenuIndex = 0;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(mainMenuIndex);
    }
}
