using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMusic : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioSource outdoorMusic;
    [SerializeField] private AudioSource indoorMusic;

    private bool hasSpawned = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string layer = LayerMask.LayerToName(collision.gameObject.layer);
        if (layer == "Player" && !hasSpawned)
        {
            hasSpawned = true;

            outdoorMusic.Stop();
            indoorMusic.Play();

            Destroy(gameObject);
        }
    }
}
