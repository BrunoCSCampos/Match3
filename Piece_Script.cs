using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece_Script : MonoBehaviour
{
    
    public Game_Manager gameManagerScript;
    public Vector3 currentPosition;
    public Vector3 newPosition;

    public int pieceId;
    public bool isMoving = false;
    public bool isSelected = false;

    public List<GameObject> neighborsList = new List<GameObject>();
    public bool[] relativeNeighbors = new bool[] { false, false, false, false };

    // Start is called before the first frame update
    void Start()
    {
        UpdatePosition();
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").transform.GetComponent<Game_Manager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DetectSelection()
    {
        //Debug.Log("Detect Selection has worked");
        isSelected = true;

    }

    //Changes a piece position, while registering it in new position
    public void ActiveMovement()
    {
        
        Swipe_Manager swipeManagerScript = GameObject.FindGameObjectWithTag("Swipe Manager").transform.GetComponent<Swipe_Manager>();

        if (isSelected == true)
        {
            if(swipeManagerScript.swipeDirection == Swipe_Manager.SwipeDirection.Up)
            {
                isMoving = true;
                Debug.Log("Moving Up!");
                newPosition = new Vector3(currentPosition.x, currentPosition.y, (currentPosition.z + 1));
                transform.Translate(0, 0, 1f);
                ReactiveMovement();
            }

            else if (swipeManagerScript.swipeDirection == Swipe_Manager.SwipeDirection.Down)
            {
                isMoving = true;
                Debug.Log("Moving Down!");
                newPosition = new Vector3(transform.position.x, transform.position.y, (transform.position.z - 1));
                transform.Translate(0, 0, -1f);
                ReactiveMovement();
            }

            else if (swipeManagerScript.swipeDirection == Swipe_Manager.SwipeDirection.Left)
            {
                isMoving = true;
                Debug.Log("Moving Left!");
                newPosition = new Vector3((transform.position.x - 1), transform.position.y, transform.position.z);
                transform.Translate(-1f, 0, 0);
                ReactiveMovement();

            }

            else if (swipeManagerScript.swipeDirection == Swipe_Manager.SwipeDirection.Right)
            {
                isMoving = true;
                Debug.Log("Moving Right!");
                newPosition = new Vector3((transform.position.x + 1), transform.position.y, transform.position.z);
                transform.Translate(1f, 0, 0);
                ReactiveMovement();

            }
            
            Debug.Log("Piece Script has send its location to game manager");
            IEnumerator selectedPieceRoutine = gameManagerScript.LegalCheckerRoutine(transform.GetComponent<Piece_Script>());
            gameManagerScript.StartCoroutine(selectedPieceRoutine);
        }
       
        isSelected = false;
    }

    //Changes a neighbor position when piece is moved while registering its new position
    
    public void ReactiveMovement()
    {
        

        Collider[] neighbors = Physics.OverlapSphere(newPosition, 0.1f);
        foreach (var neighbor in neighbors)
        {
            if (neighbor.tag == "Piece")
            {
                var neighborScript = neighbor.GetComponent<Piece_Script>();
                if(neighborScript != null)
                {
                    if(neighborScript.currentPosition == newPosition)
                    {
                        Vector3 movedPosition = currentPosition;
                        neighborScript.transform.position = new Vector3(movedPosition.x, 0, movedPosition.z);
                        neighborScript.newPosition = new Vector3(transform.position.x, 0, transform.position.z);
                        IEnumerator neighborPieceRoutine = gameManagerScript.LegalCheckerRoutine(neighborScript);
                        gameManagerScript.StartCoroutine(neighborPieceRoutine);
                        
                    }
                }

            }
           
        }
       
    }

    //Decides if pieces should stay in the same position
    //Updates currentPosition to present position
    public void ConfirmMovement(Piece_Script piece)
    {
        if (piece != null)
        {
            if (gameManagerScript.movementLegal == true)
            {
                Debug.Log("Movement was legal, confirmed");
                Vector3 piecePosition = piece.transform.position;
                piece.currentPosition = piecePosition;
                piece.newPosition = piecePosition;
                piece.neighborsList.Clear();
            }
        }
        
    }

    //Decides if pieces should return to their previous position
    //Updates newPosition to present position
    public void UndoMovement(Piece_Script piece)
    {
        if (piece != null)
        {
            if (gameManagerScript.movementLegal == false)
            {
                Debug.Log("Movement was illegal, undoing");
                Vector3 piecePosition = piece.currentPosition;
                piece.transform.position = piecePosition;
                piece.currentPosition = transform.position;
                piece.newPosition = transform.position;

                piece.neighborsList.Clear();
            }
            else
            {
                piece.currentPosition = transform.position;
                piece.newPosition = transform.position;
                piece.neighborsList.Clear();

            }
        }
    }

    public void UpdatePosition()
    {
        currentPosition = transform.position;
    }



   





}
