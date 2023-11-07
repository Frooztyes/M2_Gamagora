using System;
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

    private int nbJewels;
    private TextMeshProUGUI hudJewels;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Physics.gravity *= 40;
        defaultFacing = false; 
        nbJewels = 0;
    }

    private bool isGrounded;

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckGrounded();

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal > 0 && defaultFacing) FlipCharacter();
        else if (horizontal < 0 && !defaultFacing) FlipCharacter();
        if(horizontal != 0 && !walkSound.isPlaying && isGrounded)
        {
            walkSound.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
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
            ladderSound.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Ladders")
        {
            canGoUp = false;
        }
    }

}
