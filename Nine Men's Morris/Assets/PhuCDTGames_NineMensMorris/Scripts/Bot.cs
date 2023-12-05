using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class Bot : MonoBehaviour
{
    public static Bot Instance;
    private Slots[] slots;
    private List<int> possibleMoves = new List<int>();
    private bool oldMove = false; //Day la phuong thuc de di chuyen qua lai 1 chesspiece de tao thanh` Mill
    private int oldFirstMove = -1;
    private int oldSecondMove = -1;

    private bool removeMove = false;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void CallBot()
    {
        //print("has call");
        if (Table.Instance.currentPlayer == Table.CurrentPlayer.Black)
        {
            slots = Table.Instance.slots;
            //------------STARTING-------------
            if (Table.Instance.blackPlayer.pieceSleep > 0 && !Table.Instance.removingMove) //this is for placing, to block player mill and creating bot mill
            {
                StartingGame_PlacingChessPiece();
            }

            if (Table.Instance.removingMove) //Day la phuong thuc de remove chesspiece
            {
                RemovingPiece();
            }

            if (Table.Instance.blackPlayer.pieceSleep != 0 || removeMove) //Day la phuong thuc dat chesspiece cua Staring Game
            {
                Place();
                removeMove = false;
                return;
            }

            //----------MID GAME-------------
            if (Table.Instance.blackPlayer.pieceSleep == 0 && !oldMove && Table.Instance.blackPlayer.pieceLive > 3) //Day la phuong thuc logic cua Mid Game
            {
                MidGameMove_FirstMethod();
            }
            else if (Table.Instance.blackPlayer.pieceSleep == 0 && oldMove && Table.Instance.blackPlayer.pieceLive > 3) //Day la phuong thuc khi phuong thuc dau tien khong thuc hien duoc
            {
                MidGameMove_SecondMethod(oldMove, oldFirstMove, oldSecondMove);
                oldMove = false;
            }
            else //Day la jump move
            {
                JumpMove();
            }
        }
    }

    private void StartingGame_PlacingChessPiece()
    {
        bool isNeedRandomMove = true;
        int blackPieceNeedToPlace = 0;
        bool justCreatedMill = false;
        void CheckPiece(int a, int b, int c, bool isWhite)
        {
            int[] millSlots = { a, b, c };
            if (!isWhite) //For black
            {
                int pieceInRow = 0;
                int pieceNeedToPlace = 0;
                bool skip = false;
                bool hasWhiteInRow = false;
                foreach (int i in millSlots) // day la khi co 2 black piece canh nhau
                {
                    if (!slots[i].isWhite && !slots[i].isEmpty) //If that piece is black, ++ to piece in row
                    {
                        pieceInRow++;
                    }
                    else if (slots[i].isWhite && !slots[i].isEmpty) //But if there is a white piece, don't need to do anything
                    {
                        skip = true;
                        hasWhiteInRow = true;
                    }
                    else
                    {
                        pieceNeedToPlace = i;
                        if (!hasWhiteInRow)
                            blackPieceNeedToPlace = i;
                    }

                }
                if (pieceInRow == 2 && !skip && Table.Instance.currentPlayer == Table.CurrentPlayer.Black && !Table.Instance.removingMove)
                {
                    string message = "";
                    foreach (int i in millSlots)
                    {
                        message += i + " ";
                    }
                    print(message + ": 2 black pieces, need to place at " + pieceNeedToPlace);
                    possibleMoves.Add(pieceNeedToPlace);
                    justCreatedMill = true;
                    isNeedRandomMove = false;
                }
            }
            else if (isWhite && !justCreatedMill)
            {
                int pieceInRow = 0;
                int pieceNeedToPlace = 0;
                bool skip = false;
                foreach (int i in millSlots)
                {
                    if (slots[i].isWhite && !slots[i].isEmpty) //If that piece is white, ++ to piece in row
                    {
                        pieceInRow++;
                    }
                    else if (!slots[i].isWhite && !slots[i].isEmpty) //But if there is a black piece, don't need to do anything
                    {
                        skip = true;
                    }
                    else
                    {
                        pieceNeedToPlace = i;
                    }

                }
                if (pieceInRow == 2 && !skip && Table.Instance.currentPlayer == Table.CurrentPlayer.Black && !Table.Instance.removingMove)
                {
                    string message = "";
                    foreach (int i in millSlots)
                    {
                        message += i + " ";
                    }
                    print(message + ": 2 white pieces, need to place at " + pieceNeedToPlace);
                    possibleMoves.Add(pieceNeedToPlace);
                    isNeedRandomMove = false;
                }
            }

        }

        for (int i = 0; i < 24; i++) //Chay het 24 o de xem o nao` co possible mill, uu tien tao Mill cho Black
        {
            switch (i)
            {
                case 0:
                    CheckPiece(0, 1, 2, false); //Tai day se ta ve mot array neu no tao thanh mot Mill
                    CheckPiece(0, 9, 21, false);
                    break;
                case 1:
                    CheckPiece(1, 0, 2, false);
                    CheckPiece(1, 4, 7, false);
                    break;
                case 2:
                    CheckPiece(2, 0, 1, false);
                    CheckPiece(2, 14, 23, false);
                    break;
                case 3:
                    CheckPiece(3, 4, 5, false);
                    CheckPiece(3, 10, 18, false);
                    break;
                case 4:
                    CheckPiece(4, 3, 5, false);
                    CheckPiece(4, 1, 7, false);
                    break;
                case 5:
                    CheckPiece(5, 4, 3, false);
                    CheckPiece(5, 13, 20, false);
                    break;
                case 6:
                    CheckPiece(6, 7, 8, false);
                    CheckPiece(6, 11, 15, false);
                    break;
                case 7:
                    CheckPiece(7, 6, 8, false);
                    CheckPiece(7, 4, 1, false);
                    break;
                case 8:
                    CheckPiece(8, 7, 6, false);
                    CheckPiece(8, 12, 17, false);
                    break;
                case 9:
                    CheckPiece(9, 10, 11, false);
                    CheckPiece(9, 0, 21, false);
                    break;
                case 10:
                    CheckPiece(10, 9, 11, false);
                    CheckPiece(10, 3, 18, false);
                    break;
                case 11:
                    CheckPiece(11, 10, 9, false);
                    CheckPiece(11, 6, 15, false);
                    break;
                case 12:
                    CheckPiece(12, 13, 14, false);
                    CheckPiece(12, 8, 17, false);
                    break;
                case 13:
                    CheckPiece(13, 12, 14, false);
                    CheckPiece(13, 5, 20, false);
                    break;
                case 14:
                    CheckPiece(14, 13, 12, false);
                    CheckPiece(14, 2, 23, false);
                    break;
                case 15:
                    CheckPiece(15, 16, 17, false);
                    CheckPiece(15, 11, 6, false);
                    break;
                case 16:
                    CheckPiece(16, 15, 17, false);
                    CheckPiece(16, 19, 22, false);
                    break;
                case 17:
                    CheckPiece(17, 16, 15, false);
                    CheckPiece(17, 12, 8, false);
                    break;
                case 18:
                    CheckPiece(18, 19, 20, false);
                    CheckPiece(18, 3, 10, false);
                    break;
                case 19:
                    CheckPiece(19, 18, 20, false);
                    CheckPiece(19, 16, 22, false);
                    break;
                case 20:
                    CheckPiece(20, 19, 18, false);
                    CheckPiece(20, 13, 5, false);
                    break;
                case 21:
                    CheckPiece(21, 22, 23, false);
                    CheckPiece(21, 9, 0, false);
                    break;
                case 22:
                    CheckPiece(22, 21, 23, false);
                    CheckPiece(22, 19, 16, false);
                    break;
                case 23:
                    CheckPiece(23, 22, 21, false);
                    CheckPiece(23, 14, 2, false);
                    break;
                default:
                    break;
            }
        } //Chay het 24 o de xem o nao` co possible mill, uu tien tao Mill cho Black

        for (int i = 0; i < 24; i++) //Chay het 24 o de xem o nao` co possible mill, sau do den Block Mill cua White
        {
            switch (i)
            {
                case 0:
                    CheckPiece(0, 1, 2, true); //Tai day se ta ve mot array neu no tao thanh mot Mill
                    CheckPiece(0, 9, 21, true);
                    break;
                case 1:
                    CheckPiece(1, 0, 2, true);
                    CheckPiece(1, 4, 7, true);
                    break;
                case 2:
                    CheckPiece(2, 0, 1, true);
                    CheckPiece(2, 14, 23, true);
                    break;
                case 3:
                    CheckPiece(3, 4, 5, true);
                    CheckPiece(3, 10, 18, true);
                    break;
                case 4:
                    CheckPiece(4, 3, 5, true);
                    CheckPiece(4, 1, 7, true);
                    break;
                case 5:
                    CheckPiece(5, 4, 3, true);
                    CheckPiece(5, 13, 20, true);
                    break;
                case 6:
                    CheckPiece(6, 7, 8, true);
                    CheckPiece(6, 11, 15, true);
                    break;
                case 7:
                    CheckPiece(7, 6, 8, true);
                    CheckPiece(7, 4, 1, true);
                    break;
                case 8:
                    CheckPiece(8, 7, 6, true);
                    CheckPiece(8, 12, 17, true);
                    break;
                case 9:
                    CheckPiece(9, 10, 11, true);
                    CheckPiece(9, 0, 21, true);
                    break;
                case 10:
                    CheckPiece(10, 9, 11, true);
                    CheckPiece(10, 3, 18, true);
                    break;
                case 11:
                    CheckPiece(11, 10, 9, true);
                    CheckPiece(11, 6, 15, true);
                    break;
                case 12:
                    CheckPiece(12, 13, 14, true);
                    CheckPiece(12, 8, 17, true);
                    break;
                case 13:
                    CheckPiece(13, 12, 14, true);
                    CheckPiece(13, 5, 20, true);
                    break;
                case 14:
                    CheckPiece(14, 13, 12, true);
                    CheckPiece(14, 2, 23, true);
                    break;
                case 15:
                    CheckPiece(15, 16, 17, true);
                    CheckPiece(15, 11, 6, true);
                    break;
                case 16:
                    CheckPiece(16, 15, 17, true);
                    CheckPiece(16, 19, 22, true);
                    break;
                case 17:
                    CheckPiece(17, 16, 15, true);
                    CheckPiece(17, 12, 8, true);
                    break;
                case 18:
                    CheckPiece(18, 19, 20, true);
                    CheckPiece(18, 3, 10, true);
                    break;
                case 19:
                    CheckPiece(19, 18, 20, true);
                    CheckPiece(19, 16, 22, true);
                    break;
                case 20:
                    CheckPiece(20, 19, 18, true);
                    CheckPiece(20, 13, 5, true);
                    break;
                case 21:
                    CheckPiece(21, 22, 23, true);
                    CheckPiece(21, 9, 0, true);
                    break;
                case 22:
                    CheckPiece(22, 21, 23, true);
                    CheckPiece(22, 19, 16, true);
                    break;
                case 23:
                    CheckPiece(23, 22, 21, true);
                    CheckPiece(23, 14, 2, true);
                    break;
                default:
                    break;
            }
        } //Chay het 24 o de xem o nao` co possible mill, sau do den Block Mill cua White


        if (isNeedRandomMove && !Table.Instance.removingMove) //Neu nhu khong co 2 chesspiece nao` tao thanh 1 mill, place randomly
        {
            possibleMoves.Add(blackPieceNeedToPlace);
        }
    }

    private void RemovingPiece()
    {
        bool isNeedRandomMove = true;
        int pieceForRandomRemove = 0;
        void CheckPieceToRemove(int a, int b, int c) //this is for black removing white
        {
            int[] millSlots = { a, b, c };
            int pieceInRow = 0;
            int pieceNeedToRemove = 0;
            bool skip = false;
            bool hasBlack = false;

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

            if (!IfEveryPieceIsMilled(true))
            {
                foreach (int i in millSlots)
                {
                    if (slots[i].isWhite && !slots[i].isEmpty && !slots[i].isMilled) //If that piece is white and not milled, ++ to piece in row
                    {
                        pieceInRow++;
                        pieceNeedToRemove = i;
                        if (!hasBlack)
                        {
                            pieceForRandomRemove = i;
                        }
                    }
                    else if (!slots[i].isWhite && !slots[i].isEmpty) //But if there is a black piece, don't need to do anything
                    {
                        skip = true;
                        hasBlack = true;
                    }
                }
                if (pieceInRow >= 2 && !skip && Table.Instance.currentPlayer == Table.CurrentPlayer.Black)
                {
                    possibleMoves.Add(pieceNeedToRemove);
                    isNeedRandomMove = false;
                }
            }
            else
            {
                foreach (int i in millSlots)
                {
                    if (slots[i].isWhite && !slots[i].isEmpty) //If that piece is white and not milled, ++ to piece in row
                    {
                        pieceInRow++;
                        pieceNeedToRemove = i;
                        if (!hasBlack)
                        {
                            pieceForRandomRemove = i;
                        }
                    }
                    else if (!slots[i].isWhite && !slots[i].isEmpty) //But if there is a black piece, don't need to do anything
                    {
                        skip = true;
                        hasBlack = true;
                    }
                }
                if (pieceInRow >= 2 && !skip && Table.Instance.currentPlayer == Table.CurrentPlayer.Black)
                {
                    possibleMoves.Add(pieceNeedToRemove);
                    isNeedRandomMove = false;
                }
            }
        }

        for (int i = 0; i < 24; i++) //Chay het 24 o de xem o nao` co possible mill, uu tien tao Mill cho Black
        {
            switch (i)
            {
                case 0:
                    CheckPieceToRemove(0, 1, 2); //Tai day se ta ve mot array neu no tao thanh mot Mill
                    CheckPieceToRemove(0, 9, 21);
                    break;
                case 1:
                    CheckPieceToRemove(1, 0, 2);
                    CheckPieceToRemove(1, 4, 7);
                    break;
                case 2:
                    CheckPieceToRemove(2, 0, 1);
                    CheckPieceToRemove(2, 14, 23);
                    break;
                case 3:
                    CheckPieceToRemove(3, 4, 5);
                    CheckPieceToRemove(3, 10, 18);
                    break;
                case 4:
                    CheckPieceToRemove(4, 3, 5);
                    CheckPieceToRemove(4, 1, 7);
                    break;
                case 5:
                    CheckPieceToRemove(5, 4, 3);
                    CheckPieceToRemove(5, 13, 20);
                    break;
                case 6:
                    CheckPieceToRemove(6, 7, 8);
                    CheckPieceToRemove(6, 11, 15);
                    break;
                case 7:
                    CheckPieceToRemove(7, 6, 8);
                    CheckPieceToRemove(7, 4, 1);
                    break;
                case 8:
                    CheckPieceToRemove(8, 7, 6);
                    CheckPieceToRemove(8, 12, 17);
                    break;
                case 9:
                    CheckPieceToRemove(9, 10, 11);
                    CheckPieceToRemove(9, 0, 21);
                    break;
                case 10:
                    CheckPieceToRemove(10, 9, 11);
                    CheckPieceToRemove(10, 3, 18);
                    break;
                case 11:
                    CheckPieceToRemove(11, 10, 9);
                    CheckPieceToRemove(11, 6, 15);
                    break;
                case 12:
                    CheckPieceToRemove(12, 13, 14);
                    CheckPieceToRemove(12, 8, 17);
                    break;
                case 13:
                    CheckPieceToRemove(13, 12, 14);
                    CheckPieceToRemove(13, 5, 20);
                    break;
                case 14:
                    CheckPieceToRemove(14, 13, 12);
                    CheckPieceToRemove(14, 2, 23);
                    break;
                case 15:
                    CheckPieceToRemove(15, 16, 17);
                    CheckPieceToRemove(15, 11, 6);
                    break;
                case 16:
                    CheckPieceToRemove(16, 15, 17);
                    CheckPieceToRemove(16, 19, 22);
                    break;
                case 17:
                    CheckPieceToRemove(17, 16, 15);
                    CheckPieceToRemove(17, 12, 8);
                    break;
                case 18:
                    CheckPieceToRemove(18, 19, 20);
                    CheckPieceToRemove(18, 3, 10);
                    break;
                case 19:
                    CheckPieceToRemove(19, 18, 20);
                    CheckPieceToRemove(19, 16, 22);
                    break;
                case 20:
                    CheckPieceToRemove(20, 19, 18);
                    CheckPieceToRemove(20, 13, 5);
                    break;
                case 21:
                    CheckPieceToRemove(21, 22, 23);
                    CheckPieceToRemove(21, 9, 0);
                    break;
                case 22:
                    CheckPieceToRemove(22, 21, 23);
                    CheckPieceToRemove(22, 19, 16);
                    break;
                case 23:
                    CheckPieceToRemove(23, 22, 21);
                    CheckPieceToRemove(23, 14, 2);
                    break;
                default:
                    break;
            }
        }

        if (isNeedRandomMove)
        {
            possibleMoves.Add(pieceForRandomRemove);
        }
        removeMove = true;
    }

    private void Place()
    {
        string message = "";

        if (removeMove && Table.Instance.blackPlayer.pieceDie == 6) //Va' lai loi cua phan` nay`
        {
            message = "";
            foreach (int move in possibleMoves)
            {
                if (slots[move].isWhite && !slots[move].isEmpty)
                {
                    StartCoroutine(Table.Instance.Evaluate(move));
                    break;
                }
                message += move + ", ";
            }
        }

        message = "";
        foreach (int move in possibleMoves)
        {
            message += move + ", ";
        }
        print("All possible moves: " + message);
        int nextMove = possibleMoves[Random.Range(0, possibleMoves.Count)];
        print("Bot choose: " + nextMove);
        StartCoroutine(Table.Instance.Evaluate(nextMove));
        possibleMoves.Clear();
    }

    private void MidGameMove_FirstMethod()
    {
        List<int> FirstMove = new List<int>();

        void CheckPieceToMove(int a, int b, int c)
        {
            int[] millSlot = { a, b, c };
            int pieceHasAdjacent = -1;

            foreach (int i in millSlot)
            {
                if (!slots[i].isMilled && !slots[i].isWhite) //Doan nay de check xem neu nhu chesspiece do khong phai la blackmill
                {
                    return;
                }
                else if (slots[i].isMilled && !slots[i].isWhite) //Doan nay de check neu nhu chesspiece do la blackmill
                {
                    if (CheckIsEmpty(CheckAdjacent(i)).Count != 0)
                    {
                        foreach (int j in CheckIsEmpty(CheckAdjacent(i)))
                        {
                            pieceHasAdjacent = i;
                        }
                    }
                }
            }

            void AddToList(int value, List<int> list)
            {
                foreach (int i in list)
                {
                    if (value == i)
                    {
                        return;
                    }
                }
                list.Add(value);
            }

            //Doan code nay se duoc chay neu nhu tat ca 3 o nay` tao thanh` 1 mill
            if (pieceHasAdjacent != -1)
            {
                AddToList(pieceHasAdjacent, FirstMove);
            }
        }

        for (int i = 0; i < 24; i++) //Chay het 24 o de xem o nao` co possible mill, uu tien tao Mill cho Black
            {
                switch (i)
                {
                    case 0:
                        CheckPieceToMove(0, 1, 2); //Tai day se ta ve mot array neu no tao thanh mot Mill
                        CheckPieceToMove(0, 9, 21);
                        break;
                    case 1:
                        CheckPieceToMove(1, 0, 2);
                        CheckPieceToMove(1, 4, 7);
                        break;
                    case 2:
                        CheckPieceToMove(2, 0, 1);
                        CheckPieceToMove(2, 14, 23);
                        break;
                    case 3:
                        CheckPieceToMove(3, 4, 5);
                        CheckPieceToMove(3, 10, 18);
                        break;
                    case 4:
                        CheckPieceToMove(4, 3, 5);
                        CheckPieceToMove(4, 1, 7);
                        break;
                    case 5:
                        CheckPieceToMove(5, 4, 3);
                        CheckPieceToMove(5, 13, 20);
                        break;
                    case 6:
                        CheckPieceToMove(6, 7, 8);
                        CheckPieceToMove(6, 11, 15);
                        break;
                    case 7:
                        CheckPieceToMove(7, 6, 8);
                        CheckPieceToMove(7, 4, 1);
                        break;
                    case 8:
                        CheckPieceToMove(8, 7, 6);
                        CheckPieceToMove(8, 12, 17);
                        break;
                    case 9:
                        CheckPieceToMove(9, 10, 11);
                        CheckPieceToMove(9, 0, 21);
                        break;
                    case 10:
                        CheckPieceToMove(10, 9, 11);
                        CheckPieceToMove(10, 3, 18);
                        break;
                    case 11:
                        CheckPieceToMove(11, 10, 9);
                        CheckPieceToMove(11, 6, 15);
                        break;
                    case 12:
                        CheckPieceToMove(12, 13, 14);
                        CheckPieceToMove(12, 8, 17);
                        break;
                    case 13:
                        CheckPieceToMove(13, 12, 14);
                        CheckPieceToMove(13, 5, 20);
                        break;
                    case 14:
                        CheckPieceToMove(14, 13, 12);
                        CheckPieceToMove(14, 2, 23);
                        break;
                    case 15:
                        CheckPieceToMove(15, 16, 17);
                        CheckPieceToMove(15, 11, 6);
                        break;
                    case 16:
                        CheckPieceToMove(16, 15, 17);
                        CheckPieceToMove(16, 19, 22);
                        break;
                    case 17:
                        CheckPieceToMove(17, 16, 15);
                        CheckPieceToMove(17, 12, 8);
                        break;
                    case 18:
                        CheckPieceToMove(18, 19, 20);
                        CheckPieceToMove(18, 3, 10);
                        break;
                    case 19:
                        CheckPieceToMove(19, 18, 20);
                        CheckPieceToMove(19, 16, 22);
                        break;
                    case 20:
                        CheckPieceToMove(20, 19, 18);
                        CheckPieceToMove(20, 13, 5);
                        break;
                    case 21:
                        CheckPieceToMove(21, 22, 23);
                        CheckPieceToMove(21, 9, 0);
                        break;
                    case 22:
                        CheckPieceToMove(22, 21, 23);
                        CheckPieceToMove(22, 19, 16);
                        break;
                    case 23:
                        CheckPieceToMove(23, 22, 21);
                        CheckPieceToMove(23, 14, 2);
                        break;
                    default:
                        break;
                }
            }

        if (FirstMove.Count > 0)
        {
            int index = Random.Range(0, FirstMove.Count);
            int secondMove = CheckIsEmpty(CheckAdjacent(FirstMove[index]))[Random.Range(0, CheckIsEmpty(CheckAdjacent(FirstMove[index])).Count)];
            print("Selected Move: " + FirstMove[index] + " - " + secondMove);
            Move(FirstMove[index], secondMove);
            oldFirstMove = FirstMove[index];
            oldSecondMove = secondMove;
            oldMove = true;
        }
        else
        {
            print("second method");
            MidGameMove_SecondMethod(false, 0, 0);
        }
    } 
    
    private void Move(int fromValue, int toValue)
    {
        StartCoroutine(Table.Instance.Evaluate(fromValue));
        StartCoroutine(Table.Instance.Evaluate(toValue));
        oldFirstMove = fromValue;
        oldSecondMove = toValue;
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
    } //Tra ve 1 mang cac slot ben canh slotValue

    private List<int> CheckIsEmpty(int[] slotList) //Input la mot mang, tra ve mot List cac slots dang trong
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

    public int RemovePieceForMidGame()
    {
        List<int> avaiableMoves = new List<int>();
        bool isNeedRandomMove = true;
        int pieceForRandomRemove = 0;
        void CheckPieceToRemove(int a, int b, int c) //this is for black removing white
        {
            int[] millSlots = { a, b, c };
            int pieceInRow = 0;
            int pieceNeedToRemove = 0;
            bool skip = false;
            bool hasBlack = false;

            foreach (int i in millSlots)
            {
                if (slots[i].isWhite && !slots[i].isEmpty) //If that piece is white, ++ to piece in row
                {
                    pieceInRow++;
                    pieceNeedToRemove = i;
                    if (!hasBlack)
                    {
                        pieceForRandomRemove = i;
                    }
                }
                else if (!slots[i].isWhite && !slots[i].isEmpty) //But if there is a black piece, don't need to do anything
                {
                    skip = true;
                    hasBlack = true;
                }
            }
            if (pieceInRow >= 2 && !skip && Table.Instance.currentPlayer == Table.CurrentPlayer.Black)
            {
                avaiableMoves.Add(pieceNeedToRemove);
                isNeedRandomMove = false;
            }
        }

        for (int i = 0; i < 24; i++) //Chay het 24 o de xem o nao` co possible mill, uu tien tao Mill cho Black
        {
            switch (i)
            {
                case 0:
                    CheckPieceToRemove(0, 1, 2); //Tai day se ta ve mot array neu no tao thanh mot Mill
                    CheckPieceToRemove(0, 9, 21);
                    break;
                case 1:
                    CheckPieceToRemove(1, 0, 2);
                    CheckPieceToRemove(1, 4, 7);
                    break;
                case 2:
                    CheckPieceToRemove(2, 0, 1);
                    CheckPieceToRemove(2, 14, 23);
                    break;
                case 3:
                    CheckPieceToRemove(3, 4, 5);
                    CheckPieceToRemove(3, 10, 18);
                    break;
                case 4:
                    CheckPieceToRemove(4, 3, 5);
                    CheckPieceToRemove(4, 1, 7);
                    break;
                case 5:
                    CheckPieceToRemove(5, 4, 3);
                    CheckPieceToRemove(5, 13, 20);
                    break;
                case 6:
                    CheckPieceToRemove(6, 7, 8);
                    CheckPieceToRemove(6, 11, 15);
                    break;
                case 7:
                    CheckPieceToRemove(7, 6, 8);
                    CheckPieceToRemove(7, 4, 1);
                    break;
                case 8:
                    CheckPieceToRemove(8, 7, 6);
                    CheckPieceToRemove(8, 12, 17);
                    break;
                case 9:
                    CheckPieceToRemove(9, 10, 11);
                    CheckPieceToRemove(9, 0, 21);
                    break;
                case 10:
                    CheckPieceToRemove(10, 9, 11);
                    CheckPieceToRemove(10, 3, 18);
                    break;
                case 11:
                    CheckPieceToRemove(11, 10, 9);
                    CheckPieceToRemove(11, 6, 15);
                    break;
                case 12:
                    CheckPieceToRemove(12, 13, 14);
                    CheckPieceToRemove(12, 8, 17);
                    break;
                case 13:
                    CheckPieceToRemove(13, 12, 14);
                    CheckPieceToRemove(13, 5, 20);
                    break;
                case 14:
                    CheckPieceToRemove(14, 13, 12);
                    CheckPieceToRemove(14, 2, 23);
                    break;
                case 15:
                    CheckPieceToRemove(15, 16, 17);
                    CheckPieceToRemove(15, 11, 6);
                    break;
                case 16:
                    CheckPieceToRemove(16, 15, 17);
                    CheckPieceToRemove(16, 19, 22);
                    break;
                case 17:
                    CheckPieceToRemove(17, 16, 15);
                    CheckPieceToRemove(17, 12, 8);
                    break;
                case 18:
                    CheckPieceToRemove(18, 19, 20);
                    CheckPieceToRemove(18, 3, 10);
                    break;
                case 19:
                    CheckPieceToRemove(19, 18, 20);
                    CheckPieceToRemove(19, 16, 22);
                    break;
                case 20:
                    CheckPieceToRemove(20, 19, 18);
                    CheckPieceToRemove(20, 13, 5);
                    break;
                case 21:
                    CheckPieceToRemove(21, 22, 23);
                    CheckPieceToRemove(21, 9, 0);
                    break;
                case 22:
                    CheckPieceToRemove(22, 21, 23);
                    CheckPieceToRemove(22, 19, 16);
                    break;
                case 23:
                    CheckPieceToRemove(23, 22, 21);
                    CheckPieceToRemove(23, 14, 2);
                    break;
                default:
                    break;
            }
        }

        if (isNeedRandomMove)
        {
            avaiableMoves.Add(pieceForRandomRemove);
        }

        return avaiableMoves[Random.Range(0, avaiableMoves.Count)];
    }

    private void MidGameMove_SecondMethod(bool isSet, int oldFirstMove, int oldSecondMove)
    {
        if (!isSet) //Neu nhu day khong phai la nuoc di tu 1 Mill ra
        {
            List<int> firstMove = new List<int>();
            List<int> secondMove = new List<int>();
            bool hasFoundAWay = false;

            void CheckForLegalMove(int a, int b, int c)
            {
                int[] millSlot = { a, b, c };
            
                int chesspieceCount = 0;
                foreach (int slot in millSlot)
                {
                    if (!slots[slot].isWhite && !slots[slot].isEmpty) //Neu slot do co chesspiece mau` den
                    {
                        chesspieceCount++;
                    }
                }

                if (chesspieceCount == 2)
                {
                    foreach (int slot in millSlot)
                    {
                        //Check 1 o bat ky, xem o ben canh do co chesspiece mau` den khong thi chon o do
                        if (slots[slot].isEmpty) //Xem slot do co dang trong hay k
                        {
                            foreach (int childSlot in CheckAdjacent(slot)) //Xem nhung o ben canh no
                            {
                                if (!slots[childSlot].isWhite && !slots[childSlot].isEmpty && !slots[childSlot].isMilled && childSlot != a && childSlot != b && childSlot != c) //Xem o ben canh no, o nao` la o mau` den va khong Milled
                                {
                                    secondMove.Add(childSlot);
                                }
                            }
                            firstMove.Add(slot);
                        }
                    }
                }


                if (firstMove.Count > 0 && secondMove.Count > 0)
                {
                    string message = "";
                    foreach (int i in firstMove)
                    {
                        message += i + ", ";
                    }
                    print("First Move: " + message);
                    message = "";
                    foreach (int i in secondMove)
                    {
                        message += i + ", ";
                    }
                    print("Second Move: " + message);
                }

                if (firstMove.Count > 0 && secondMove.Count > 0)
                {
                    int corrected2ndMoveIndex = Random.Range(0, secondMove.Count);
                    int corrected1stMove = -1;

                    foreach (int index in CheckAdjacent(secondMove[corrected2ndMoveIndex]))
                    {
                        if (slots[index].isEmpty)
                        {
                            corrected1stMove = index;
                        }
                    }

                    if (corrected1stMove != -1)
                    {
                        print("Next Move: " + secondMove[corrected2ndMoveIndex] + " - " + corrected1stMove);
                        Move(secondMove[corrected2ndMoveIndex], corrected1stMove);
                        hasFoundAWay = true;
                    }
                }
            }

            for (int i = 0; i < 24; i++) //Chay het 24 o de xem o nao` co possible mill, uu tien tao Mill cho Black
            {
                if (hasFoundAWay)
                {
                    return;
                }
                switch (i)
                {
                    case 0:
                        CheckForLegalMove(0, 1, 2); //Tai day se ta ve mot array neu no tao thanh mot Mill
                        CheckForLegalMove(0, 9, 21);
                        break;
                    case 1:
                        CheckForLegalMove(1, 0, 2);
                        CheckForLegalMove(1, 4, 7);
                        break;
                    case 2:
                        CheckForLegalMove(2, 0, 1);
                        CheckForLegalMove(2, 14, 23);
                        break;
                    case 3:
                        CheckForLegalMove(3, 4, 5);
                        CheckForLegalMove(3, 10, 18);
                        break;
                    case 4:
                        CheckForLegalMove(4, 3, 5);
                        CheckForLegalMove(4, 1, 7);
                        break;
                    case 5:
                        CheckForLegalMove(5, 4, 3);
                        CheckForLegalMove(5, 13, 20);
                        break;
                    case 6:
                        CheckForLegalMove(6, 7, 8);
                        CheckForLegalMove(6, 11, 15);
                        break;
                    case 7:
                        CheckForLegalMove(7, 6, 8);
                        CheckForLegalMove(7, 4, 1);
                        break;
                    case 8:
                        CheckForLegalMove(8, 7, 6);
                        CheckForLegalMove(8, 12, 17);
                        break;
                    case 9:
                        CheckForLegalMove(9, 10, 11);
                        CheckForLegalMove(9, 0, 21);
                        break;
                    case 10:
                        CheckForLegalMove(10, 9, 11);
                        CheckForLegalMove(10, 3, 18);
                        break;
                    case 11:
                        CheckForLegalMove(11, 10, 9);
                        CheckForLegalMove(11, 6, 15);
                        break;
                    case 12:
                        CheckForLegalMove(12, 13, 14);
                        CheckForLegalMove(12, 8, 17);
                        break;
                    case 13:
                        CheckForLegalMove(13, 12, 14);
                        CheckForLegalMove(13, 5, 20);
                        break;
                    case 14:
                        CheckForLegalMove(14, 13, 12);
                        CheckForLegalMove(14, 2, 23);
                        break;
                    case 15:
                        CheckForLegalMove(15, 16, 17);
                        CheckForLegalMove(15, 11, 6);
                        break;
                    case 16:
                        CheckForLegalMove(16, 15, 17);
                        CheckForLegalMove(16, 19, 22);
                        break;
                    case 17:
                        CheckForLegalMove(17, 16, 15);
                        CheckForLegalMove(17, 12, 8);
                        break;
                    case 18:
                        CheckForLegalMove(18, 19, 20);
                        CheckForLegalMove(18, 3, 10);
                        break;
                    case 19:
                        CheckForLegalMove(19, 18, 20);
                        CheckForLegalMove(19, 16, 22);
                        break;
                    case 20:
                        CheckForLegalMove(20, 19, 18);
                        CheckForLegalMove(20, 13, 5);
                        break;
                    case 21:
                        CheckForLegalMove(21, 22, 23);
                        CheckForLegalMove(21, 9, 0);
                        break;
                    case 22:
                        CheckForLegalMove(22, 21, 23);
                        CheckForLegalMove(22, 19, 16);
                        break;
                    case 23:
                        CheckForLegalMove(23, 22, 21);
                        CheckForLegalMove(23, 14, 2);
                        break;
                    default:
                        break;
                }
            }

            //This is when we have no way left
            RandomMove();
        }
        else if (isSet && slots[oldFirstMove].isEmpty && (!slots[oldSecondMove].isEmpty && !slots[oldSecondMove].isWhite)) //Day la khi di lai nuoc di tu mot Mill ra, va toValue phai la slot trong'
        {
            Move(oldSecondMove, oldFirstMove);
        }
        else if (isSet && slots[oldSecondMove].isEmpty && slots[oldFirstMove].isEmpty) //Day la khi 1 chesspiece khi di chuyen ra khoi mill ma` bi remove ngay sau do
        {
            List<int> firstMove = new List<int>();
            List<int> secondMove = new List<int>();
            bool hasFoundAWay = false;

            void CheckForLegalMove(int a, int b, int c)
            {
                int[] millSlot = { a, b, c };

                int chesspieceCount = 0;
                foreach (int slot in millSlot)
                {
                    if (!slots[slot].isWhite && !slots[slot].isEmpty) //Neu slot do co chesspiece mau` den
                    {
                        chesspieceCount++;
                    }
                }

                if (chesspieceCount == 1)
                {
                    foreach (int slot in millSlot)
                    {
                        //Check 1 o bat ky, xem o ben canh do co chesspiece mau` den khong thi chon o do
                        if (slots[slot].isEmpty) //Xem slot do co dang trong hay k
                        {
                            foreach (int childSlot in CheckAdjacent(slot)) //Xem nhung o ben canh no
                            {
                                if (!slots[childSlot].isWhite && !slots[childSlot].isEmpty && !slots[childSlot].isMilled && childSlot != a && childSlot != b && childSlot != c) //Xem o ben canh no, o nao` la o mau` den va khong Milled
                                {
                                    secondMove.Add(childSlot);
                                }
                            }
                            firstMove.Add(slot);
                        }
                    }
                }


                if (firstMove.Count > 0 && secondMove.Count > 0)
                {
                    string message = "";
                    foreach (int i in firstMove)
                    {
                        message += i + ", ";
                    }
                    print("First Move: " + message);
                    message = "";
                    foreach (int i in secondMove)
                    {
                        message += i + ", ";
                    }
                    print("Second Move: " + message);
                }

                if (firstMove.Count > 0 && secondMove.Count > 0)
                {
                    int corrected2ndMoveIndex = Random.Range(0, secondMove.Count);
                    int corrected1stMove = -1;

                    foreach (int index in CheckAdjacent(secondMove[corrected2ndMoveIndex]))
                    {
                        if (slots[index].isEmpty)
                        {
                            corrected1stMove = index;
                        }
                    }

                    if (corrected1stMove != -1)
                    {
                        print("Next Move: " + secondMove[corrected2ndMoveIndex] + " - " + corrected1stMove);
                        Move(secondMove[corrected2ndMoveIndex], corrected1stMove);
                        hasFoundAWay = true;
                    }
                }
            }

            for (int i = 0; i < 24; i++) //Chay het 24 o de xem o nao` co possible mill, uu tien tao Mill cho Black
            {
                if (hasFoundAWay)
                {
                    return;
                }
                switch (i)
                {
                    case 0:
                        CheckForLegalMove(0, 1, 2); //Tai day se ta ve mot array neu no tao thanh mot Mill
                        CheckForLegalMove(0, 9, 21);
                        break;
                    case 1:
                        CheckForLegalMove(1, 0, 2);
                        CheckForLegalMove(1, 4, 7);
                        break;
                    case 2:
                        CheckForLegalMove(2, 0, 1);
                        CheckForLegalMove(2, 14, 23);
                        break;
                    case 3:
                        CheckForLegalMove(3, 4, 5);
                        CheckForLegalMove(3, 10, 18);
                        break;
                    case 4:
                        CheckForLegalMove(4, 3, 5);
                        CheckForLegalMove(4, 1, 7);
                        break;
                    case 5:
                        CheckForLegalMove(5, 4, 3);
                        CheckForLegalMove(5, 13, 20);
                        break;
                    case 6:
                        CheckForLegalMove(6, 7, 8);
                        CheckForLegalMove(6, 11, 15);
                        break;
                    case 7:
                        CheckForLegalMove(7, 6, 8);
                        CheckForLegalMove(7, 4, 1);
                        break;
                    case 8:
                        CheckForLegalMove(8, 7, 6);
                        CheckForLegalMove(8, 12, 17);
                        break;
                    case 9:
                        CheckForLegalMove(9, 10, 11);
                        CheckForLegalMove(9, 0, 21);
                        break;
                    case 10:
                        CheckForLegalMove(10, 9, 11);
                        CheckForLegalMove(10, 3, 18);
                        break;
                    case 11:
                        CheckForLegalMove(11, 10, 9);
                        CheckForLegalMove(11, 6, 15);
                        break;
                    case 12:
                        CheckForLegalMove(12, 13, 14);
                        CheckForLegalMove(12, 8, 17);
                        break;
                    case 13:
                        CheckForLegalMove(13, 12, 14);
                        CheckForLegalMove(13, 5, 20);
                        break;
                    case 14:
                        CheckForLegalMove(14, 13, 12);
                        CheckForLegalMove(14, 2, 23);
                        break;
                    case 15:
                        CheckForLegalMove(15, 16, 17);
                        CheckForLegalMove(15, 11, 6);
                        break;
                    case 16:
                        CheckForLegalMove(16, 15, 17);
                        CheckForLegalMove(16, 19, 22);
                        break;
                    case 17:
                        CheckForLegalMove(17, 16, 15);
                        CheckForLegalMove(17, 12, 8);
                        break;
                    case 18:
                        CheckForLegalMove(18, 19, 20);
                        CheckForLegalMove(18, 3, 10);
                        break;
                    case 19:
                        CheckForLegalMove(19, 18, 20);
                        CheckForLegalMove(19, 16, 22);
                        break;
                    case 20:
                        CheckForLegalMove(20, 19, 18);
                        CheckForLegalMove(20, 13, 5);
                        break;
                    case 21:
                        CheckForLegalMove(21, 22, 23);
                        CheckForLegalMove(21, 9, 0);
                        break;
                    case 22:
                        CheckForLegalMove(22, 21, 23);
                        CheckForLegalMove(22, 19, 16);
                        break;
                    case 23:
                        CheckForLegalMove(23, 22, 21);
                        CheckForLegalMove(23, 14, 2);
                        break;
                    default:
                        break;
                }
            }

            //This is when we have no way left
            RandomMove();
        }
        else
        {
            RandomMove();
        }
    }

    private void RandomMove() //Random move chi danh cho black
    {
        List<int> randomBlack = new List<int>();
        for (int i = 0; i < 24; i++) //Xem tren table, luu tat ca nhung vi tri co black
        {
            if (!slots[i].isEmpty && !slots[i].isWhite)
            {
                randomBlack.Add(i);
            }
        }

        int index = Random.Range(0, randomBlack.Count);
        int rFromValue = randomBlack[index];

        do
        {
            index = Random.Range(0, randomBlack.Count);
            rFromValue = randomBlack[index];
        }
        while (CheckIsEmpty(CheckAdjacent(rFromValue)).Count == 0);

        int rToValue = CheckIsEmpty(CheckAdjacent(randomBlack[index]))[Random.Range(0, CheckIsEmpty(CheckAdjacent(randomBlack[index])).Count)];

        print("This is random move: " + rFromValue + " - " + rToValue);
        Move(rFromValue, rToValue);
    }

    private void JumpMove()
    {
        bool isNeedRandomMove = true;
        int blackPieceNeedToPlace = 0;
        bool justCreatedMill = false;
        List<int> pieceToAvoid = new List<int>();
        void CheckPiece(int a, int b, int c, bool isWhite)
        {
            int[] millSlots = { a, b, c };
            if (!isWhite) //For black
            {
                int pieceInRow = 0;
                int pieceNeedToPlace = 0;
                bool skip = false;
                bool hasWhiteInRow = false;
                foreach (int i in millSlots) // day la khi co 2 black piece canh nhau
                {
                    if (!slots[i].isWhite && !slots[i].isEmpty) //If that piece is black, ++ to piece in row
                    {
                        pieceInRow++;
                        pieceToAvoid.Add(i);
                    }
                    else if (slots[i].isWhite && !slots[i].isEmpty) //But if there is a white piece, don't need to do anything
                    {
                        skip = true;
                        hasWhiteInRow = true;
                    }
                    else
                    {
                        pieceNeedToPlace = i;
                        if (!hasWhiteInRow)
                            blackPieceNeedToPlace = i;
                    }
                }

                if (pieceInRow == 2 && !skip && Table.Instance.currentPlayer == Table.CurrentPlayer.Black && !Table.Instance.removingMove)
                {
                    string message = "";
                    foreach (int i in millSlots)
                    {
                        message += i + " ";
                    }
                    print(message + ": 2 black pieces, need to place at " + pieceNeedToPlace);
                    possibleMoves.Add(pieceNeedToPlace);
                    justCreatedMill = true;
                    isNeedRandomMove = false;
                    message = "";
                    foreach (int i in pieceToAvoid)
                    {
                        message += i + ", ";
                    }
                    print("Here are avoiding pieces for last 3 pieces: " + message);
                    print("So move from: " + CheckLastBlack() + " to " + pieceNeedToPlace);
                    Move(CheckLastBlack(), pieceNeedToPlace);
                }
                else
                {
                    foreach (int i in millSlots) // day la khi co 2 black piece canh nhau
                    {
                        if (!slots[i].isWhite && !slots[i].isEmpty) //If that piece is black, remove it from the list
                        {
                            pieceToAvoid.Remove(i);
                        }
                    }
                }
            }
            else if (isWhite && !justCreatedMill)
            {
                int pieceInRow = 0;
                int pieceNeedToPlace = 0;
                bool skip = false;
                foreach (int i in millSlots)
                {
                    if (slots[i].isWhite && !slots[i].isEmpty) //If that piece is white, ++ to piece in row
                    {
                        pieceInRow++;
                        pieceToAvoid.Add(i);
                    }
                    else if (!slots[i].isWhite && !slots[i].isEmpty) //But if there is a black piece, don't need to do anything
                    {
                        skip = true;
                    }
                    else
                    {
                        pieceNeedToPlace = i;
                    }

                }
                if (pieceInRow == 2 && !skip && Table.Instance.currentPlayer == Table.CurrentPlayer.Black && !Table.Instance.removingMove)
                {
                    string message = "";
                    foreach (int i in millSlots)
                    {
                        message += i + " ";
                    }
                    print(message + ": 2 white pieces, need to place at " + pieceNeedToPlace);
                    possibleMoves.Add(pieceNeedToPlace);
                    isNeedRandomMove = false;
                    message = "";
                    foreach (int i in pieceToAvoid)
                    {
                        message += i + ", ";
                    }
                    print("Here are avoiding pieces for last 3 pieces: " + message);
                    print("So move from: " + CheckLastBlack() + " to " + pieceNeedToPlace);
                    Move(CheckLastBlack(), pieceNeedToPlace);
                }
                else
                {
                    foreach (int i in millSlots) // day la khi co 2 black piece canh nhau
                    {
                        if (!slots[i].isWhite && !slots[i].isEmpty) //If that piece is black, remove it from the list
                        {
                            pieceToAvoid.Remove(i);
                        }
                    }
                }
            }

        }

        int CheckLastBlack() //Cho nay de check black piece cuoi cung
        {
            for (int i = 0; i < 24; i++)
            {
                bool isDuplicate = false;
                foreach (int j in pieceToAvoid)
                {
                    if (i == j)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!slots[i].isWhite && !slots[i].isEmpty && !isDuplicate)
                {
                    return i;
                }
            }
            return 0;
        }

        for (int i = 0; i < 24; i++) //Chay het 24 o de xem o nao` co possible mill, uu tien tao Mill cho Black
        {
            switch (i)
            {
                case 0:
                    CheckPiece(0, 1, 2, false); //Tai day se ta ve mot array neu no tao thanh mot Mill
                    CheckPiece(0, 9, 21, false);
                    break;
                case 1:
                    CheckPiece(1, 0, 2, false);
                    CheckPiece(1, 4, 7, false);
                    break;
                case 2:
                    CheckPiece(2, 0, 1, false);
                    CheckPiece(2, 14, 23, false);
                    break;
                case 3:
                    CheckPiece(3, 4, 5, false);
                    CheckPiece(3, 10, 18, false);
                    break;
                case 4:
                    CheckPiece(4, 3, 5, false);
                    CheckPiece(4, 1, 7, false);
                    break;
                case 5:
                    CheckPiece(5, 4, 3, false);
                    CheckPiece(5, 13, 20, false);
                    break;
                case 6:
                    CheckPiece(6, 7, 8, false);
                    CheckPiece(6, 11, 15, false);
                    break;
                case 7:
                    CheckPiece(7, 6, 8, false);
                    CheckPiece(7, 4, 1, false);
                    break;
                case 8:
                    CheckPiece(8, 7, 6, false);
                    CheckPiece(8, 12, 17, false);
                    break;
                case 9:
                    CheckPiece(9, 10, 11, false);
                    CheckPiece(9, 0, 21, false);
                    break;
                case 10:
                    CheckPiece(10, 9, 11, false);
                    CheckPiece(10, 3, 18, false);
                    break;
                case 11:
                    CheckPiece(11, 10, 9, false);
                    CheckPiece(11, 6, 15, false);
                    break;
                case 12:
                    CheckPiece(12, 13, 14, false);
                    CheckPiece(12, 8, 17, false);
                    break;
                case 13:
                    CheckPiece(13, 12, 14, false);
                    CheckPiece(13, 5, 20, false);
                    break;
                case 14:
                    CheckPiece(14, 13, 12, false);
                    CheckPiece(14, 2, 23, false);
                    break;
                case 15:
                    CheckPiece(15, 16, 17, false);
                    CheckPiece(15, 11, 6, false);
                    break;
                case 16:
                    CheckPiece(16, 15, 17, false);
                    CheckPiece(16, 19, 22, false);
                    break;
                case 17:
                    CheckPiece(17, 16, 15, false);
                    CheckPiece(17, 12, 8, false);
                    break;
                case 18:
                    CheckPiece(18, 19, 20, false);
                    CheckPiece(18, 3, 10, false);
                    break;
                case 19:
                    CheckPiece(19, 18, 20, false);
                    CheckPiece(19, 16, 22, false);
                    break;
                case 20:
                    CheckPiece(20, 19, 18, false);
                    CheckPiece(20, 13, 5, false);
                    break;
                case 21:
                    CheckPiece(21, 22, 23, false);
                    CheckPiece(21, 9, 0, false);
                    break;
                case 22:
                    CheckPiece(22, 21, 23, false);
                    CheckPiece(22, 19, 16, false);
                    break;
                case 23:
                    CheckPiece(23, 22, 21, false);
                    CheckPiece(23, 14, 2, false);
                    break;
                default:
                    break;
            }
        } //Chay het 24 o de xem o nao` co possible mill, uu tien tao Mill cho Black

        for (int i = 0; i < 24; i++) //Chay het 24 o de xem o nao` co possible mill, sau do den Block Mill cua White
        {
            switch (i)
            {
                case 0:
                    CheckPiece(0, 1, 2, true); //Tai day se ta ve mot array neu no tao thanh mot Mill
                    CheckPiece(0, 9, 21, true);
                    break;
                case 1:
                    CheckPiece(1, 0, 2, true);
                    CheckPiece(1, 4, 7, true);
                    break;
                case 2:
                    CheckPiece(2, 0, 1, true);
                    CheckPiece(2, 14, 23, true);
                    break;
                case 3:
                    CheckPiece(3, 4, 5, true);
                    CheckPiece(3, 10, 18, true);
                    break;
                case 4:
                    CheckPiece(4, 3, 5, true);
                    CheckPiece(4, 1, 7, true);
                    break;
                case 5:
                    CheckPiece(5, 4, 3, true);
                    CheckPiece(5, 13, 20, true);
                    break;
                case 6:
                    CheckPiece(6, 7, 8, true);
                    CheckPiece(6, 11, 15, true);
                    break;
                case 7:
                    CheckPiece(7, 6, 8, true);
                    CheckPiece(7, 4, 1, true);
                    break;
                case 8:
                    CheckPiece(8, 7, 6, true);
                    CheckPiece(8, 12, 17, true);
                    break;
                case 9:
                    CheckPiece(9, 10, 11, true);
                    CheckPiece(9, 0, 21, true);
                    break;
                case 10:
                    CheckPiece(10, 9, 11, true);
                    CheckPiece(10, 3, 18, true);
                    break;
                case 11:
                    CheckPiece(11, 10, 9, true);
                    CheckPiece(11, 6, 15, true);
                    break;
                case 12:
                    CheckPiece(12, 13, 14, true);
                    CheckPiece(12, 8, 17, true);
                    break;
                case 13:
                    CheckPiece(13, 12, 14, true);
                    CheckPiece(13, 5, 20, true);
                    break;
                case 14:
                    CheckPiece(14, 13, 12, true);
                    CheckPiece(14, 2, 23, true);
                    break;
                case 15:
                    CheckPiece(15, 16, 17, true);
                    CheckPiece(15, 11, 6, true);
                    break;
                case 16:
                    CheckPiece(16, 15, 17, true);
                    CheckPiece(16, 19, 22, true);
                    break;
                case 17:
                    CheckPiece(17, 16, 15, true);
                    CheckPiece(17, 12, 8, true);
                    break;
                case 18:
                    CheckPiece(18, 19, 20, true);
                    CheckPiece(18, 3, 10, true);
                    break;
                case 19:
                    CheckPiece(19, 18, 20, true);
                    CheckPiece(19, 16, 22, true);
                    break;
                case 20:
                    CheckPiece(20, 19, 18, true);
                    CheckPiece(20, 13, 5, true);
                    break;
                case 21:
                    CheckPiece(21, 22, 23, true);
                    CheckPiece(21, 9, 0, true);
                    break;
                case 22:
                    CheckPiece(22, 21, 23, true);
                    CheckPiece(22, 19, 16, true);
                    break;
                case 23:
                    CheckPiece(23, 22, 21, true);
                    CheckPiece(23, 14, 2, true);
                    break;
                default:
                    break;
            }
        } //Chay het 24 o de xem o nao` co possible mill, sau do den Block Mill cua White


        if (isNeedRandomMove && !Table.Instance.removingMove) //Neu nhu khong co 2 chesspiece nao` tao thanh 1 mill, place randomly
        {
            possibleMoves.Add(blackPieceNeedToPlace);
            string message = "";
            foreach (int i in possibleMoves)
            {
                message += i + ", ";
            }
            print("Here are possible moves for last 3 pieces: " + message);
            message = "";
            foreach (int i in pieceToAvoid)
            {
                message += i + ", ";
            }
            print("Here are avoiding pieces for last 3 pieces: " + message);
            int index = Random.Range(0, possibleMoves.Count);
            do
            {
                index = Random.Range(0, possibleMoves.Count);
            }
            while (CheckLastBlack() == possibleMoves[index]);

            print("So move from: " + CheckLastBlack() + " to " + possibleMoves[index]);
            Move(CheckLastBlack(), possibleMoves[index]);
            possibleMoves.Clear();
        }
    }
}
