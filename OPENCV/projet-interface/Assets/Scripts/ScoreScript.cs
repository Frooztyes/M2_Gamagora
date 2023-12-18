using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    public enum Score
    {
        GreenScore, RedScore
    }

    [SerializeField] private TextMeshProUGUI redScoreText;
    [SerializeField] private TextMeshProUGUI greenScoreText;
    [SerializeField] private UiManager uiManager;

    [SerializeField] private int MaxScore;

    #region Scores
    private int greenScore;
    private int redScore;

    private int RedScore
    {
        get
        {
            return redScore;
        }

        set
        {
            redScore = value;

            if(value >= MaxScore)
            {
                uiManager.ShowRestartCanvas(true);
            }
        }
    }
    private int GreenScore
    {
        get
        {
            return greenScore;
        }

        set
        {
            greenScore = value;

            if (value >= MaxScore)
            {
                uiManager.ShowRestartCanvas(false);
            }
        }
    }

    #endregion



    public void Increment(Score whichScore)
    {
        if(whichScore == Score.GreenScore)
        {
            greenScoreText.text = (++GreenScore).ToString();
        }
        if (whichScore == Score.RedScore)
        {
            redScoreText.text = (++RedScore).ToString();
        }
    }

    public void ResetScores()
    {
        RedScore = GreenScore = 0;
        redScoreText.text = greenScoreText.text = "0";
    }


}
