using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAdd_NineMensMorris
{
    public class Player : MonoBehaviour
{
    public int pieceSleep = 9;
    public int pieceLive = 0;
    public int pieceDie = 0;

    public GameObject[] sleepingPiece;
    private int currentSleepingIndex = 8;

    public GameObject indicator;

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

    public void MyTurn(bool value)
    {
        indicator.SetActive(value);
    }

    public void Reset()
    {
        pieceSleep = 9;
        pieceLive = 0;
        pieceDie = 0;
        currentSleepingIndex = 8;
        for (int i = 0; i < 9; i++)
        {
            sleepingPiece[i].gameObject.SetActive(true);
        }
    }

    public void SetHowManyPieceIsSleeping(int numberOfSleepingPiece)
    {
        currentSleepingIndex = 8;
        for (int i = 0; i < 9; i++)
        {
            sleepingPiece[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < (9 - numberOfSleepingPiece); i++)
        {
            PieceAwake();
        }
    }
}
}
