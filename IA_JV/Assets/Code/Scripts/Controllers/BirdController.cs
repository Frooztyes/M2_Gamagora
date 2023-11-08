using System;
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

    private Vector3 dir;
    bool canMove;
    Rigidbody2D rb;
    private bool defaultFacing;
    private Animator animator;

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
        rb = GetComponent<Rigidbody2D>();
        dir = Vector2.zero;
        canMove = false;
        defaultFacing = false;
        animator = GetComponent<Animator>();
    }

    public void Stop()
    {
        canMove = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
        Destroy(gameObject);
    }
}
