using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndMenuHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI jewelNumber;
    [SerializeField] private TextMeshProUGUI jewelScore;

    [SerializeField] private TextMeshProUGUI ennemiesNumber;
    [SerializeField] private TextMeshProUGUI ennemiesScore;

    [SerializeField] private TextMeshProUGUI floorNumber;
    [SerializeField] private TextMeshProUGUI floorScore;

    [SerializeField] private TextMeshProUGUI totalScore;

    [SerializeField] private float scoreByJewel = 1;
    [SerializeField] private float scoreByEnnemies = 1;
    [SerializeField] private float scoreByFloor = 2;

    public void SetScore(
        int jewels,
        int ennemies,
        int floor
        )
    {
        jewelNumber.text = jewels.ToString();
        jewelScore.text = "+" + Mathf.Round(jewels * scoreByEnnemies).ToString();
        
        ennemiesNumber.text = ennemies.ToString();
        ennemiesScore.text = "+" + Mathf.Round(ennemies * scoreByEnnemies).ToString();

        floorNumber.text = floor.ToString();
        floorScore.text = "+" + Mathf.Round(floor * scoreByFloor).ToString();

        totalScore.text = (
            Mathf.Round(jewels * scoreByEnnemies) +
            Mathf.Round(ennemies * scoreByEnnemies) +
            Mathf.Round(floor * scoreByFloor)).ToString();

    }
}
