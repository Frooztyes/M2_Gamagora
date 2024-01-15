using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] AudioClip PuckCollision;
    [SerializeField] AudioClip Goal;
    [SerializeField] AudioClip WonGame;

    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();   
    }

    public void PlayPuckCollision()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(PuckCollision);
    }

    public void PlayGoal()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(Goal);
    }

    public void PlayWonGame()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(WonGame);
    }
}
