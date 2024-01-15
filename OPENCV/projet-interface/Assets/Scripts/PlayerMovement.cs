using Emgu.CV;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PlayerMovement : MonoBehaviour
{
    bool wasJustClicked;
    bool canMove;
    Vector2 playerSize;
    Vector2 startingPosition;
    Rigidbody2D rb;

    #region Circle Detection

    Mat frame;

    VideoCamReader cam;
    CircleDetector circ;

    #endregion

    [SerializeField] private Transform boundaryHolder;
    [SerializeField] private bool IsRed;

    private Boundary playerBoundary;

    struct Boundary
    {
        public float Up;
        public float Down;
        public float Right;
        public float Left;

        public Boundary(float up, float down, float left, float right)
        {
            Up = up;
            Down = down;
            Left = left;
            Right = right;
        }
    }

    void Start()
    {
        cam = VideoCamReader.Instance;
        circ = new CircleDetector(cam);

        playerSize = GetComponent<SpriteRenderer>().bounds.size;
        rb = GetComponent<Rigidbody2D>();

        playerBoundary = new Boundary(
            boundaryHolder.GetChild(0).position.y,
            boundaryHolder.GetChild(1).position.y,
            boundaryHolder.GetChild(2).position.x,
            boundaryHolder.GetChild(3).position.x
            );
        startingPosition = transform.position;
    }

    void Update()
    {
        if (cam.IsOk && (frame = cam.GetData()).Size != new System.Drawing.Size(0, 0))
        {
            CircleDetector.DataCircle? dataOpt = circ.DetectColor(frame, IsRed ? Color.red : Color.green);
            if (dataOpt is not CircleDetector.DataCircle data) return;
            
            Vector2 unscaledPosition = data.center;
            Vector2 position = new(
                ((playerBoundary.Right - playerBoundary.Left) * unscaledPosition.x) - playerBoundary.Right,
                ((playerBoundary.Up - playerBoundary.Down) * unscaledPosition.y) - playerBoundary.Up
            );


            Vector2 clampedPosition = new(
                Mathf.Clamp(position.x, playerBoundary.Left, playerBoundary.Right),
                Mathf.Clamp(position.y, playerBoundary.Down, playerBoundary.Up)
            );

            rb.MovePosition(clampedPosition);
        } 

        //if (Input.GetMouseButton(0))
        //{
        //    Vector3 screenPosDepth = Input.mousePosition;
        //    screenPosDepth.z = 5.0f;
        //    Vector2 mousePos = Camera.main.ScreenToWorldPoint(screenPosDepth);

        //    if (wasJustClicked)
        //    {

        //        wasJustClicked = false;
        //        canMove = (Mathf.Sqrt(Mathf.Pow((transform.position.x - mousePos.x), 2) + Mathf.Pow((transform.position.y - mousePos.y), 2)) <= playerSize.x);
        //    }

        //    if (canMove)
        //    {
        //        Vector2 clampedMousePos = new Vector2(
        //            Mathf.Clamp(mousePos.x, playerBoundary.Left, playerBoundary.Right),
        //            Mathf.Clamp(mousePos.y, playerBoundary.Down, playerBoundary.Up)
        //        );

        //        rb.MovePosition(clampedMousePos);
        //    }
        //} else
        //{
        //    wasJustClicked = true;
        //}
    }

    public void ResetPosition()
    {
        rb.position = startingPosition;
    }
}
