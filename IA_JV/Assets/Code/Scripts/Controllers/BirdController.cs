using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [HideInInspector]
    public GameObject PositionInGraph;
    public List<Node> lastPath;

    [SerializeField] private float MoveSpeed = 1f;
    [SerializeField] private AudioSource DeathSound;
    [SerializeField] private AudioSource SqueakSound;

    private Vector3 dir;
    bool canMove;
    Rigidbody2D rb;
    private bool defaultFacing;
    private Animator animator;
    private CharacterController player;

    public Vector2 WaypointToGo
    {
        get
        {
            return dir;
        }

        set
        {
            canMove = true;
            dir = new Vector3(value.x, value.y, 0);
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody2D>();
        dir = Vector2.zero;
        canMove = false;
        defaultFacing = false;
        isDead = false;
        animator = GetComponent<Animator>();
    }

    public void Stop()
    {
        canMove = false;
    }

    private bool isDead = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player.IsDead) return;
        if(isDead)
        {
            if(!DeathSound.isPlaying)
            {
                Destroy(gameObject);
            }
            return;
        }
        Vector2 toDir = transform.position - dir;
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, dir, MoveSpeed * Time.fixedDeltaTime);
            if (toDir.x > 0 && defaultFacing) FlipCharacter();
            else if (toDir.x < 0 && !defaultFacing) FlipCharacter();
            animator.SetFloat("Speed", toDir.magnitude);
        } 
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    void FlipCharacter()
    {
        defaultFacing = !defaultFacing;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void Kill()
    {
        Destroy(GetComponent<SpriteRenderer>());
        DeathSound.pitch = Random.Range(0.8f, 1.2f);
        DeathSound.Play();
        isDead = true;
    }
}
