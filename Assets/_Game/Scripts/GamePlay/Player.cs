using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direct { Forward, Back, Right, Left, None }

public class Player : MonoBehaviour
{
    //[SerializeField]
    public float speed = 5;
    public LayerMask layerBrick;
    public Transform playerBrickPrefab;
    public Transform brickHolder;
    public Transform playerSkin;

    private Vector3 mouseDown, mouseUp;
    private bool isMoving;
    private bool isSwiping;
    private Vector3 targetPoint;
    private List<Transform> playerBricks = new List<Transform>();


    void Update()
    {
        if (GameManager.Instance.IsState(GameState.Gameplay) && !isMoving)
        {
            if (Input.GetMouseButtonDown(0) && !isSwiping)
            {
                isSwiping = true;
                mouseDown = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0) && isSwiping)
            {
                isSwiping = false;
                mouseUp = Input.mousePosition;

                Direct direct = GetDirect(mouseDown, mouseUp);
                if(direct != Direct.None)
                {
                    targetPoint = GetNextPoint(direct);
                    isMoving= true;
                }
                
            }
        }
        else if (isMoving)
        {
            if(Vector3.Distance(transform.position, targetPoint) < 0.1f)
            {
                isMoving = false;
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, Time.deltaTime * speed);
        }
    }

    public void OnInit()
    {
        isMoving = false;
        ClearBrick();
        playerSkin.localPosition = Vector3.zero;
    }

    private Direct GetDirect(Vector3 mouseDown, Vector3 mouseUp)
    {
        Direct direct = Direct.None;

        float deltaX = mouseUp.x - mouseDown.x;
        float deltaY = mouseUp.y - mouseDown.y;

        if(Vector3.Distance(mouseDown , mouseUp) < 100)
        {
            direct = Direct.None;
        }

        else
        {
            if(Mathf.Abs(deltaY) > Mathf.Abs(deltaX))
            {
                if(deltaY > 0)
                {
                    direct = Direct.Forward;
                }
                else
                {
                    direct = Direct.Back;
                }
            }
            else
            {
                if(deltaX > 0)
                {
                    direct = Direct.Right;
                }
                else
                {
                    direct = Direct.Left;
                }
            }
        }

        return direct;
    }

    //private Vector3 GetNextPoint(Direct direct)
    //{
    //    RaycastHit hit;
    //    Vector3 nextPoint = Vector3.zero;
    //    Vector3 ahead = Vector3.zero;

    //    switch (direct)
    //    {
    //        case Direct.Forward:
    //            ahead = Vector3.forward;
    //            break;
    //        case Direct.Back:
    //            ahead = Vector3.back;
    //            break;
    //        case Direct.Left:
    //            ahead = Vector3.left;
    //            break;
    //        case Direct.Right:
    //            ahead = Vector3.right;
    //            break;
    //        case Direct.None:
    //            break;
    //        default:
    //            break;
    //    }

    //    for (int i = 1; i < 100; i++)
    //    {
    //        if (Physics.Raycast(transform.position + ahead * i + Vector3.up * 2, Vector3.down, out hit, 10f, layerBrick))
    //        {
    //            nextPoint = hit.collider.transform.position;
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }

    //    return nextPoint;
    //}

    private Vector3 GetNextPoint(Direct direct)
    {
        RaycastHit hit;
        Vector3 nextPoint = transform.position; // Start from the current position
        Vector3 ahead = Vector3.zero;

        // Determine direction
        switch (direct)
        {
            case Direct.Forward:
                ahead = transform.forward; // Uses character's forward direction
                break;
            case Direct.Back:
                ahead = -transform.forward;
                break;
            case Direct.Left:
                ahead = -transform.right;
                break;
            case Direct.Right:
                ahead = transform.right;
                break;
            case Direct.None:
                return nextPoint;
        }

        // Adjust raycasting loop
        for (int i = 1; i < 100; i++)
        {
            Vector3 rayOrigin = transform.position + ahead * i + Vector3.up * 2; // Start ray slightly above
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 2f, layerBrick))
            {
                nextPoint = hit.point; // Use hit point instead of collider's position
            }
            else
            {
                break;
            }
        }

        return nextPoint;
    }

    public void AddBrick()
    {
        int index = playerBricks.Count;

        Transform playerBrick = Instantiate(playerBrickPrefab, brickHolder);
        playerBrick.localPosition = Vector3.down + index * 0.25f * Vector3.up;
        playerBrick.localRotation = Quaternion.Euler(-90f, 0f, -180f);

        playerBricks.Add(playerBrick);
        playerSkin.localPosition = playerSkin.localPosition + Vector3.up * 0.25f;
    }

    //public void AddBrick()
    //{
    //    int index = playerBricks.Count;

    //    Transform playerBrick = Instantiate(playerBrickPrefab, brickHolder);
    //    playerBrick.localPosition = Vector3.down + index * 0.25f * Vector3.up;
    //    playerBrick.localRotation = Quaternion.Euler(-90f, 0f, -180f);

    //    playerBricks.Add(playerBrick);

    //    // Chỉ tăng chiều cao playerSkin mà không ảnh hưởng đến Capsule Collider
    //    playerSkin.localPosition += Vector3.up * 0.25f;
    //}

    public void RemoveBrick()
    {
        int index = playerBricks.Count - 1;

        if (index >= 0)
        {
            Transform playerBrick = playerBricks[index];
            playerBricks.RemoveAt(index);
            Destroy(playerBrick.gameObject);

            playerSkin.localPosition = playerSkin.localPosition - Vector3.up * 0.25f;
        }
    }

    //public void RemoveBrick()
    //{
    //    int index = playerBricks.Count - 1;

    //    if (index >= 0)
    //    {
    //        Transform playerBrick = playerBricks[index];
    //        playerBricks.RemoveAt(index);
    //        Destroy(playerBrick.gameObject);

    //        // Chỉ giảm chiều cao playerSkin mà không ảnh hưởng đến Capsule Collider
    //        playerSkin.localPosition -= Vector3.up * 0.25f;
    //    }
    //}

    public void ClearBrick()
    {
        for(int i = 0; i < playerBricks.Count; i++)
        {
            Destroy(playerBricks[i].gameObject);
        }

        playerBricks.Clear();
    }
}
