using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnnemy : MonoBehaviour
{
    [SerializeField] private GameObject ennemy;

    [HideInInspector] public Vector3 EnnemySpawn;

    
    void CreateEnnemy()
    {
        Instantiate(ennemy, EnnemySpawn, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string layer = LayerMask.LayerToName(collision.gameObject.layer);
        if (layer == "Player")
        {
            CreateEnnemy();
            Destroy(gameObject);
        }
    }
}
