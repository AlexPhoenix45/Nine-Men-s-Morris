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
                undoButton.interactable = true;
            }
        }
    }

    public void OnClick_Undo()
    {
        if (Table.Instance.isBotPlaying)
        {
            print("Last Undo Move: " + BoardState.Instance.whiteMoves[BoardState.Instance.whiteMoves.Count - 1]);
            //BoardState.Instance.LoadBoardData(BoardState.Instance.whiteMoves[BoardState.Instance.whiteMoves.Count - 1]);
            //BoardState.Instance.whiteMoves.RemoveAt(BoardState.Instance.whiteMoves[BoardState.Instance.whiteMoves.Count - 1]);
        }
    }

}
