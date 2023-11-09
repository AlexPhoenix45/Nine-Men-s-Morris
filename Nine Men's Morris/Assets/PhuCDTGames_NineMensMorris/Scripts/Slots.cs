using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class Slots : MonoBehaviour
{
    public GameObject ring;
    public bool isEmpty = true;
    public bool isWhite = false;
    public int slotValue;
    public bool isMilled;
    public string state;

    private void Start()
    {
        ring.SetActive(false);
    }

    public void setPiece(string piece)
    {
        if (piece == "White") //Slot do = chesspiece Trang
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            isWhite = true;
            isEmpty = false;
            state = "White";
            isMilled = false;
            ring.SetActive(false);
        }
        else if (piece == "Black") //Slot do = chesspiece Den
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            isWhite = false;
            isEmpty = false;
            state = "Black";
            isMilled = false;
            ring.SetActive(false);
        }
        else if (piece == "Empty") //Day la slot rong~
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 0, 0, 0);
            isEmpty = true;
            state = "Empty";
            isMilled = false;
            ring.SetActive(false);
        }
        else if (piece == "Marker") //Slot do la nhung duong co the di cua chesspiece duoc chon
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
            isEmpty = true;
            state = "Marker";
            isMilled = false;
            ring.SetActive(true);
            ring.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else if (piece == "MillWhite") //Day la slot duoc tao boi 1 mill (3 in a row) - Mau Trang
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            state = "MillWhite";
            isMilled = true;
            ring.SetActive(true);
            ring.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (piece == "MillBlack") //Day la slot duoc tao boi 1 mill (3 in a row) - Mau Trang
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            state = "MillBlack";
            isMilled = true;
            ring.SetActive(true);
            ring.GetComponent<SpriteRenderer>().color = Color.red;
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
