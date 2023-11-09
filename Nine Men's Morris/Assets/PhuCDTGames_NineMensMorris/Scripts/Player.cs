using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int pieceSleep = 9;
    public int pieceLive = 0;
    public int pieceDie = 0;

    public GameObject[] sleepingPiece;
    private int currentSleepingIndex = 8;

    public void ChangePieceAmount(string type, int amount)
    {
        if (type == "pieceSleep")
        {
            pieceSleep += amount;
            PieceAwake();
        }
        else if (type == "pieceLive")
        {
            pieceLive += amount;
        }
        else if (type == "pieceDie")
        {
            pieceDie += amount;
        }
    }

    private void PieceAwake()
    {
        if (currentSleepingIndex >= 0)
        {
            sleepingPiece[currentSleepingIndex].gameObject.SetActive(false);
            currentSleepingIndex--;
        }
    }
}
