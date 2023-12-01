using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardState : MonoBehaviour
{
    List<BoardStateData> boardStateDatas = new List<BoardStateData>();
    public int testIndex;

    public void SaveBoardData(Slots[] slots)
    {
        var boardStateData = new BoardStateData();

        for (int i = 0; i < 24; i++)
        {
            boardStateData.slotState[i] = slots[i].state;
        }
        boardStateDatas.Add(boardStateData);
    }

    [Button("Test Load Data")]
    public void TestLoadData()
    {
        LoadBoardData(testIndex);
    }

    public void LoadBoardData(int index)
    {
        for (int i = 0; i < 24; i++)
        {
            print(boardStateDatas[index].slotState[i]);
        }
    }
}

public class BoardStateData
{
    public List<string> slotState;

    public BoardStateData()
    {
        slotState = new List<string>();
        for (int i = 0; i < 24; i++)
        {
            slotState.Add("");
        }
    }
}