using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject canvasGame;
    [SerializeField] private GameObject canvasRestart;

    [Header("CanvasRestart")]
    [SerializeField] private GameObject redWin;
    [SerializeField] private GameObject greenWin;

    [Header("Other")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private ScoreScript scoreScript;
    [SerializeField] private PuckScript puckScript;

    [SerializeField] private PlayerMovement[] playerMovements = new PlayerMovement[2];

    public void ShowRestartCanvas(bool didRedWin)
    {
        Time.timeScale = 0;

        canvasGame.SetActive(false);
        canvasRestart.SetActive(true);

        if(didRedWin)
        {
            redWin.SetActive(true);
            greenWin.SetActive(false);
        } else
        {
            greenWin.SetActive(true);
            redWin.SetActive(false);
        }

        audioManager.PlayWonGame();
    }

    public void RestartGame()
    {
        Time.timeScale = 1;

        canvasGame.SetActive(true); 
        canvasRestart.SetActive(false);
        scoreScript.ResetScores();

        puckScript.CenterPuck();

        playerMovements[0].ResetPosition();
        playerMovements[1].ResetPosition();
    }
}
