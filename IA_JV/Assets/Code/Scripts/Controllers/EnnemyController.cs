using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyController : MonoBehaviour
{
    [HideInInspector]
    public Vector2? PositionInGraph;
    public List<Node> lastPath;

    [SerializeField] private AudioSource DeathSound;
    [SerializeField] private AudioSource HitSound;

    [SerializeField] private float MoveSpeed = 1f;
    [SerializeField] private int HealthPoints = 2;
    [SerializeField] private GameObject deathAnim;
    private int CurrentHeath;
    private float moveSpeedInternal;
    private Vector3 dir;
    private CharacterController player;
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
    private bool canMove;
    public bool IsDead { get; set; }

    public void Stop()
    {
        canMove = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        moveSpeedInternal = MoveSpeed;
        rb = GetComponent<Rigidbody2D>();
        dir = Vector2.zero;
        canMove = false;
        defaultFacing = true;
        walkEffect = GetComponent<AudioSource>();
        CurrentHeath = HealthPoints;
        isInvicible = false;
        IsDead = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player.IsDead) return;
        if (IsDead)
        {
            if (!DeathSound.isPlaying)
            {
                Destroy(gameObject);
            }
            return;
        }

        if (Vector3.Distance(transform.position, player.transform.position) > 20)
        {
            Destroy(gameObject);
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
                if (player != null && transform.position.x > player.transform.position.x && defaultFacing) FlipCharacter();
                else if (player != null && transform.position.x < player.transform.position.x && !defaultFacing) FlipCharacter();
            }
            else
            {
                if (dir.x > 0 && defaultFacing) FlipCharacter();
                else if (dir.x < 0 && !defaultFacing) FlipCharacter();
            }
        }
    }

    private ParticleSystem part;

    private bool isInvicible;

    void Unblink()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void Blink()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        Invoke(nameof(Unblink), 0.2f);
    }


    private void RemoveInvincibility()
    {
        isInvicible = false;
    }

    public void TakeDamage()
    {
        if (isInvicible || IsDead) return;
        isInvicible = true;
        CurrentHeath--;
        if(CurrentHeath <= 0)
        {
            part = Instantiate(deathAnim, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            DeathSound.pitch = Random.Range(0.8f, 1.2f);
            DeathSound.Play();
            Destroy(GetComponent<SpriteRenderer>());
            IsDead = true;
        } 
        else
        {
            Blink();
            HitSound.pitch = Random.Range(0.8f, 1.2f);
            HitSound.Play();
        }
        Invoke(nameof(RemoveInvincibility), 0.5f);
    }

    void FlipCharacter()
    {
        defaultFacing = !defaultFacing;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
