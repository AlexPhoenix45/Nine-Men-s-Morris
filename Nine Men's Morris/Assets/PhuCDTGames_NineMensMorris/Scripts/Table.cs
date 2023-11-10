using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public static Table Instance;
    public enum CurrentPlayer
    { 
        White,
        Black
    }

    public Slots[] slots;
    public GameObject[] grids;
    public CurrentPlayer currentPlayer = CurrentPlayer.White;
    public Player whitePlayer;
    public Player blackPlayer;

    //This part is for moving a chesspiece and Jump move
    public bool selectToMove = false;
    public List<int> adjacentSlots = new List<int>();
    public int lastestSlot;
    public List<int> emptySlots = new List<int>(); //List of all empty slots
    public bool isJumpMove;

    //This part is for removing a chesspiece
    public bool removingMove = false;

    //Thí part is for bot
    public bool isBotPlaying;

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

        whitePlayer.MyTurn(true);
        blackPlayer.MyTurn(false);
    }
    
    public IEnumerator Evaluate(int slotValue)
    {
        yield return new WaitForSeconds(0.2f);
        if (selectToMove)
        {
            if (!isJumpMove)
            {
                MoveChesspiece(lastestSlot, slotValue, adjacentSlots);
                adjacentSlots.Clear();
                selectToMove = false;
            }
            else
            {
                MoveChesspiece(lastestSlot, slotValue, emptySlots);
                emptySlots.Clear();
                selectToMove = false;
            }
        }
        else if (removingMove)
        {
            if (currentPlayer == CurrentPlayer.White)
            {
                RemovingChesspiece(slotValue, true);
            }
            else
            {
                RemovingChesspiece(slotValue, false);
            }
        }
        else
        {
            //Khi click vao 1 slot
            //Check xem slot do con trong hay khong, neu co thi dat chesspiece vao do, dieu kien la chua dat het 9 chesspiece
            if (CheckIfSlotIsEmpty(slotValue) && ReturnCurrentPlayer(currentPlayer).pieceSleep != 0)
            {
                PlaceChesspiece(slotValue); //Khi player da dat du so chesspiece cua minh thi se khong chay vao day nua
            }
            else if (!CheckIfSlotIsEmpty(slotValue) && ReturnCurrentPlayer(currentPlayer).pieceSleep == 0 && ReturnCurrentPlayer(currentPlayer).pieceLive > 3) //Phai dat het 9 chesspiece moi duoc di chuyen nhung chesspiece tren table
            {
                //Neu slot do khong trong, xem player no an dung vao chesspiece cua minh hay khong, neu an dung thi hien ra nhung nuoc di co the di cua chesspiece do
                CheckChesspieceAndShowAdjacent(slotValue, false);
            }
            else if (!CheckIfSlotIsEmpty(slotValue) && ReturnCurrentPlayer(currentPlayer).pieceSleep == 0 && ReturnCurrentPlayer(currentPlayer).pieceLive == 3) //Khi con` lai 3 chesspiece, nguoi choi duoc thuc hien jump move
            {
                CheckChesspieceAndShowAdjacent(slotValue, true);
                //Jump Move
            }
        }
    }
    private bool CheckIfSlotIsEmpty(int slotValue)
    {
        if (slots[slotValue].isEmpty) //Neu slot nay trong
        {
            return true;
        }
        else //Neu slot nay khong trong
        {
            return false;
        }

    } //Tra lai true neu slot trong, nguoc lai tra ve false

    private void PlaceChesspiece(int slotValue)
    {
        if (currentPlayer == CurrentPlayer.White) //Neu nhu nguoi choi hien tai la trang thi dat chesspiece trang vao slot do
        {
            slots[slotValue].setPiece("White");
            whitePlayer.ChangePieceAmount("pieceSleep", -1);
            whitePlayer.ChangePieceAmount("pieceLive", +1);
            CheckRemainingMill(slotValue, true); //Tai day check xem da co nhung Mill nao`
            if (CheckNewMillCreated(slotValue, true) == "True, White") //Tai day check xem lieu chesspiece vua dat xuong co tao thanh Mill nao` khong
            {
                print("White can remove a chess piece from Black");
                removingMove = true;
            }
            else
            {
                SwitchPlayer(); //Dat chesspiece xong thi doi luot choi
            }
        }
        else if (currentPlayer == CurrentPlayer.Black) //Neu nhu nguoi choi hien tai la den thi dat chesspiece den vao slot do
        {
            slots[slotValue].setPiece("Black");
            blackPlayer.ChangePieceAmount("pieceSleep", -1);
            blackPlayer.ChangePieceAmount("pieceLive", +1);
            CheckRemainingMill(slotValue, false); //Tai day check xem da co nhung Mill nao`
            if (CheckNewMillCreated(slotValue, false) == "True, Black") //Tai day check xem lieu chesspiece vua dat xuong co tao thanh Mill nao` khong
            {
                print("Black can remove a chess piece from White");
                removingMove = true;
                Bot.Instance.CallBot();
            }
            else
            {
                SwitchPlayer(); //Dat chesspiece xong thi doi luot choi
            }
        }
    } //Dat chesspiece len ban`

    private void CheckChesspieceAndShowAdjacent(int slotValue, bool jumpMove) //Hien len cac o ben canh chesspiece cua nguoi choi de di chuyen
    {
        if (currentPlayer == CurrentPlayer.White) 
        {
            if (slots[slotValue].isWhite && !jumpMove) //Neu nguoi choi mau trang va an vao chesspiece mau trang va day khong phai jump move
            {
                foreach (int value in CheckAdjacent(slotValue))
                {
                    if (slots[value].isEmpty)
                    {
                        slots[value].setPiece("Marker"); //Hien ra nhung o con trong ben canh o cua nguoi choi 
                        adjacentSlots.Add(value);
                        lastestSlot = slotValue;
                        selectToMove = true;
                        isJumpMove = false;
                    }
                }
            }
            else if (slots[slotValue].isWhite && jumpMove) //Neu nguoi choi mau trang va an vao chesspiece mau trang va day la jump move
            {
                foreach (int value in CheckAllEmpty())
                {
                    if (slots[value].isEmpty)
                    {
                        slots[value].setPiece("Marker"); //Hien ra nhung o con trong ben canh o cua nguoi choi 
                        emptySlots.Add(value);
                        lastestSlot = slotValue;
                        selectToMove = true;
                        isJumpMove = true;
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
            if (!slots[slotValue].isWhite && !jumpMove) //Neu nguoi choi mau den va an vao chesspiece mau den
            {
                foreach (int value in CheckAdjacent(slotValue))
                {
                    if (slots[value].isEmpty)
                    {
                        slots[value].setPiece("Marker"); //Hien ra nhung o con trong ben canh o cua nguoi choi 
                        adjacentSlots.Add(value);
                        lastestSlot = slotValue;
                        selectToMove = true;
                        isJumpMove = false;
                    }
                }
            }
            else if (!slots[slotValue].isWhite && jumpMove) //Neu nguoi choi mau trang va an vao chesspiece mau trang va day la jump move
            {
                foreach (int value in CheckAllEmpty())
                {
                    if (slots[value].isEmpty)
                    {
                        slots[value].setPiece("Marker"); //Hien ra nhung o con trong ben canh o cua nguoi choi 
                        emptySlots.Add(value);
                        lastestSlot = slotValue;
                        selectToMove = true;
                        isJumpMove = true;
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
                    slots[fromValue].setPiece("Empty");
                    UnCheckMill(fromValue, true);  //Neu chesspiece o vi tri cu dang tao thanh` 1 Mill, thi` se bo Mill do di
                    slots[toValue].setPiece("White");

                    for (int i = 0; i < 24; i++) //Xet xem lieu trong 3 chesspiece o tren, co chesspiece nao` thuoc ` Mill khac khong
                    {
                        if (slots[i].state == "MillWhite")
                        {
                            CheckRemainingMill(i, true);
                        }
                    }

                    if (CheckNewMillCreated(toValue, true) == "True, White") //Check xem chesspiece o vi tri moi co tao thanh Mill nao` khong
                    {
                        print("White can remove a chess piece from Black");
                        removingMove = true;
                    }
                    else
                    {
                        SwitchPlayer();
                    }
                }
                else if (currentPlayer == CurrentPlayer.Black)
                {
                    slots[fromValue].setPiece("Empty");
                    UnCheckMill(fromValue, false); //Neu chesspiece o vi tri cu dang tao thanh` 1 Mill, thi` se bo Mill do di
                    slots[toValue].setPiece("Black"); 

                    for (int i = 0; i < 24; i++) //Xet xem lieu trong 3 chesspiece o tren, co chesspiece nao` thuoc ` Mill khac khong
                    {
                        if (slots[i].state == "MillBlack")
                        {
                            CheckRemainingMill(i, false);
                        }
                    }
                    if (CheckNewMillCreated(toValue, false) == "True, Black") //Check xem chesspiece o vi tri moi co tao thanh Mill nao` khong
                    {
                        print("Black can remove a chess piece from White");
                        removingMove = true;
                        Bot.Instance.CallBot();
                    }
                    else
                    {
                        SwitchPlayer();
                    }
                }
            }
        }
    } //Di chuyen chesspiece den o trong ben canh

    private void SwitchPlayer()
    {
        if (currentPlayer == CurrentPlayer.White)
        {
            whitePlayer.MyTurn(false);
            blackPlayer.MyTurn(true);
            currentPlayer = CurrentPlayer.Black;
            if (isBotPlaying)
            {
                Bot.Instance.CallBot();
            }
        }
        else if (currentPlayer == CurrentPlayer.Black)
        {
            whitePlayer.MyTurn(true);
            blackPlayer.MyTurn(false);
            currentPlayer = CurrentPlayer.White;
        }

        CheckForWinning(currentPlayer);
        CheckIfSuffocated(currentPlayer);

        if (ReturnCurrentPlayer(currentPlayer).pieceSleep == 0)
        {
            for (int i = 0; i < 24; i++)
            {
                if (slots[i].isEmpty)
                {
                    slots[i].setPiece("Empty");
                }
            }
        }
    }

    private void CheckForWinning(CurrentPlayer current)
    {
        if (ReturnCurrentPlayer(current).pieceDie == 7)
        {
            if (current == CurrentPlayer.White)
            {
                UIController.Instance.SetEndgameText("BLACK WIN!");
            }
            else
            {
                UIController.Instance.SetEndgameText("WHITE WIN!");
            }
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

    private List<int> CheckAllEmpty()
    {
        List<int> empty = new List<int>();
        empty.Clear();
        for (int i = 0; i < 24; i++)
        {
            if (slots[i].isEmpty)
            {
                empty.Add(i);
            }
        }
        return empty;
    }

    private void CheckRemainingMill(int slotValue, bool isWhite) //Tra ve cac slot tao thanh Mill, cho truoc slotValue (tai day se ra` soat' nhung Mill cu~)
    {
        void SettingMillWhite(int current, int prev, int next)
        {
            if (slots[prev].isWhite && !slots[prev].isEmpty)
            {
                if (slots[next].isWhite && !slots[next].isEmpty)
                {
                    slots[current].setPiece("MillWhite");
                    slots[prev].setPiece("MillWhite");
                    slots[next].setPiece("MillWhite");
                    SetGrid(true, current, prev, next);
                }
            }
        }
        void SettingMillBlack(int current, int prev, int next)
        {
            if (!slots[prev].isWhite && !slots[prev].isEmpty)
            {
                if (!slots[next].isWhite && !slots[next].isEmpty)
                {
                    slots[current].setPiece("MillBlack");
                    slots[prev].setPiece("MillBlack");
                    slots[next].setPiece("MillBlack");
                    SetGrid(true, current, prev, next);
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

    private void UnCheckMill(int slotValue, bool isWhite) //Ham nay dung de xoa di nhung mill cu sau khi di chuyen
    {
        void UnMillWhite(int current, int prev, int next)
        {
            if (slots[prev].isWhite && !slots[prev].isEmpty)
            {
                if (slots[next].isWhite && !slots[next].isEmpty)
                {
                    slots[current].setPiece("Empty");
                    slots[prev].setPiece("White");
                    slots[next].setPiece("White");
                    SetGrid(false, current, prev, next);
                }
            }
        }
        void UnMillBlack(int current, int prev, int next)
        {
            if (!slots[prev].isWhite && !slots[prev].isEmpty)
            {
                if (!slots[next].isWhite && !slots[next].isEmpty)
                {
                    slots[current].setPiece("Empty");
                    slots[prev].setPiece("Black");
                    slots[next].setPiece("Black");
                    SetGrid(false, current, prev, next);
                }
            }
        }

        if (isWhite) //Mau trang
        {
            switch (slotValue)
            {
                case 0:
                    UnMillWhite(0, 1, 2); 
                    UnMillWhite(0, 9, 21);
                    break;
                case 1:
                    UnMillWhite(1, 0, 2);
                    UnMillWhite(1, 4, 7);
                    break;
                case 2:
                    UnMillWhite(2, 0, 1);
                    UnMillWhite(2, 14, 23);
                    break;
                case 3:
                    UnMillWhite(3, 4, 5);
                    UnMillWhite(3, 10, 18);
                    break;
                case 4:
                    UnMillWhite(4, 3, 5);
                    UnMillWhite(4, 1, 7);
                    break;
                case 5:
                    UnMillWhite(5, 4, 3);
                    UnMillWhite(5, 13, 20);
                    break;
                case 6:
                    UnMillWhite(6, 7, 8);
                    UnMillWhite(6, 11, 15);
                    break;
                case 7:
                    UnMillWhite(7, 6, 8);
                    UnMillWhite(7, 4, 1);
                    break;
                case 8:
                    UnMillWhite(8, 7, 6);
                    UnMillWhite(8, 12, 17);
                    break;
                case 9:
                    UnMillWhite(9, 10, 11);
                    UnMillWhite(9, 0, 21);
                    break;
                case 10:
                    UnMillWhite(10, 9, 11);
                    UnMillWhite(10, 3, 18);
                    break;
                case 11:
                    UnMillWhite(11, 10, 9);
                    UnMillWhite(11, 6, 15);
                    break;
                case 12:
                    UnMillWhite(12, 13, 14);
                    UnMillWhite(12, 8, 17);
                    break;
                case 13:
                    UnMillWhite(13, 12, 14);
                    UnMillWhite(13, 5, 20);
                    break;
                case 14:
                    UnMillWhite(14, 13, 12);
                    UnMillWhite(14, 2, 23);
                    break;
                case 15:
                    UnMillWhite(15, 16, 17);
                    UnMillWhite(15, 11, 6);
                    break;
                case 16:
                    UnMillWhite(16, 15, 17);
                    UnMillWhite(16, 19, 22);
                    break;
                case 17:
                    UnMillWhite(17, 16, 15);
                    UnMillWhite(17, 12, 8);
                    break;
                case 18:
                    UnMillWhite(18, 19, 20);
                    UnMillWhite(18, 3, 10);
                    break;
                case 19:
                    UnMillWhite(19, 18, 20);
                    UnMillWhite(19, 16, 22);
                    break;
                case 20:
                    UnMillWhite(20, 19, 18);
                    UnMillWhite(20, 13, 5);
                    break;
                case 21:
                    UnMillWhite(21, 22, 23);
                    UnMillWhite(21, 9, 0);
                    break;
                case 22:
                    UnMillWhite(22, 21, 23);
                    UnMillWhite(22, 19, 16);
                    break;
                case 23:
                    UnMillWhite(23, 22, 21);
                    UnMillWhite(23, 14, 2);
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
                    UnMillBlack(0, 1, 2); //Tai day se ta ve mot array neu no tao thanh mot Mill
                    UnMillBlack(0, 9, 21);
                    break;
                case 1:
                    UnMillBlack(1, 0, 2);
                    UnMillBlack(1, 4, 7);
                    break;
                case 2:
                    UnMillBlack(2, 0, 1);
                    UnMillBlack(2, 14, 23);
                    break;
                case 3:
                    UnMillBlack(3, 4, 5);
                    UnMillBlack(3, 10, 18);
                    break;
                case 4:
                    UnMillBlack(4, 3, 5);
                    UnMillBlack(4, 1, 7);
                    break;
                case 5:
                    UnMillBlack(5, 4, 3);
                    UnMillBlack(5, 13, 20);
                    break;
                case 6:
                    UnMillBlack(6, 7, 8);
                    UnMillBlack(6, 11, 15);
                    break;
                case 7:
                    UnMillBlack(7, 6, 8);
                    UnMillBlack(7, 4, 1);
                    break;
                case 8:
                    UnMillBlack(8, 7, 6);
                    UnMillBlack(8, 12, 17);
                    break;
                case 9:
                    UnMillBlack(9, 10, 11);
                    UnMillBlack(9, 0, 21);
                    break;
                case 10:
                    UnMillBlack(10, 9, 11);
                    UnMillBlack(10, 3, 18);
                    break;
                case 11:
                    UnMillBlack(11, 10, 9);
                    UnMillBlack(11, 6, 15);
                    break;
                case 12:
                    UnMillBlack(12, 13, 14);
                    UnMillBlack(12, 8, 17);
                    break;
                case 13:
                    UnMillBlack(13, 12, 14);
                    UnMillBlack(13, 5, 20);
                    break;
                case 14:
                    UnMillBlack(14, 13, 12);
                    UnMillBlack(14, 2, 23);
                    break;
                case 15:
                    UnMillBlack(15, 16, 17);
                    UnMillBlack(15, 11, 6);
                    break;
                case 16:
                    UnMillBlack(16, 15, 17);
                    UnMillBlack(16, 19, 22);
                    break;
                case 17:
                    UnMillBlack(17, 16, 15);
                    UnMillBlack(17, 12, 8);
                    break;
                case 18:
                    UnMillBlack(18, 19, 20);
                    UnMillBlack(18, 3, 10);
                    break;
                case 19:
                    UnMillBlack(19, 18, 20);
                    UnMillBlack(19, 16, 22);
                    break;
                case 20:
                    UnMillBlack(20, 19, 18);
                    UnMillBlack(20, 13, 5);
                    break;
                case 21:
                    UnMillBlack(21, 22, 23);
                    UnMillBlack(21, 9, 0);
                    break;
                case 22:
                    UnMillBlack(22, 21, 23);
                    UnMillBlack(22, 19, 16);
                    break;
                case 23:
                    UnMillBlack(23, 22, 21);
                    UnMillBlack(23, 14, 2);
                    break;
                default:
                    break;
            }
        }
    }

    private string CheckNewMillCreated(int slotValue, bool isWhite) //Tra ve cac slot tao thanh Mill, cho truoc slotValue (tai day se ra` soat' nhung Mill moi duoc tao)
    {
        string returnMessage = "";
        void SettingMillWhite(int current, int prev, int next)
        {
            if (slots[prev].isWhite && !slots[prev].isEmpty)
            {
                if (slots[next].isWhite && !slots[next].isEmpty)
                {
                    returnMessage = "True, White";
                    slots[current].setPiece("MillWhite");
                    slots[prev].setPiece("MillWhite");
                    slots[next].setPiece("MillWhite");
                    SetGrid(true, current, prev, next);
                }
            }
        }
        void SettingMillBlack(int current, int prev, int next)
        {
            if (!slots[prev].isWhite && !slots[prev].isEmpty)
            {
                if (!slots[next].isWhite && !slots[next].isEmpty)
                {
                    returnMessage = "True, Black";
                    slots[current].setPiece("MillBlack");
                    slots[prev].setPiece("MillBlack");
                    slots[next].setPiece("MillBlack");
                    SetGrid(true, current, prev, next);
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

        return returnMessage;
    }

    private void RemovingChesspiece(int slotValue, bool isWhite)
    {
        bool IfEveryPieceIsMilled(bool isWhite)
        {
            if (isWhite) //Chesspiece mau Trang
            {
                for (int i = 0; i < 24; i++)
                {
                    if (!slots[i].isEmpty)
                    {
                        if (slots[i].isWhite && !slots[i].isMilled)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else //Chesspiece mau den
            {
                for (int i = 0; i < 24; i++)
                {
                    if (!slots[i].isEmpty)
                    {
                        if (!slots[i].isWhite && !slots[i].isMilled)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        if (isWhite) //Day la luot cua White
        {
            if (!IfEveryPieceIsMilled(false)) //Neu nhu tat ca cac chesspiece cua doi thu deu Milled
            {
                if (!slots[slotValue].isWhite && !slots[slotValue].isMilled && !slots[slotValue].isEmpty)
                {
                    blackPlayer.ChangePieceAmount("pieceDie", +1);
                    blackPlayer.ChangePieceAmount("pieceLive", -1);
                    slots[slotValue].setPiece("Empty");
                    UnCheckMill(slotValue, false); //Neu chesspiece o vi tri cu dang tao thanh` 1 Mill, thi` se bo Mill do di

                    for (int i = 0; i < 24; i++) //Xet xem lieu trong 3 chesspiece o tren, co chesspiece nao` thuoc ` Mill khac khong
                    {
                        if (slots[i].state == "MillBlack")
                        {
                            CheckRemainingMill(i, false);
                        }
                    }
                    removingMove = false;
                    SwitchPlayer();
                }
            }
            else
            {
                if (!slots[slotValue].isWhite && slots[slotValue].isMilled && !slots[slotValue].isEmpty)
                {
                    blackPlayer.ChangePieceAmount("pieceDie", +1);
                    blackPlayer.ChangePieceAmount("pieceLive", -1);
                    slots[slotValue].setPiece("Empty");
                    UnCheckMill(slotValue, false); //Neu chesspiece o vi tri cu dang tao thanh` 1 Mill, thi` se bo Mill do di

                    for (int i = 0; i < 24; i++) //Xet xem lieu trong 3 chesspiece o tren, co chesspiece nao` thuoc ` Mill khac khong
                    {
                        if (slots[i].state == "MillBlack")
                        {
                            CheckRemainingMill(i, false);
                        }
                    }
                    removingMove = false;
                    SwitchPlayer();
                }
            }
        }
        else //Day la luot cua Black
        {
            if (!IfEveryPieceIsMilled(true))
            {
                if (slots[slotValue].isWhite && !slots[slotValue].isMilled && !slots[slotValue].isEmpty)
                {
                    whitePlayer.ChangePieceAmount("pieceDie", +1);
                    whitePlayer.ChangePieceAmount("pieceLive", -1);
                    slots[slotValue].setPiece("Empty");
                    UnCheckMill(slotValue, true); //Neu chesspiece o vi tri cu dang tao thanh` 1 Mill, thi` se bo Mill do di

                    for (int i = 0; i < 24; i++) //Xet xem lieu trong 3 chesspiece o tren, co chesspiece nao` thuoc ` Mill khac khong
                    {
                        if (slots[i].state == "MillWhite")
                        {
                            CheckRemainingMill(i, true);
                        }
                    }
                    removingMove = false;
                    SwitchPlayer();
                }
            }
            else
            {
                if (slots[slotValue].isWhite && slots[slotValue].isMilled && !slots[slotValue].isEmpty)
                {
                    whitePlayer.ChangePieceAmount("pieceDie", +1);
                    whitePlayer.ChangePieceAmount("pieceLive", -1);
                    slots[slotValue].setPiece("Empty");
                    UnCheckMill(slotValue, true); //Neu chesspiece o vi tri cu dang tao thanh` 1 Mill, thi` se bo Mill do di

                    for (int i = 0; i < 24; i++) //Xet xem lieu trong 3 chesspiece o tren, co chesspiece nao` thuoc ` Mill khac khong
                    {
                        if (slots[i].state == "MillWhite")
                        {
                            CheckRemainingMill(i, true);
                        }
                    }
                    removingMove = false;
                    SwitchPlayer();
                }
            }
        }
    }

    private void SetGrid(bool value, int a, int b, int c)
    {
        print("CALL SET GRID: " + a + "-" + b + "-" + c + value);
        if ((a == 0 || a == 1 || a == 2) && a != b && a != c)
        {
            if ((b == 0 || b == 1 || b == 2) && b != a && b != c)
            {
                if ((c == 0 || c == 1 || c == 2) && c != a && c != b)
                {
                    grids[0].SetActive(value);
                }
            }
        }
        
        if ((a == 3 || a == 4 || a == 5) && a != b && a != c)
        {
            if ((b == 3 || b == 4 || b == 5) && b != a && b != c)
            {
                if ((c == 3 || c == 4 || c == 5) && c != a && c != b)
                {
                    grids[1].SetActive(value);
                }
            }
        }
        
        if ((a == 6 || a == 7 || a == 8) && a != b && a != c)
        {
            if ((b == 6 || b == 7 || b == 8) && b != a && b != c)
            {
                if ((c == 6 || c == 7 || c == 8) && c != a && c != b)
                {
                    grids[2].SetActive(value);
                }
            }
        }
        
        if ((a == 15 || a == 16 || a == 17) && a != b && a != c)
        {
            if ((b == 15 || b == 16 || b == 17) && b != a && b != c)
            {
                if ((c == 15 || c == 16 || c == 17) && c != a && c != b)
                {
                    grids[3].SetActive(value);
                }
            }
        }
        
        if ((a == 18 || a == 19 || a == 20) && a != b && a != c)
        {
            if ((b == 18 || b == 19 || b == 20) && b != a && b != c)
            {
                if ((c == 18 || c == 19 || c == 20) && c != a && c != b)
                {
                    grids[4].SetActive(value);
                }
            }
        }
        
        if ((a == 21 || a == 22 || a == 23) && a != b && a != c)
        {
            if ((b == 21 || b == 22 || b == 23) && b != a && b != c)
            {
                if ((c == 21 || c == 22 || c == 23) && c != a && c != b)
                {
                    grids[5].SetActive(value);
                }
            }
        }
        
        if ((a == 0 || a == 9 || a == 21) && a != b && a != c)
        {
            if ((b == 0 || b == 9 || b == 21) && b != a && b != c)
            {
                if ((c == 0 || c == 9 || c == 21) && c != a && c != b)
                {
                    grids[6].SetActive(value);
                }
            }
        }
        
        if ((a == 3 || a == 10 || a == 18) && a != b && a != c)
        {
            if ((b == 3 || b == 10 || b == 18) && b != a && b != c)
            {
                if ((c == 3 || c == 10 || c == 18) && c != a && c != b)
                {
                    grids[7].SetActive(value);
                }
            }
        }
        
        if ((a == 6 || a == 11 || a == 15) && a != b && a != c)
        {
            if ((b == 6 || b == 11 || b == 15) && b != a && b != c)
            {
                if ((c == 6 || c == 11 || c == 15) && c != a && c != b)
                {
                    grids[8].SetActive(value);
                }
            }
        }
        
        if ((a == 8 || a == 12 || a == 17) && a != b && a != c)
        {
            if ((b == 8 || b == 12 || b == 17) && b != a && b != c)
            {
                if ((c == 8 || c == 12 || c == 17) && c != a && c != b)
                {
                    grids[9].SetActive(value);
                }
            }
        }
        
        if ((a == 5 || a == 13 || a == 20) && a != b && a != c)
        {
            if ((b == 5 || b == 13 || b == 20) && b != a && b != c)
            {
                if ((c == 5 || c == 13 || c == 20) && c != a && c != b)
                {
                    grids[10].SetActive(value);
                }
            }
        }
        
        if ((a == 2 || a == 14 || a == 23) && a != b && a != c)
        {
            if ((b == 2 || b == 14 || b == 23) && b != a && b != c)
            {
                if ((c == 2 || c == 14 || c == 23) && c != a && c != b)
                {
                    print(value);
                    grids[11].SetActive(value);
                }
            }
        }
        
        if ((a == 1 || a == 4 || a == 7) && a != b && a != c)
        {
            if ((b == 1 || b == 4 || b == 7) && b != a && b != c)
            {
                if ((c == 1 || c == 4 || c == 7) && c != a && c != b)
                {
                    grids[12].SetActive(value);
                }
            }
        }
        
        if ((a == 16 || a == 19 || a == 22) && a != b && a != c)
        {
            if ((b == 16 || b == 19 || b == 22) && b != a && b != c)
            {
                if ((c == 16 || c == 19 || c == 22) && c != a && c != b)
                {
                    grids[13].SetActive(value);
                }
            }
        }
        
        if ((a == 9 || a == 10 || a == 11) && a != b && a != c)
        {
            if ((b == 9 || b == 10 || b == 11) && b != a && b != c)
            {
                if ((c == 9 || c == 10 || c == 11) && c != a && c != b)
                {
                    grids[14].SetActive(value);
                }
            }
        }
        
        if ((a == 12 || a == 13 || a == 14) && a != b && a != c)
        {
            if ((b == 12 || b == 13 || b == 14) && b != a && b != c)
            {
                if ((c == 12 || c == 13 || c == 14) && c != a && c != b)
                {
                    grids[15].SetActive(value);
                }
            }
        }
    }

    private void CheckIfSuffocated(CurrentPlayer current) //Check xem lieu chesspiece cua nguoi choi co the di chuyen duoc nua hay khong
    {
        int[] CheckAdjacent(int slotValue)
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
        } //Tra ve 1 mang cac slot ben canh slotValue

        List<int> CheckIsEmpty(int[] slotList) //Input la mot mang, tra ve mot List cac slots dang trong
        {
            List<int> emptyAdjacentSlot = new List<int>();

            foreach (int i in slotList)
            {
                if (slots[i].isEmpty)
                {
                    emptyAdjacentSlot.Add(i);
                }
            }

            return emptyAdjacentSlot;
        }


        if (current == CurrentPlayer.White && whitePlayer.pieceSleep == 0) //Day la voi nguoi choi White
        {
            bool isSuffocated = true;
            for (int i = 0; i < 24; i++)
            {
                if (slots[i].isWhite && !slots[i].isEmpty) //Neu la quan co trang
                {
                    if (CheckIsEmpty(CheckAdjacent(i)).Count != 0) //Neu quan co trang do con` cho trong
                    {
                        isSuffocated = false;
                    }
                }
            }
            if (isSuffocated)
            {
                UIController.Instance.SetEndgameText("BLACK WINS!");
            }
        }
        else if (current == CurrentPlayer.Black && blackPlayer.pieceSleep == 0) //Day la voi nguoi choi Black
        {
            bool isSuffocated = true;
            for (int i = 0; i < 24; i++)
            {
                if (!slots[i].isWhite && !slots[i].isEmpty) //Neu la quan co den
                {
                    if (CheckIsEmpty(CheckAdjacent(i)).Count != 0) //Neu quan co den do con` cho trong
                    {
                        isSuffocated = false;
                    }
                }
            }
            if (isSuffocated)
            {
                UIController.Instance.SetEndgameText("WHITE WINS!");
            }
        }
    }
}
