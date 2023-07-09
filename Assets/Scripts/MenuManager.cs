using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject initialScreen;
    [SerializeField] GameObject levelSelectionScreen;
    [SerializeField] GameObject instructionsScreen;
    [SerializeField] GameObject creditsScreen;

    [Header("Scenes:")]
    [SerializeField] private int scene1Index;
    [SerializeField] private int scene2Index;
    [SerializeField] private int scene3Index;

    public void PlayLevel(int sceneNumber)
    {
        switch (sceneNumber)
        {
            case 1:
                SceneManager.LoadScene(scene1Index);
                break;

            case 2:
                SceneManager.LoadScene(scene2Index);
                break;

            case 3:
                SceneManager.LoadScene(scene3Index);
                break;

            default:
                Debug.LogError("Scene number not recognized.");
                break;
        }
    }

    public void ClickEnterLevelSelection()
    {
        initialScreen.SetActive(false);
        levelSelectionScreen.SetActive(true);
    }

    public void ClickExitLevelSelection()
    {
        initialScreen.SetActive(true);
        levelSelectionScreen.SetActive(false);
    }


    public void ClickEnterCredits()
    {
        initialScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void ClickExitCredits()
    {
        initialScreen.SetActive(true);
        creditsScreen.SetActive(false);
    }


    public void ClickEnterInstructions()
    {
        initialScreen.SetActive(false);
        instructionsScreen.SetActive(true);
    }

    public void ClickExitInstructions()
    {
        initialScreen.SetActive(true);
        instructionsScreen.SetActive(false);
    }
}

