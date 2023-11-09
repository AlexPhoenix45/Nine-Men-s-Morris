using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class Slots : MonoBehaviour
{
    public bool isEmpty = true;
    public bool isWhite = false;
    public int slotValue;
    public bool isMilled;
    public string state;

    [Header("Chess Piece")]
    public GameObject whitePiece;
    public GameObject blackPiece;

    public void setPiece(string piece)
    {
        if (piece == "White") //Slot do = chesspiece Trang
        {
            isWhite = true;
            isEmpty = false;
            state = "White";
            isMilled = false;

            //Setting UI
            //gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            whitePiece.SetActive(true);
            blackPiece.SetActive(false);
        }
        else if (piece == "Black") //Slot do = chesspiece Den
        {
            isWhite = false;
            isEmpty = false;
            state = "Black";
            isMilled = false;

            //Setting UI
            //gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            blackPiece.SetActive(true);
            whitePiece.SetActive(false);
        }
        else if (piece == "Empty") //Day la slot rong~
        {
            isEmpty = true;
            state = "Empty";
            isMilled = false;

            //Setting UI
            //gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 0, 0, 0);
            whitePiece.SetActive(false);
            blackPiece.SetActive(false);
        }
        else if (piece == "Marker") //Slot do la nhung duong co the di cua chesspiece duoc chon
        {
            //gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
            isEmpty = true;
            state = "Marker";
            isMilled = false;
        }
        else if (piece == "MillWhite") //Day la slot duoc tao boi 1 mill (3 in a row) - Mau Trang
        {
            //gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            state = "MillWhite";
            isMilled = true;
        }
        else if (piece == "MillBlack") //Day la slot duoc tao boi 1 mill (3 in a row) - Mau Trang
        {
            //gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            state = "MillBlack";
            isMilled = true;
        }
    }

    public void OnMouseDown()
    {
        print("Player choose: " + slotValue);
        StartCoroutine(Table.Instance.Evaluate(slotValue));
    }

    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
