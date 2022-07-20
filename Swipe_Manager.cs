using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe_Manager : MonoBehaviour
{
    public enum SwipeDirection { None, Up, Down, Left, Right }
    public float minimumSwipeLenght = 20f;

    public Vector2 swipePressPosition;
    public Vector2 swipeReleasePosition;
    public Vector2 currentSwipe;
    public Piece_Script pieceObject;

    public SwipeDirection swipeDirection;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ConfirmSwipe();
    }

    public void ConfirmSwipe()
    {
        foreach (Touch touch in Input.touches)
        {
            //Gets the position in screen where a touch happened
            //Resets the release position at every Touch Start

            if (touch.phase == TouchPhase.Began)
            {
                swipePressPosition = touch.position;
                swipeReleasePosition = touch.position;
                DetectTouch();    
            }

            //Gets and returns a string where the touch ended in screen


            if (touch.phase == TouchPhase.Ended)
            {
                swipeReleasePosition = touch.position;
                currentSwipe = new Vector3((swipeReleasePosition.x - swipePressPosition.x), (swipeReleasePosition.y - swipePressPosition.y)).normalized;
                DetectSwipe();
                Debug.Log("Position X:" + currentSwipe.x.ToString() + "Position Y:" + currentSwipe.y.ToString());
                Debug.Log("Swiped in direction:" + swipeDirection);
                pieceObject.ActiveMovement();
               if (swipeDirection != SwipeDirection.None)
                {
                    Debug.Log("Swiped in direction:" + swipeDirection);
                }


            }
            swipeDirection = SwipeDirection.None;


        }

    }

    //Gets the position and returns the relative direction of a swipe
    //Returns no direction if a swipe hasn't occured

    public void DetectSwipe()
    {
        if (SwipeDistanceMet())
        {
            if (IsVerticalSwipe())
            {
                var direction = swipePressPosition.y - swipeReleasePosition.y < 0 ? swipeDirection = SwipeDirection.Up : swipeDirection = SwipeDirection.Down;
            //Debug.Log("VerticalMovement is:" + VerticalMovementDistance());
                SendSwipe(direction);
            }

            else
            {
                var direction = swipePressPosition.x - swipeReleasePosition.x < 0 ? swipeDirection = SwipeDirection.Right : swipeDirection = SwipeDirection.Left;
                //Debug.Log("HorizontalMovement is:" + HorizontalMovementDistance());
                SendSwipe(direction);
            }
        }

        else
        {
            swipeDirection = SwipeDirection.None;
            SendSwipe(SwipeDirection.None);
            Debug.Log("Player has Tapped");
        }

        swipeReleasePosition = swipePressPosition;
    }

    //Stores the data of a Swipe
    public struct SwipeData
    {
        public Vector2 StartPosition;
        public Vector2 EndPosition;
        public SwipeDirection Direction;

    }
    
    public void SendSwipe(SwipeDirection direction)
    {

        SwipeData swipeData = new SwipeData()
        {
            Direction = direction,
            StartPosition = swipePressPosition,
            EndPosition = swipeReleasePosition,
        };

    }

    //Calculates the size of the Swipe to assure it's not a tap
    public float VerticalMovementDistance()
    {
        return Mathf.Abs(swipePressPosition.y - swipeReleasePosition.y);
    }


    public float HorizontalMovementDistance()
    {
        return Mathf.Abs(swipePressPosition.x - swipeReleasePosition.x);
    }
    public bool SwipeDistanceMet()

    {
        return VerticalMovementDistance() > minimumSwipeLenght || HorizontalMovementDistance() > minimumSwipeLenght;
    }

    //Returns true if a swipe is mostly Vertical
    //Otherwise it's Horizontal
    public bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    public void DetectTouch()
    {
        Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit rayCastHit;
        if (Physics.Raycast(raycast, out rayCastHit))
        {
            if (rayCastHit.collider.CompareTag("Piece"))
            {
                Debug.Log("Selected the Piece: " + rayCastHit.transform.name + " at " + rayCastHit.transform.position);
                Piece_Script selectedPiece = rayCastHit.transform.GetComponent<Piece_Script>();
                if(selectedPiece != null)
                {
                    selectedPiece.DetectSelection();
                    pieceObject = selectedPiece;
                }
            }
        }
    }



   
}
