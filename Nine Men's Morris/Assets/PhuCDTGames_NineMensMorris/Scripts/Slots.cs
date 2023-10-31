using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots : MonoBehaviour
{
    public bool isEmpty = true;
    public bool isWhite = false;
    public int slotValue;

    public void setPiece(string piece)
    {
        if (piece == "White")
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            isWhite = true;
            isEmpty = false;
        }
        else if (piece == "Black")
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            isWhite = false;
            isEmpty = false;
        }
        else if (piece == "Empty")
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 0, 0, 0.5333334f);
            isEmpty = true;
        }
        else if (piece == "Marker")
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
            isEmpty = true;
        }
    }

    public void OnMouseDown()
    {
        Table.Instance.Evaluate(slotValue);
    }
}
