using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Direction
{
    Idle,
    Up,
    Down,
    Left,
    Right
}
[System.Serializable]
public class Movement
{
    [SerializeField] public Direction Direction;
    public float Distance = 1;
    public float TimeOnIdleAfter = 0;
}
public enum NPCMode
{
    FollowMovements,
    FollowPlayer,
    None
}
public enum FacingDirection
{
    UpIdle,
    DownIdle,
    LeftIdle,
    RightIdle,
    None
}
public class FollowData
{
    public Vector2 Pos;
    public Direction Direction;
}

public class NPCBrain : MonoBehaviour
{
    [SerializeField] public NPCMode Mode;
    [SerializeField] public FacingDirection StartingOrientation;
    [Header("FollowMovements breaks after vel: 5!")]
    public float vel = 1;
    public bool StopExec;
    [Header("FollowMovements Properties.")]
    public bool Loop;
    public bool MovementsDone = true;
    public bool RunOnStart;
    public List<Movement> Movements = new List<Movement>();

    [Header("FollowPlayer Properties.")]
    public PlayerMovement PlayerScript;
    public int QueuePosition;
    public int latencyBetween;
    private Queue<FollowData> playerpositions = new Queue<FollowData>();
    private Vector2 lastplayerpos;
    private bool CanMove;
    private FollowData CurrentFollowData;

