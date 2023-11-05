using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnnemy : MonoBehaviour
{
    [SerializeField] private GameObject ennemy;

    [HideInInspector] public Vector3 EnnemySpawnPoint;
    public Transform parent;

    
    void CreateEnnemy()
    {
        Transform go = Instantiate(ennemy, EnnemySpawnPoint, Quaternion.identity).transform;
        go.SetParent(parent);
        go.position = EnnemySpawnPoint;
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
