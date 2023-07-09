using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject initialScreen;
    [SerializeField] GameObject levelSelection;

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
        levelSelection.SetActive(true);
    }

    public void ClickExitLevelSelection()
    {
        initialScreen.SetActive(true);
        levelSelection.SetActive(false);
    }
}
