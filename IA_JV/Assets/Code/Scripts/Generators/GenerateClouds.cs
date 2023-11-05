using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenerateClouds : MonoBehaviour
{

    [SerializeField] List<GameObject> clouds;
    [SerializeField] float maxDelay = 2f;
    [SerializeField] float offsetDelay = 0.2f;
    float cameraWidth;
    float cameraHeight;

    float delay;
    bool stopGeneration = false;

    // Start is called before the first frame update
    void Start()
    {
        Camera cam = Camera.main;
        cameraHeight = 2f * cam.orthographicSize;
        cameraWidth = cameraHeight * cam.aspect;

        Random.InitState(System.DateTime.Now.Millisecond);
        delay = 0;
        stopGeneration = false;
    }

    public void Stop()
    {
        stopGeneration = true;
    }

    void GenerateCloud()
    {
        Vector3 pos = transform.position;
        float y = pos.y + Random.Range(-cameraHeight/2, cameraHeight/2);
        float x = pos.x + cameraWidth / 2;

        GameObject cloud = clouds[Random.Range(0, clouds.Count)];
        float size = cloud.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x;
        CloudMovement cm = Instantiate(cloud, new Vector3(x + size, y, 0), Quaternion.identity).GetComponent<CloudMovement>();
        Vector3 scale = cm.gameObject.transform.localScale;
        scale *= Random.Range(0.9f, 1.5f);
        gameObject.transform.localScale = scale;
        cm.speed = Random.Range(4, 10);
        cm.borderX = (pos.x - cameraWidth / 2) - size * 4;
    }

    // Update is called once per frame
    void Update()
    {
        if (stopGeneration) return;
        delay -= Time.deltaTime;
        if(delay <= 0)
        {
            int nbCloud = Random.Range(1, 3);
            for (int i = 0; i < nbCloud; i++)
            {
                Invoke(nameof(GenerateCloud), Random.value);
            }
            delay = Random.Range(maxDelay - offsetDelay, maxDelay + offsetDelay);
        }
    }
}
