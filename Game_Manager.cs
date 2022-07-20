using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Game_Manager : MonoBehaviour
{
    public GameObject detectorPrefab;
    public GameObject[] pieces;
    public Piece_Script pieceScript;
   
    public bool movementLegal = false;
    public Vector3[] adjacentDirections = new Vector3[] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
    public List<GameObject> matchedPieces = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SpawnPieces();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SpawnPieces()
    {
        Debug.Log("Starting piece spawn");
        foreach (Transform child in transform)
        {
            
          Instantiate(pieces[Random.Range(0, pieces.Length)], child.transform.position, Quaternion.identity);
          
           
        }
    }

    //Casts a ray on all adjacentDirections
    //Compares Id with adjacent neighbors
    //Compares neighbor Id with their neighbors
    //Confirms if a move was valid
    //Destroys any matching pieces in a 3-line

    public void LegalMovementCheck(Piece_Script piece)

    {
        if (piece != null)
        {
            float maxDistance = 1f;
            for (int i = 0; i <= adjacentDirections.Length - 1; i++)
            {
                RaycastHit firstHit;
                if (Physics.Raycast(piece.transform.position, adjacentDirections[i], out firstHit, maxDistance))
                {
                    Debug.Log("Firstcast hit a piece");
                    Piece_Script firstHitScript = firstHit.transform.GetComponent<Piece_Script>();
                    piece.neighborsList.Add(firstHitScript.gameObject);



                    if (piece.pieceId == firstHitScript.pieceId)
                    {
                        Debug.Log("A moved piece has a matching neighbor");

                        if (i == 0)
                        {
                            Debug.Log("There is a valid neighbor up");
                            piece.relativeNeighbors[0] = true;
                        }

                        if (i == 1)
                        {
                            Debug.Log("There is a valid neighbor down");
                            piece.relativeNeighbors[1] = true;
                        }

                        if (i == 2)
                        {
                            Debug.Log("There is a valid neighbor right");
                            piece.relativeNeighbors[2] = true;
                        }

                        if (i == 3)
                        {
                            Debug.Log("There is a  valid neighbor left");
                            piece.relativeNeighbors[3] = true;
                        }



                        RaycastHit secondHit;
                        if (Physics.Raycast(firstHit.transform.position, (firstHit.transform.position - piece.transform.position), out secondHit, maxDistance))
                        {
                            Debug.Log("Secondcast hit a piece");
                            Piece_Script secondHitScript = secondHit.transform.GetComponent<Piece_Script>();

                            if (firstHitScript.pieceId == secondHitScript.pieceId)
                            {
                                Debug.Log("Moved a piece forming a 3-line");
                                movementLegal = true;
                                piece.ConfirmMovement(piece);
                                matchedPieces.Add(piece.gameObject);
                                matchedPieces.Add(firstHitScript.gameObject);
                                matchedPieces.Add(secondHitScript.gameObject);


                            }
                            if (piece.relativeNeighbors[0] == true && piece.relativeNeighbors[1] == true || piece.relativeNeighbors[2] == true && piece.relativeNeighbors[3] == true)
                            {
                                Debug.Log("Piece is in the middle of a 3-line");
                                movementLegal = true;
                                piece.ConfirmMovement(piece);
                                matchedPieces.Add(piece.gameObject);
                                matchedPieces.Add(firstHitScript.gameObject);
                                matchedPieces.Add(secondHitScript.gameObject);
                            }




                        }



                    }

                }
            }
        }
        IEnumerator pieceCoroutine = transform.GetComponent<Game_Manager>().UndoMovementRoutine(piece);
        StartCoroutine(pieceCoroutine);
        StartCoroutine("NewTurnRoutine");
        DestroyPieces();

    }

    //Makes sure pieces are at their final destination before casting
    public IEnumerator LegalCheckerRoutine(Piece_Script piece)
    {
        if (piece != null)
        {
            Debug.Log("Coroutine has started for:" + piece.name);
            yield return new WaitForSeconds(1);
            LegalMovementCheck(piece);

            IEnumerator pieceCoroutine = transform.GetComponent<Game_Manager>().LegalCheckerRoutine(piece);
            yield return new WaitForSeconds(0.5f);
            StopCoroutine(pieceCoroutine);
        }
    }


    //Resets the gameManager to its default
    public void NewTurnLoop()
    {
      
        movementLegal = false;
        TilesFeedback();
    }


    public IEnumerator NewTurnRoutine()
    {
        yield return new WaitForSeconds(1f);
        NewTurnLoop();
        Debug.Log("New Turn Coroutine has ended");
        StopCoroutine("NewTurnRoutine");
    }

    //Makes sure UndoMovement is not coming out too soon
    public IEnumerator UndoMovementRoutine(Piece_Script piece)
    {
        if (piece != null)
        {
            yield return new WaitForSeconds(0.5f);
            piece.UndoMovement(piece);
            IEnumerator pieceCoroutine = transform.GetComponent<Game_Manager>().UndoMovementRoutine(piece);
            StopCoroutine(pieceCoroutine);
        }

    }

    
    public void DestroyPieces()
    {
        if (matchedPieces.Count > 1)
        {
            foreach (var piece in matchedPieces)
            {
                Destroy(piece);
                
            }
            
        }
        
        //else if(matchesPieces.Count > 5)
        //{
        //    //Instantiate a powerful piece
        //    //at matchedPieces[0].transform.position
        //}
             
        
    }

    
    public void TilesFeedback()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Debug.Log("The locating loop should have started");
            Tile_Script thisChild = transform.GetChild(i).GetComponent<Tile_Script>();
            thisChild.PieceLocating();
        }
    }

  

  
  

}
