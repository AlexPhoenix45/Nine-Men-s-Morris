using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAdd_NineMensMorris
{
    public class BoardState : MonoBehaviour
    {
        public static BoardState Instance;

        [SerializeField]
        public List<BoardStateData> boardStateDatas = new List<BoardStateData>();
        public int testIndex;

        public List<int> whiteMoves = new List<int>(); //White moves list
        public List<int> blackMoves = new List<int>(); //Black moves List

        public int currentMoveIndex = 0;

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
                whiteMoves.Add(currentMoveIndex);
            }
            else
            {
                blackMoves.Add(currentMoveIndex);
            }

            //Set player sleeping pieces
            boardStateData.whiteSleep = whitePlayer.pieceSleep;
            boardStateData.whiteLive = whitePlayer.pieceLive;
            boardStateData.whiteDie = whitePlayer.pieceDie;
            boardStateData.blackSleep = blackPlayer.pieceSleep;
            boardStateData.blackLive = blackPlayer.pieceLive;
            boardStateData.blackDie = blackPlayer.pieceDie;

            currentMoveIndex++;
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
            //Update sleeping pieces UI
            Table.Instance.whitePlayer.SetHowManyPieceIsSleeping(boardStateDatas[index].whiteSleep);
            Table.Instance.blackPlayer.SetHowManyPieceIsSleeping(boardStateDatas[index].blackSleep);

            //Update pieces parameters
            Table.Instance.whitePlayer.pieceSleep = boardStateDatas[index].whiteSleep;
            Table.Instance.whitePlayer.pieceLive = boardStateDatas[index].whiteLive;
            Table.Instance.whitePlayer.pieceDie = boardStateDatas[index].whiteDie;
            Table.Instance.blackPlayer.pieceSleep = boardStateDatas[index].blackSleep;
            Table.Instance.blackPlayer.pieceLive = boardStateDatas[index].blackLive;
            Table.Instance.blackPlayer.pieceDie = boardStateDatas[index].blackDie;


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

        [Button("Moves List")]
        public void PrintMoveList()
        {
            string mess = "";
            foreach (int i in whiteMoves)
            {
                mess += i.ToString() + " ";
            }
            Debug.Log("White Moves: " + mess);
            mess = "";
            foreach (int i in blackMoves)
            {
                mess += i.ToString() + " ";
            }
            Debug.Log("Black Moves: " + mess);
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
        public int whiteSleep;
        public int whiteLive;
        public int whiteDie;
        public int blackSleep;
        public int blackLive;
        public int blackDie;

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
}