using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardState : MonoBehaviour
{
    public static BoardState Instance;

    [SerializeField]
    public List<BoardStateData> boardStateDatas = new List<BoardStateData>();
    public int testIndex;

    public List<int> whiteMoves = new List<int>(); //White moves list
    public int currentWhiteMoveIndex = 0;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SaveBoardData(Slots[] slots, Table.CurrentPlayer currentPlayer, Player whitePlayer, Player blackPlayer, bool removingMove, bool isJumpMove)
    {
        print("GET DATA!");
        var boardStateData = new BoardStateData();

        //Set slot parameters
        //State
        for (int i = 0; i < 24; i++) 
        {
            boardStateData.SaveSlotParameters(slots[i].state, slots[i].isEmpty, slots[i].isWhite, slots[i].slotValue, slots[i].isMilled, slots[i].isFlared, isJumpMove, removingMove, i);
        }

        //Set grids
        for (int i = 0; i < 16; i++)
        {
            boardStateData.gridsValue[i] = Table.Instance.grids[i].activeSelf;
        }

        //Set player parameters
        boardStateData.currentPlayer = currentPlayer; 
        if (currentPlayer == Table.CurrentPlayer.White)
        {
            whiteMoves.Add(currentWhiteMoveIndex);
        }

        //Set player sleeping pieces
        boardStateData.whiteSleepingPieceLeft = whitePlayer.pieceSleep;
        boardStateData.blackSleepingPieceLeft = blackPlayer.pieceSleep;

        currentWhiteMoveIndex++;
        boardStateDatas.Add(boardStateData);
    }

    [Button("Test Load Data")]
    public void TestLoadData()
    {
        LoadBoardData(testIndex);
    }

    public void LoadBoardData(int index)
    {
        UIController.Instance.EmptySlots();
        for (int i = 0; i < 24; i++)
        {
            //Update slots state
            Table.Instance.slots[i].setPiece(boardStateDatas[index].state[i]); //UI Part

            Table.Instance.slots[i].isWhite = boardStateDatas[index].isWhite[i];
            Table.Instance.slots[i].isEmpty = boardStateDatas[index].isEmpty[i];
            Table.Instance.slots[i].isMilled = boardStateDatas[index].isMilled[i];
            Table.Instance.slots[i].slotValue = boardStateDatas[index].slotValue[i];
            Table.Instance.slots[i].state = boardStateDatas[index].state[i];

            if (boardStateDatas[index].isFlared[i])
            {
                Table.Instance.slots[i].setPiece("AddFlare");
            }
            else
            {
                Table.Instance.slots[i].setPiece("RemoveFlare");
            }
        }

        //Grids
        for (int i = 0; i < 16; i++)
        {
            Table.Instance.grids[i].SetActive(boardStateDatas[index].gridsValue[i]);
        }

        Table.Instance.currentPlayer = boardStateDatas[index].currentPlayer;
        //Update sleeping pieces
        Table.Instance.whitePlayer.SetHowManyPieceIsSleeping(boardStateDatas[index].whiteSleepingPieceLeft);
        Table.Instance.blackPlayer.SetHowManyPieceIsSleeping(boardStateDatas[index].blackSleepingPieceLeft);

        //Update turn
        Table.Instance.currentPlayer = boardStateDatas[index].currentPlayer;
        if (boardStateDatas[index].currentPlayer == Table.CurrentPlayer.White)
        {
            Table.Instance.whitePlayer.MyTurn(true);
            Table.Instance.blackPlayer.MyTurn(false);
        }
        else
        {
            Table.Instance.whitePlayer.MyTurn(false);
            Table.Instance.blackPlayer.MyTurn(true);
        }

        //Update table parameter
        Table.Instance.removingMove = boardStateDatas[index].removingMove;
        Table.Instance.isJumpMove = boardStateDatas[index].isJumpMove;
    }

    [Button("White Moves List")]
    public void PrintWhiteMoveList()
    {
        string mess = "";
        foreach (int i in whiteMoves)
        {
            mess += i.ToString() + " ";
        }
        Debug.Log("White Moves: " + mess);
    }
}

[System.Serializable]
public class BoardStateData
{
    //Slot paremeters
    public string[] state = new string[24];
    public bool[] isEmpty = new bool[24];
    public bool[] isWhite = new bool[24];
    public int[] slotValue = new int[24];
    public bool[] isMilled = new bool[24];
    public bool[] isFlared = new bool[24];

    //Line parameters
    public bool[] gridsValue = new bool[16];

    //Player parameters
    public Table.CurrentPlayer currentPlayer;
    public int whiteSleepingPieceLeft;
    public int blackSleepingPieceLeft;

    //Table parameters
    public bool isJumpMove = false;
    public bool removingMove = false;

    public BoardStateData()
    {
        for (int i = 0; i < 24; i++)
        {
            state[i] = "";
            isEmpty[i] = true;
            isWhite[i] = false;
            slotValue[i] = i;
            isMilled[i] = false;
        }
    }

    public void SaveSlotParameters(string state, bool isEmpty, bool isWhite, int slotValue, bool isMilled, bool isFlared, bool isJumpMove, bool removingMove, int index)
    {
        this.state[index] = state;
        this.isEmpty[index] = isEmpty;
        this.isWhite[index] = isWhite;
        this.slotValue[index] = slotValue;
        this.isMilled[index] = isMilled;
        this.isFlared[index] = isFlared;
        this.isJumpMove = isJumpMove;
        this.removingMove = removingMove;
    }
}