    [Header("If the subject is NOT this gameobject, set the proper settings of SubjectRB and\nanimator.")]
    public Animator animator;
    private string currentState;
    private Vector2 direction = new Vector2(0, 0);
    public Rigidbody2D SubjectRB;
    private string Orientation;
    private Vector2 InitialPos = new Vector2(0, 0);
    private bool PosMet;
    private Vector2 ParsedDistance;
    public void Start()
    {
        if (SubjectRB == null)
        {
            SubjectRB = GetComponent<Rigidbody2D>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (StartingOrientation != FacingDirection.None)
        {
            ChangeAnimation(StartingOrientation.ToString());
        }
        if (Mode == NPCMode.FollowMovements)
        {
            if (!StopExec && RunOnStart)
            {
                InitialPos = new Vector2(Mathf.FloorToInt(SubjectRB.gameObject.transform.position.x), Mathf.FloorToInt(SubjectRB.gameObject.transform.position.y));
                StartCoroutine(StartMovement(0));
            }
        }
        if (Mode == NPCMode.FollowPlayer)
        {
            FollowData followdata = new FollowData();
            followdata.Pos = PlayerScript.gameObject.transform.position;
            followdata.Direction = ParseVectorToDirection(PlayerScript.direction);
            playerpositions.Enqueue(followdata);
            lastplayerpos = PlayerScript.gameObject.transform.position;
        }
    }
    Direction ParseVectorToDirection(Vector2 direction)
    {
        if (direction == Vector2.left)
        {
            return Direction.Left;
        }
        else if (direction == Vector2.right)
        {
            return Direction.Right;
        }
        else if (direction == Vector2.up)
        {
            return Direction.Up;
        }
        else
        {
            return Direction.Down;
        }
    }
    Vector2 ParseDistance(Direction direction, float distance)
    {
        if (direction == Direction.Up)
        {
            return new Vector2(0, distance);
        }
        else if (direction == Direction.Down)
        {
            return new Vector2(0, -1 * distance);
        }
        else if (direction == Direction.Left)
        {
            return new Vector2(-1 * distance, 0);
        }
        else
        {
            return new Vector2(distance, 0);
        }
    }
    public IEnumerator StartMovement(int i)
    {
        InitialPos = new Vector2(Mathf.FloorToInt(SubjectRB.gameObject.transform.position.x), Mathf.FloorToInt(SubjectRB.gameObject.transform.position.y));
        PosMet = false;
        MovementsDone = false;
        if (i < Movements.Count)
        {
            if (Movements[i].Direction == Direction.Left)
            {
                direction = new Vector2(-1, 0);
            }
            if (Movements[i].Direction == Direction.Right)
            {
                direction = new Vector2(1, 0);
            }
            if (Movements[i].Direction == Direction.Up)
            {
                direction = new Vector2(0, 1);
            }
            if (Movements[i].Direction == Direction.Down)
            {
                direction = new Vector2(0, -1);
            }
            if (Movements[i].Direction == Direction.Idle)
            {
                direction = new Vector2(0, 0);
            }
        }
        if (direction == new Vector2(0, 1))
        {
            ChangeAnimation("Up");
            Orientation = "Up";
        }
        if (direction == new Vector2(-1, 0))
        {
            ChangeAnimation("Left");
            Orientation = "Left";
        }
        if (direction == new Vector2(0, -1))
        {
            ChangeAnimation("Down");
            Orientation = "Down";
        }
        if (direction == new Vector2(1, 0))
        {
            ChangeAnimation("Right");
            Orientation = "Right";
        }
        ParsedDistance = ParseDistance(Movements[i].Direction, Movements[i].Distance);
        yield return new WaitUntil(() => PosMet);
        if (Movements[i].TimeOnIdleAfter != 0)
        {
            direction = Vector2.zero;
            if (Orientation == "Up")
            {
                ChangeAnimation("UpIdle");
            }
            if (Orientation == "Left")
            {
                ChangeAnimation("LeftIdle");
            }
            if (Orientation == "Down")
            {
                ChangeAnimation("DownIdle");
            }
            if (Orientation == "Right")
            {
                ChangeAnimation("RightIdle");
            }
            yield return new WaitForSeconds(Movements[i].TimeOnIdleAfter);
        }
        i++;
        if (i == Movements.Count && Loop)
        {
            InitialPos = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
            StartCoroutine(StartMovement(0));
        }
        else if (i == Movements.Count && !Loop)
        {
            direction = Vector2.zero;
            if (Orientation == "Up")
            {
                ChangeAnimation("UpIdle");
            }
            if (Orientation == "Left")
            {
                ChangeAnimation("LeftIdle");
            }
            if (Orientation == "Down")
            {
                ChangeAnimation("DownIdle");
            }
            if (Orientation == "Right")
            {
                ChangeAnimation("RightIdle");
            }
            MovementsDone = true;
        }
        else
        {
            InitialPos = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
            StartCoroutine(StartMovement(i));
        }
    }
    public void ChangeAnimation(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }
    private void FixedUpdate()
    {
        if (!MovementsDone)
        {
            // Player Movement
            Vector2 delta = direction * vel * Time.deltaTime;
            Vector2 newPos = SubjectRB.position + delta;
            SubjectRB.MovePosition(newPos);
        }
        Vector2 pos = new Vector2();
        if (Mode == NPCMode.FollowMovements)
        {
            pos = new Vector2(Mathf.FloorToInt(SubjectRB.gameObject.transform.position.x), Mathf.FloorToInt(SubjectRB.gameObject.transform.position.y));
        }
        else
        {
            pos = new Vector2(SubjectRB.gameObject.transform.position.x, SubjectRB.gameObject.transform.position.y);
        }

        Vector2 iv = new Vector2(ParsedDistance.x, ParsedDistance.y);
        if (pos == iv + InitialPos)
        {
            PosMet = true;
        }
        else
        {
            PosMet = false;
        }

        // Follow Player
        if (Mode == NPCMode.FollowPlayer)
        {
            if (PlayerScript.gameObject.transform.position != (Vector3)lastplayerpos)
            {
                FollowData followdata = new FollowData();
                followdata.Pos = PlayerScript.gameObject.transform.position;
                followdata.Direction = ParseVectorToDirection(PlayerScript.direction);
                playerpositions.Enqueue(followdata);
                lastplayerpos = PlayerScript.gameObject.transform.position;

                if (playerpositions.Count >= latencyBetween * QueuePosition)
                {
                    CanMove = true;
                }
                if (CanMove)
                {
                    CurrentFollowData = playerpositions.Dequeue();
                    gameObject.transform.position = CurrentFollowData.Pos;
                    animator.Play(CurrentFollowData.Direction.ToString());
                }
            }
            else if (CurrentFollowData != null)
            {
                animator.Play(CurrentFollowData.Direction.ToString() + "Idle");
            }
        }
    }
}
