using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private EndMenuHandler endMenu;
    [SerializeField] private GameObject hud;
    [SerializeField] private ChunkGenerator chunkGen;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource endSource;

    private CharacterController player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
    }

    public void Menu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
        Resources.UnloadUnusedAssets();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.IsDead && !endMenu.gameObject.activeSelf)
        {
            endMenu.SetScore(player.NbJewels, player.NbEnnemies, chunkGen.GetPlayerChunk());
            endMenu.gameObject.SetActive(true);
            hud.SetActive(false);
            musicSource.Stop();
            endSource.Play();
        }
    }
}
