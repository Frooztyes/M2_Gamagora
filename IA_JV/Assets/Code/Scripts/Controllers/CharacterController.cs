using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterController : MonoBehaviour
{
    [Header("Ground collisions")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform groundChecker;

    [SerializeField] private float MoveSpeed;
    [SerializeField] private float ClimbSpeed;

    private bool defaultFacing;
    private bool canGoUp = false;
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] private AudioSource ladderSound;
    [SerializeField] private AudioSource walkSound;
    [SerializeField] private AudioSource swordSwoosh;
    [SerializeField] private AudioSource deathSound;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask ennemyLayer;

    private int nbJewels;
    private TextMeshProUGUI hudJewels;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Physics.gravity *= 40;
        isAttacking = false;
        defaultFacing = false; 
        nbJewels = 0;
        isDead = false;
    }

    private bool isGrounded;
    private bool isAttacking;
    private bool isDead;

    void Attack()
    {
        swordSwoosh.pitch = Random.Range(0.8f, 1.2f);
        swordSwoosh.Play();
        animator.SetTrigger("IsAttacking");
        Collider2D[] hits =  Physics2D.OverlapCircleAll(attackPoint.position, attackRange, ennemyLayer);
        Debug.Log(hits.Length);
        foreach(Collider2D collider in hits)
        {
            int layer = collider.gameObject.layer;

            if(layer == LayerMask.NameToLayer("Bird"))
            {
                BirdController contr = collider.GetComponent<BirdController>();

                contr.Kill();

            }
            else if (layer == LayerMask.NameToLayer("Skeleton"))
            {
                EnnemyController contr = collider.GetComponent<EnnemyController>();

                contr.TakeDamage();
            }





        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDead)
        {
            Attack();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDead) return;
        CheckGrounded();

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal > 0 && defaultFacing) FlipCharacter();
        else if (horizontal < 0 && !defaultFacing) FlipCharacter();
        if(horizontal != 0 && !walkSound.isPlaying && isGrounded)
        {
            walkSound.pitch = Random.Range(0.8f, 1.2f);
            walkSound.Play();
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        rb.velocity = new Vector2(horizontal * MoveSpeed * Time.deltaTime, rb.velocity.y);
        if(canGoUp && vertical != 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, vertical * ClimbSpeed * Time.deltaTime);
        }

        if(vertical != 0 && !isGrounded && !ladderSound.isPlaying)
        {
            ladderSound.pitch = Random.Range(0.8f, 1.2f);
            ladderSound.Play();
        }
    }

    void CheckGrounded()
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundChecker.position, 0.3f, ground);

        foreach (Collider2D col in colliders)
        {
            if (col.gameObject != gameObject)
            {
                isGrounded = true;
            }
        }
    }

    void FlipCharacter()
    {
        defaultFacing = !defaultFacing;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string layer = LayerMask.LayerToName(collision.gameObject.layer);

        if (layer == "Ground") return;

        if (layer == "Ladders")
        {
            canGoUp = true;
        }
        if (layer == "Jewel")
        {
            if (hudJewels == null)
            {
                hudJewels = GameObject.FindGameObjectWithTag("HUD_Jewels").GetComponent<TextMeshProUGUI>();
            }
            nbJewels++;
            hudJewels.text = nbJewels.ToString();
            Destroy(collision.gameObject);
        }
        if((layer == "Bird" || layer == "Skeleton") && !isDead)
        {
            deathSound.Play();
            transform.rotation = Quaternion.Euler(0, 0, 90f * (defaultFacing ? -1 : 1));
            GetComponent<Animator>().enabled = false;
            isDead = true;
            rb.gravityScale = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Ladders")
        {
            canGoUp = false;
        }
    }

}
