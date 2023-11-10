using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject MenuHUD;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GenerateClouds cloudGenerator;


    private void Start()
    {
        MenuHUD.SetActive(true);
    }

    public void ButtonQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void ButtonStart()
    {
        Camera.main.GetComponent<CameraFollow>().enabled = true;
        cloudGenerator.Stop();
        MenuHUD.SetActive(false);
        HUD.SetActive(true);
    }
}
