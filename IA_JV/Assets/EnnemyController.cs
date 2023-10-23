using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class EnnemyController : MonoBehaviour
{
    [HideInInspector]
    public GameObject PositionInGraph;

    [SerializeField] private float MoveSpeed = 20f;

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
            dir = new Vector3(value.x, value.y, 0);
        }
    }

    Rigidbody2D rb;

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
        canMove = true;
        defaultFacing = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(canMove && dir != Vector3.zero)
        {

            rb.MovePosition(transform.position - moveSpeedInternal * Time.fixedDeltaTime * dir);
            if(dir.y != 0)
            {
                if (transform.position.x > player.position.x && defaultFacing) FlipCharacter();
                else if (transform.position.x < player.position.x && !defaultFacing) FlipCharacter();
            } else
            {
                if (dir.x > 0 && defaultFacing) FlipCharacter();
                else if (dir.x < 0 && !defaultFacing) FlipCharacter();
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
        //if (LayerMask.LayerToName(collision.gameObject.layer) == "Ladders")
        //{
        //    moveSpeedInternal = MoveSpeed / 2;
        //}
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (LayerMask.LayerToName(collision.gameObject.layer) == "Ladders")
        //{
        //    moveSpeedInternal = MoveSpeed;
        //}
    }
}
