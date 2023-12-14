using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private string gameScene;
    [SerializeField] private GenerateClouds cloudGenerator;

    public void ButtonQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void ButtonStart()
    {
        SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }
}
