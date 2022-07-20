using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Script : MonoBehaviour
{
    
    public bool isEmpty = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SimulateGravity();
    }

    //Keeps track of pieces on tiles
    
    public void PieceLocating()
    {
        Collider[] pieces = Physics.OverlapSphere(transform.position, 0.5f);
        foreach(var piece in pieces)
        {
            if (piece.tag == "Piece")
            {
                Debug.Log("There is a " + piece.name + " in location " + transform.name);
                isEmpty = false;
            }
                        
          
        }
        
        
    }

    //Makes pieces located above empty tiles drop down

    public void SimulateGravity()
    {
        if (isEmpty == true)
        {
            
            float maxDistance = 1f;
            RaycastHit raycastHit;
            if (Physics.Raycast(transform.position, Vector3.forward, out raycastHit, maxDistance))
            {
                
                Piece_Script pieceScript = raycastHit.transform.GetComponent<Piece_Script>();
                pieceScript.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
        }

    }

 

    

 }
