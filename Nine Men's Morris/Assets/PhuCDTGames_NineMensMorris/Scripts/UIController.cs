using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    //Game Move Selection Parameters
    public GameObject gamemodePanel;

    //Endgame Parameters
    public GameObject endgamePanel;
    public TextMeshProUGUI endgameText;

    //Undo Parameter
    public Button undoButton;

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

        BoardState.Instance.boardStateDatas.Clear();
        BoardState.Instance.currentWhiteMoveIndex = 0;
        BoardState.Instance.whiteMoves.Clear();

        undoButton.interactable = false;
    }

    public void UpdateUndoButtonTurn(Table.CurrentPlayer currentPlayer)
    {
        if (Table.Instance.isBotPlaying)
        {
            if (currentPlayer == Table.CurrentPlayer.White)
            {
                undoButton.interactable = false;
            }
            else
            {
                if (BoardState.Instance.currentWhiteMoveIndex > 1)
                {
                    undoButton.interactable = true;
                }
                else
                {
                    undoButton.interactable = false;
                }
            }
        }
        else
        {
            if (BoardState.Instance.boardStateDatas.Count > 4)
            {
                undoButton.interactable = true;
            }
            else
            {
                undoButton.interactable = false;
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
        int undoMove = BoardState.Instance.whiteMoves[BoardState.Instance.whiteMoves.Count - 2];
        int currentWhiteMoves = BoardState.Instance.whiteMoves[BoardState.Instance.whiteMoves.Count - 1]; 

        if (Table.Instance.isBotPlaying)
        {
            print("Last Undo Move: " + undoMove);
            print("Delete Move Index: " + currentWhiteMoves);

            //Restore previous data
            BoardState.Instance.LoadBoardData(undoMove);
            //Delete the current data
            BoardState.Instance.whiteMoves.RemoveAt(BoardState.Instance.whiteMoves.Count - 1);
            //Set some involved parameter to the previous one
            BoardState.Instance.currentWhiteMoveIndex = undoMove;
            BoardState.Instance.boardStateDatas.RemoveRange(undoMove + 1, (BoardState.Instance.boardStateDatas.Count - undoMove) - 1);
        }
    }
}
