using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyController : MonoBehaviour
{
    [HideInInspector]
    public Vector2? PositionInGraph;
    public List<Node> lastPath;

    [SerializeField] private float MoveSpeed = 1f;
    [SerializeField] private int HealthPoints = 2;
    [SerializeField] private GameObject deathAnim;
    private int CurrentHeath;
    private float moveSpeedInternal;
    private Vector3 dir;
    private Transform player;
    private bool defaultFacing;
    public Vector2 WaypointToGo
    {
        get
        {
            return dir;
        }

        set
        {
            canMove = true;
            dir = new Vector3(value.x, value.y - 0.5f, 0);
        }
    }

    Rigidbody2D rb;
    AudioSource walkEffect;
    bool canMove;

    public void Stop()
    {
        canMove = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        moveSpeedInternal = MoveSpeed;
        rb = GetComponent<Rigidbody2D>();
        dir = Vector2.zero;
        canMove = false;
        defaultFacing = true;
        walkEffect = GetComponent<AudioSource>();
        CurrentHeath = HealthPoints;
        isInvicible = false;
        isDying = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDying)
        {
            if(part == null)
            {
                Destroy(gameObject);
            }
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 2.5f * Time.fixedDeltaTime);
        }

        if (canMove)
        {
            Vector3 oldPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, dir, MoveSpeed * Time.fixedDeltaTime);

            if((oldPosition - transform.position).magnitude > 0.02f && !walkEffect.isPlaying && dir.y != 0)
            {
                walkEffect.pitch = Random.Range(0.8f, 1.2f);
                walkEffect.Play();
            }

            if (dir.y != 0)
            {
                if (transform.position.x > player.position.x && defaultFacing) FlipCharacter();
                else if (transform.position.x < player.position.x && !defaultFacing) FlipCharacter();
            }
            else
            {
                if (dir.x > 0 && defaultFacing) FlipCharacter();
                else if (dir.x < 0 && !defaultFacing) FlipCharacter();
            }
        }
    }

    private ParticleSystem part;
    private bool isDying;

    private bool isInvicible;

    private void Die()
    {
        part = Instantiate(deathAnim, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        isDying = true;
    }

    private void RemoveInvincibility()
    {
        isInvicible = false;
    }

    public void TakeDamage()
    {
        if (isInvicible) return;
        isInvicible = true;
        CurrentHeath--;
        if(CurrentHeath <= 0)
        {
            Die();
        }
        Invoke(nameof(RemoveInvincibility), 0.5f);
    }

    void FlipCharacter()
    {
        defaultFacing = !defaultFacing;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
