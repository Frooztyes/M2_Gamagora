using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckScript : MonoBehaviour
{
    [SerializeField] private ScoreScript scoreScriptInstance;
    [SerializeField] private AudioManager audioManager;
    public static bool WasGoal { get; private set; }

    private Rigidbody2D rb;
    [SerializeField] private float maxSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        WasGoal = false;
    }

    private Vector2 side;

    private void ResetPuck()
    {
        WasGoal = false;
        rb.velocity = Vector2.zero;
        rb.position = side;
        GetComponent<Collider2D>().enabled = true;
    }

    public void CenterPuck()
    {
        rb.position = Vector2.zero;
        rb.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        audioManager.PlayPuckCollision();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!WasGoal)
        {
            ScoreScript.Score? scored = null;

            if(other.CompareTag("RedGoal"))
            {
                scored = ScoreScript.Score.GreenScore;
                side = new Vector2(-1, 0);
            }

            if (other.CompareTag("GreenGoal"))
            {
                scored = ScoreScript.Score.RedScore;
                side = new Vector2(1, 0);
            }

            if(scored is ScoreScript.Score scoredOk)
            {
                audioManager.PlayGoal();
                scoreScriptInstance.Increment(scoredOk);
                WasGoal = true;
                GetComponent<Collider2D>().enabled = false;
                Invoke(nameof(ResetPuck), 1f);
            }
        }
    }
}
