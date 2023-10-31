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

    } //Tra lai true neu slot trong, nguoc lai tra ve false

    private void PlaceChesspiece(int slotValue)
    {
        if (currentPlayer == CurrentPlayer.White) //Neu nhu nguoi choi hien tai la trang thi dat chesspiece trang vao slot do
        {
            slots[slotValue].setPiece("White");
            whitePlayer.pieceSleep--;
            whitePlayer.pieceLive++;
            CheckMill(slotValue, true);
            SwitchPlayer(); //Dat chesspiece xong thi doi luot choi
        }
        else if (currentPlayer == CurrentPlayer.Black) //Neu nhu nguoi choi hien tai la den thi dat chesspiece den vao slot do
        {
            slots[slotValue].setPiece("Black");
            blackPlayer.pieceSleep--;
            blackPlayer.pieceLive++;
            CheckMill(slotValue, false);
            SwitchPlayer(); //Dat chesspiece xong thi doi luot choi
        }
    } //Dat chesspiece len ban`


    private void CheckChesspieceAndShowAdjacent(int slotValue) //Hien len cac o ben canh chesspiece cua nguoi choi de di chuyen
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
                    CheckMill(toValue, true);
                    SwitchPlayer();
                }
                else if (currentPlayer == CurrentPlayer.Black)
                {
                    slots[toValue].setPiece("Black");
                    slots[fromValue].setPiece("Empty");
                    CheckMill(toValue, false);
                    SwitchPlayer();
                }
            }
        }
    } //Di chuyen chesspiece den o trong ben canh

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
    } //Tra ve cac slot ben canh slotValue

    private void CheckMill(int slotValue, bool isWhite) //Tra ve cac slot tao thanh Mill, cho truoc slotValue
    {
        void SettingMillWhite(int current, int prev, int next)
        {
            if (slots[prev].isWhite && !slots[prev].isEmpty)
            {
                if (slots[next].isWhite && !slots[next].isEmpty)
                {
                    print("true, white");
                    slots[current].setPiece("MillWhite");
                    slots[prev].setPiece("MillWhite");
                    slots[next].setPiece("MillWhite");
                }
            }
        }
        void SettingMillBlack(int current, int prev, int next)
        {
            if (!slots[prev].isWhite && !slots[prev].isEmpty)
            {
                if (!slots[next].isWhite && !slots[next].isEmpty)
                {
                    print("true, black");
                    slots[current].setPiece("MillBlack");
                    slots[prev].setPiece("MillBlack");
                    slots[next].setPiece("MillBlack");
                }
            }
        }

        if (isWhite) //Mau trang
        {
            switch (slotValue)
            {
                case 0:
                    SettingMillWhite(0, 1, 2); //Tai day se ta ve mot array neu no tao thanh mot Mill
                    SettingMillWhite(0, 9, 21);
                    break;
                case 1:
                    SettingMillWhite(1, 0, 2);
                    SettingMillWhite(1, 4, 7);
                    break;
                case 2:
                    SettingMillWhite(2, 0, 1);
                    SettingMillWhite(2, 14, 23);
                    break;
                case 3:
                    SettingMillWhite(3, 4, 5);
                    SettingMillWhite(3, 10, 18);
                    break;
                case 4:
                    SettingMillWhite(4, 3, 5);
                    SettingMillWhite(4, 1, 7);
                    break;
                case 5:
                    SettingMillWhite(5, 4, 3);
                    SettingMillWhite(5, 13, 20);
                    break;
                case 6:
                    SettingMillWhite(6, 7, 8);
                    SettingMillWhite(6, 11, 15);
                    break;
                case 7:
                    SettingMillWhite(7, 6, 8);
                    SettingMillWhite(7, 4, 1);
                    break;
                case 8:
                    SettingMillWhite(8, 7, 6);
                    SettingMillWhite(8, 12, 17);
                    break;
                case 9:
                    SettingMillWhite(9, 10, 11);
                    SettingMillWhite(9, 0, 21);
                    break;
                case 10:
                    SettingMillWhite(10, 9, 11);
                    SettingMillWhite(10, 3, 18);
                    break;
                case 11:
                    SettingMillWhite(11, 10, 9);
                    SettingMillWhite(11, 6, 15);
                    break;
                case 12:
                    SettingMillWhite(12, 13, 14);
                    SettingMillWhite(12, 8, 17);
                    break;
                case 13:
                    SettingMillWhite(13, 12, 14);
                    SettingMillWhite(13, 5, 20);
                    break;
                case 14:
                    SettingMillWhite(14, 13, 12);
                    SettingMillWhite(14, 2, 23);
                    break;
                case 15:
                    SettingMillWhite(15, 16, 17);
                    SettingMillWhite(15, 11, 6);
                    break;
                case 16:
                    SettingMillWhite(16, 15, 17);
                    SettingMillWhite(16, 19, 22);
                    break;
                case 17:
                    SettingMillWhite(17, 16, 15);
                    SettingMillWhite(17, 12, 8);
                    break;
                case 18:
                    SettingMillWhite(18, 19, 20);
                    SettingMillWhite(18, 3, 10);
                    break;
                case 19:
                    SettingMillWhite(19, 18, 20);
                    SettingMillWhite(19, 16, 22);
                    break;
                case 20:
                    SettingMillWhite(20, 19, 18);
                    SettingMillWhite(20, 13, 5);
                    break;
                case 21:
                    SettingMillWhite(21, 22, 23);
                    SettingMillWhite(21, 9, 0);
                    break;
                case 22:
                    SettingMillWhite(22, 21, 23);
                    SettingMillWhite(22, 19, 16);
                    break;
                case 23:
                    SettingMillWhite(23, 22, 21);
                    SettingMillWhite(23, 14, 2);
                    break;
                default:
                    break;
            }
        } //Neu chesspiece mau trang
        else
        {
            switch (slotValue)
            {
                case 0:
                    SettingMillBlack(0, 1, 2); //Tai day se ta ve mot array neu no tao thanh mot Mill
                    SettingMillBlack(0, 9, 21);
                    break;
                case 1:
                    SettingMillBlack(1, 0, 2);
                    SettingMillBlack(1, 4, 7);
                    break;
                case 2:
                    SettingMillBlack(2, 0, 1);
                    SettingMillBlack(2, 14, 23);
                    break;
                case 3:
                    SettingMillBlack(3, 4, 5);
                    SettingMillBlack(3, 10, 18);
                    break;
                case 4:
                    SettingMillBlack(4, 3, 5);
                    SettingMillBlack(4, 1, 7);
                    break;
                case 5:
                    SettingMillBlack(5, 4, 3);
                    SettingMillBlack(5, 13, 20);
                    break;
                case 6:
                    SettingMillBlack(6, 7, 8);
                    SettingMillBlack(6, 11, 15);
                    break;
                case 7:
                    SettingMillBlack(7, 6, 8);
                    SettingMillBlack(7, 4, 1);
                    break;
                case 8:
                    SettingMillBlack(8, 7, 6);
                    SettingMillBlack(8, 12, 17);
                    break;
                case 9:
                    SettingMillBlack(9, 10, 11);
                    SettingMillBlack(9, 0, 21);
                    break;
                case 10:
                    SettingMillBlack(10, 9, 11);
                    SettingMillBlack(10, 3, 18);
                    break;
                case 11:
                    SettingMillBlack(11, 10, 9);
                    SettingMillBlack(11, 6, 15);
                    break;
                case 12:
                    SettingMillBlack(12, 13, 14);
                    SettingMillBlack(12, 8, 17);
                    break;
                case 13:
                    SettingMillBlack(13, 12, 14);
                    SettingMillBlack(13, 5, 20);
                    break;
                case 14:
                    SettingMillBlack(14, 13, 12);
                    SettingMillBlack(14, 2, 23);
                    break;
                case 15:
                    SettingMillBlack(15, 16, 17);
                    SettingMillBlack(15, 11, 6);
                    break;
                case 16:
                    SettingMillBlack(16, 15, 17);
                    SettingMillBlack(16, 19, 22);
                    break;
                case 17:
                    SettingMillBlack(17, 16, 15);
                    SettingMillBlack(17, 12, 8);
                    break;
                case 18:
                    SettingMillBlack(18, 19, 20);
                    SettingMillBlack(18, 3, 10);
                    break;
                case 19:
                    SettingMillBlack(19, 18, 20);
                    SettingMillBlack(19, 16, 22);
                    break;
                case 20:
                    SettingMillBlack(20, 19, 18);
                    SettingMillBlack(20, 13, 5);
                    break;
                case 21:
                    SettingMillBlack(21, 22, 23);
                    SettingMillBlack(21, 9, 0);
                    break;
                case 22:
                    SettingMillBlack(22, 21, 23);
                    SettingMillBlack(22, 19, 16);
                    break;
                case 23:
                    SettingMillBlack(23, 22, 21);
                    SettingMillBlack(23, 14, 2);
                    break;
                default:
                    break;
            }
        }
    }
}
