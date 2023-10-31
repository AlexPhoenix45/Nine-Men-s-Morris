using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public static Table Instance;
    private enum CurrentPlayer
    { 
        White,
        Black
    }

    public Slots[] slots;
    private CurrentPlayer currentPlayer = CurrentPlayer.White;
    public Player whitePlayer;
    public Player blackPlayer;

    //This part is for moving a chesspiece
    private bool selectToMove = false;
    private List<int> adjacentSlots = new List<int>();
    private int lastestSlot;

    private void Start() //Set slot value for each empty slot
    {
        int slotValue = 0;
        foreach (Slots slot in slots)
        {
            slot.slotValue = slotValue;
            slotValue++;
        }

        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void Evaluate(int slotValue)
    {
        if (selectToMove)
        {
            MoveChesspiece(lastestSlot, slotValue, adjacentSlots);
            adjacentSlots.Clear();
            selectToMove = false;
        }
        else
        {
            //Khi click vao 1 slot
            //Check xem slot do con trong hay khong, neu co thi dat chesspiece vao do, dieu kien la chua dat het 9 chesspiece
            if (CheckIfSlotIsEmpty(slotValue) && ReturnCurrentPlayer(currentPlayer).pieceSleep != 0)
            {
                PlaceChesspiece(slotValue); //Khi player da dat du so chesspiece cua minh thi se khong chay vao day nua
            }
            else if (!CheckIfSlotIsEmpty(slotValue) && ReturnCurrentPlayer(currentPlayer).pieceSleep == 0) //Phai dat het 9 chesspiece moi duoc di chuyen nhung chesspiece tren table
            {
                //Neu slot do khong trong, xem player no an dung vao chesspiece cua minh hay khong, neu an dung thi hien ra nhung nuoc di co the di cua chesspiece do
                CheckChesspieceAndShowAdjacent(slotValue);
            }

            //Check xem co 3 in a row nao hay khong
            if (Check3InARow(currentPlayer)) //Neu co va la cua currentPlayer
            {
                //MoveAPieceOfOpponent();
            }
            else //Neu khong co gi, doi luot choi
            {
                SwitchPlayer();
            }


        }

    }
    private bool CheckIfSlotIsEmpty(int slotValue)
    {
        if (slots[slotValue].isEmpty) //Neu slot nay trong
        {
            print(true);
            return true;
        }
        else //Neu slot nay khong trong
        {
            print(false);
            return false;
        }

    }

    private void PlaceChesspiece(int slotValue)
    {
        if (currentPlayer == CurrentPlayer.White) //Neu nhu nguoi choi hien tai la trang thi dat chesspiece trang vao slot do
        {
            slots[slotValue].setPiece("White");
            whitePlayer.pieceSleep--;
            whitePlayer.pieceLive++;
            SwitchPlayer(); //Dat chesspiece xong thi doi luot choi
        }
        else if (currentPlayer == CurrentPlayer.Black) //Neu nhu nguoi choi hien tai la den thi dat chesspiece den vao slot do
        {
            slots[slotValue].setPiece("Black");
            blackPlayer.pieceSleep--;
            blackPlayer.pieceLive++;
            SwitchPlayer(); //Dat chesspiece xong thi doi luot choi
        }
    }


    private void CheckChesspieceAndShowAdjacent(int slotValue)
    {
        if (currentPlayer == CurrentPlayer.White) 
        {
            if (slots[slotValue].isWhite) //Neu nguoi choi mau trang va an vao chesspiece mau trang
            {
                foreach (int value in CheckAdjacent(slotValue))
                {
                    if (slots[value].isEmpty)
                    {
                        slots[value].setPiece("Marker"); //Hien ra nhung o con trong ben canh o cua nguoi choi 
                        adjacentSlots.Add(value);
                        lastestSlot = slotValue;
                        selectToMove = true;
                    }
                }
            }
            else //Neu khong thi thoat luon
            {
                return;
            }
        }
        else if (currentPlayer == CurrentPlayer.Black) 
        {
            if (!slots[slotValue].isWhite) //Neu nguoi choi mau den va an vao chesspiece mau den
            {
                foreach (int value in CheckAdjacent(slotValue))
                {
                    if (slots[value].isEmpty)
                    {
                        slots[value].setPiece("Marker"); //Hien ra nhung o con trong ben canh o cua nguoi choi 
                        adjacentSlots.Add(value);
                        lastestSlot = slotValue;
                        selectToMove = true;
                    }
                }
            }
            else
            {
                return;
            }

        }
    }

    private int[] CheckAdjacent(int slotValue)
    {
        switch (slotValue)
        {
            case 0:
                return new int[] { 1, 9 };
            case 1:
                return new int[] { 0, 2, 4 };
            case 2:
                return new int[] { 1, 14 };
            case 3:
                return new int[] { 4, 10 };
            case 4:
                return new int[] { 1, 3, 5, 7 };
            case 5:
                return new int[] { 4, 13 };
            case 6:
                return new int[] { 7, 11 };
            case 7:
                return new int[] { 6, 4, 8 };
            case 8:
                return new int[] { 7, 12 };
            case 9:
                return new int[] { 0, 10, 21 };
            case 10:
                return new int[] { 3, 9, 11, 18 };
            case 11:
                return new int[] { 6, 10, 15 };
            case 12:
                return new int[] { 8, 17, 13 };
            case 13:
                return new int[] { 5, 12, 14, 20 };
            case 14:
                return new int[] { 2, 13, 23 };
            case 15:
                return new int[] { 11, 16 };
            case 16:
                return new int[] { 15, 17, 19 };
            case 17:
                return new int[] { 12, 16 };
            case 18:
                return new int[] { 10, 19 };
            case 19:
                return new int[] { 16, 18, 20, 22 };
            case 20:
                return new int[] { 13, 19 };
            case 21:
                return new int[] { 9, 22 };
            case 22:
                return new int[] { 19, 21, 23 };
            case 23:
                return new int[] { 14, 22 };
            default:
                return new int[] { 0 };
        }
    }


    private void MoveChesspiece(int fromValue, int toValue, List<int> adjacentSlots)
    {
        foreach (int value in adjacentSlots)
        {
            slots[value].setPiece("Empty");
            if (toValue == value)
            {
                if (currentPlayer == CurrentPlayer.White)
                {
                    slots[toValue].setPiece("White");
                    slots[fromValue].setPiece("Empty");
                    SwitchPlayer();
                }
                else if (currentPlayer == CurrentPlayer.Black)
                {
                    slots[toValue].setPiece("Black");
                    slots[fromValue].setPiece("Empty");
                    SwitchPlayer();
                }
            }
        }
    }

    private bool Check3InARow(CurrentPlayer currentPlayer)
    {
        return true;
    }

    private void SwitchPlayer()
    {
        if (currentPlayer == CurrentPlayer.White)
        {
            currentPlayer = CurrentPlayer.Black;
        }
        else if (currentPlayer == CurrentPlayer.Black)
        {
            currentPlayer = CurrentPlayer.White;
        }
    }

    private Player ReturnCurrentPlayer(CurrentPlayer currentPlayer)
    {
        if (currentPlayer == CurrentPlayer.White)
        {
            return whitePlayer;
        }
        else if (currentPlayer == CurrentPlayer.Black)
        {
            return blackPlayer;
        }
        else
            return null;
    }
}
