using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class Bot : MonoBehaviour
{
    public static Bot Instance;
    private Slots[] slots;
    public List<int> possibleMoves = new List<int>();

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void CallBot()
    {
        if (Table.Instance.currentPlayer == Table.CurrentPlayer.Black)
        {
            slots = Table.Instance.slots;
            if (Table.Instance.blackPlayer.pieceSleep > 0 && !Table.Instance.removingMove) //this is for placing, to block player mill and creating bot mill
            {
                StartingGame_PlacingChessPiece();
            }

            if (Table.Instance.removingMove)
            {
                StartingGame_RemovingPiece();
            }

            Move();
        }
    }

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
                    //print(message + ": 2 black pieces, need to place at " + pieceNeedToPlace);
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
                    //print(message + ": 2 white pieces, need to place at " + pieceNeedToPlace);
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

    private void StartingGame_RemovingPiece()
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
                possibleMoves.Add(pieceNeedToRemove);
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
            possibleMoves.Add(pieceForRandomRemove);
        }
    }

    private void Move()
    {
        ClearLog();
        string message = "";
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

    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
