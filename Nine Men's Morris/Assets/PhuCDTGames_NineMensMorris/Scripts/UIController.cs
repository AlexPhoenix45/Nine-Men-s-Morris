using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace GameAdd_NineMensMorris
{
    public class UIController : MonoBehaviour
{
    public static UIController Instance;
    //Game Move Selection Parameters
    public GameObject gamemodePanel;

    //Endgame Parameters
    public GameObject endgamePanel;
    public TextMeshProUGUI endgameText;

    //Undo Parameter
    public Button whiteUndoButton;
    public Button blackUndoButton;

    private void Start()
    {
        gamemodePanel.SetActive(true);
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnClick_SelectGameModePlayer()
    {
        Table.Instance.isBotPlaying = false;
        gamemodePanel.SetActive(false);
    }    

    public void OnClick_SelectGameModeBot()
    {
        Table.Instance.isBotPlaying = true;
        gamemodePanel.SetActive(false);
    }

    public void SetEndgameText(string text)
    {
        endgamePanel.SetActive(true);
        endgameText.text = text;
    }

    public void OnClick_Restart()
    {
        endgamePanel.SetActive(false);
        foreach (Slots slot in Table.Instance.slots)
        {
            slot.setPiece("Empty");
            slot.setPiece("Marker");
        }

        for (int i = 0; i < 16; i++)
        {
            Table.Instance.grids[i].SetActive(false);
        }

        Table.Instance.blackPlayer.Reset();
        Table.Instance.whitePlayer.Reset();

        Table.Instance.whitePlayer.MyTurn(true);
        Table.Instance.blackPlayer.MyTurn(false);

        Table.Instance.currentPlayer = Table.CurrentPlayer.White;

        BoardState.Instance.boardStateDatas.Clear();
        BoardState.Instance.currentMoveIndex = 0;
        BoardState.Instance.whiteMoves.Clear();
        BoardState.Instance.blackMoves.Clear();

        whiteUndoButton.interactable = false;
    }

    public void UpdateUndoButtonTurn(Table.CurrentPlayer currentPlayer)
    {
        if (Table.Instance.isBotPlaying)
        {
            if (currentPlayer == Table.CurrentPlayer.Black)
            {
                whiteUndoButton.interactable = false;
                StopAllCoroutines();
            }
            else
            {
                if (BoardState.Instance.currentMoveIndex < 3)
                {
                    whiteUndoButton.interactable = false;
                    blackUndoButton.interactable = false;
                }
                else
                {
                    FreezeUndoButton(true);
                }
            }
        }
        else if (!Table.Instance.isBotPlaying)
        {
            if (BoardState.Instance.currentMoveIndex < 3)
            {
                whiteUndoButton.interactable = false;
                blackUndoButton.interactable = false;
            }
            else
            {
                if (currentPlayer == Table.CurrentPlayer.White)
                {
                    FreezeUndoButton(true);
                    blackUndoButton.interactable = false;
                    StopAllCoroutines();
                }
                else
                {
                    whiteUndoButton.interactable = false;
                    FreezeUndoButton(false);
                    StopAllCoroutines();
                }
            }
        }
    }

    public void EmptySlots()
    {
        foreach (Slots slot in Table.Instance.slots)
        {
            slot.setPiece("Empty");
        }
    }

    public void OnClick_Undo()
    {
        if (Table.Instance.currentPlayer == Table.CurrentPlayer.White)
        {
            int undoMove = BoardState.Instance.whiteMoves[BoardState.Instance.whiteMoves.Count - 2];
            int currentWhiteMoves = BoardState.Instance.whiteMoves[BoardState.Instance.whiteMoves.Count - 1];
            if (undoMove <= 0)
            {
                OnClick_Restart();
            }
            else
            {
                print("Last Undo Move: " + undoMove);
                print("Delete Move Index: " + currentWhiteMoves);

                //Restore previous data
                BoardState.Instance.LoadBoardData(undoMove);
                //Delete the current data
                BoardState.Instance.whiteMoves.RemoveAt(BoardState.Instance.whiteMoves.Count - 1);
                //Set some involved parameter to the previous one
                BoardState.Instance.currentMoveIndex = undoMove + 1;
                BoardState.Instance.boardStateDatas.RemoveRange(undoMove + 1, (BoardState.Instance.boardStateDatas.Count - undoMove) - 1);

                foreach (int item in BoardState.Instance.blackMoves.ToArray())
                {
                    if (item >= undoMove)
                    {
                        BoardState.Instance.blackMoves.Remove(item);
                    }
                }
            }
        }
        else if (Table.Instance.currentPlayer == Table.CurrentPlayer.Black)
        {
            int undoMove = BoardState.Instance.blackMoves[BoardState.Instance.blackMoves.Count - 2];
            int currentBlackMoves = BoardState.Instance.blackMoves[BoardState.Instance.blackMoves.Count - 1];
            if (undoMove <= 0)
            {
                OnClick_Restart();
            }
            else
            {
                print("Last Undo Move: " + undoMove);
                print("Delete Move Index: " + currentBlackMoves);

                //Restore previous data
                BoardState.Instance.LoadBoardData(undoMove);
                //Delete the current data
                BoardState.Instance.blackMoves.RemoveAt(BoardState.Instance.blackMoves.Count - 1);
                //Set some involved parameter to the previous one
                BoardState.Instance.currentMoveIndex = undoMove + 1;
                BoardState.Instance.boardStateDatas.RemoveRange(undoMove + 1, (BoardState.Instance.boardStateDatas.Count - undoMove) - 1);

                foreach (int item in BoardState.Instance.whiteMoves.ToArray())
                {
                    if (item >= undoMove)
                    {
                        BoardState.Instance.whiteMoves.Remove(item);
                    }
                }
            }
        }
        UpdateUndoButtonTurn(Table.Instance.currentPlayer);
    }
    
    public void FreezeUndoButton(bool isWhite)
    {
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(.5f);
            if (isWhite)
            {
                whiteUndoButton.interactable = true;
            }
            else
            {
                blackUndoButton.interactable = true;
            }
        }
        StartCoroutine(Wait());
    }
}
}
