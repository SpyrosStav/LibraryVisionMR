using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAreaBehaviour : MonoBehaviour
{
    [SerializeField] 
    GameObject X;
    [SerializeField] 
    GameObject O;
    [SerializeField] 
    GameObject parentObject;
    [SerializeField] 
    GameManager gameManager;
    string cubename,index;
    bool isAvailable;
    public bool isX;

    // Use this for initialization
    void Start()
    {   
        cubename = gameObject.name;
        index = cubename.Substring(cubename.Length - 2);
        isAvailable = true;
    }

    void OnSelect()
    {
        if(isAvailable)
        {
            Debug.Log("Selected");
            if(GameManager.isPlayer1Turn)
            {
                var playerSymbol = Instantiate(X, gameObject.transform.position,gameObject.transform.rotation);
                playerSymbol.transform.parent = parentObject.transform;

                GameManager.isPlayer1Turn = false;
                isX = true;
                isAvailable = false;

                gameManager.victoryCondition(index, 1);
            }
            else
            {
                var playerSymbol = Instantiate(O, gameObject.transform.position,gameObject.transform.rotation);
                playerSymbol.transform.parent = parentObject.transform;

                GameManager.isPlayer1Turn = true;
                isX = false;
                isAvailable = false;
                
                gameManager.victoryCondition(index, 2);
            }        
        }
    }
}
