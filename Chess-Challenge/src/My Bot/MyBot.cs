using ChessChallenge.API;
using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Console.WriteLine("New eval: " + evaluateBoard(board));
        return Minimax(board, 5, float.NegativeInfinity, float.PositiveInfinity, new Move()).Item2;
    }

    public float evaluateBoard(Board board)
    {
        float eval = 0;

        var pieceValues = new Dictionary<PieceType, int> {
            { PieceType.None, 0 },
            { PieceType.King, 900 },
            { PieceType.Queen, 90 },
            { PieceType.Rook, 50 },
            { PieceType.Bishop, 30 },
            { PieceType.Knight, 30 },
            { PieceType.Pawn, 10 }
        };

        foreach (var pList in board.GetAllPieceLists())
        {
            foreach (Piece p in pList)
            {
                //Evaluation heuristic
                float value = pieceValues[p.PieceType];
                eval += p.IsWhite ? value : -value; //Value of pieces
                eval += p.IsWhite ? board.GetLegalMoves().Length : - board.GetLegalMoves().Length; //Amount of legal moves (mobility)
                
            }
        }
        return eval;
    }

    public (float, Move) Minimax(Board board, int depth, float alpha, float beta, Move bestMove)
    {
        if (depth == 0 || board.GetLegalMoves().Length == 0)
        {
            return (evaluateBoard(board), bestMove); // Return the evaluation and no move at leaf nodes
        }

        Move[] legalMoves = board.GetLegalMoves();

        if (board.IsWhiteToMove)
        {
            float bestValue = float.NegativeInfinity;
            foreach (Move m in legalMoves)
            {
                board.MakeMove(m);
                (float value, _) = Minimax(board, depth - 1, alpha, beta, bestMove);
                board.UndoMove(m); // Undo the move after evaluating

                if (value > bestValue)
                {
                    bestValue = value;
                    bestMove = m;
                }

                alpha = Math.Max(alpha, bestValue);
                if (alpha >= beta)
                {
                    break;
                }
            }
            return (bestValue, bestMove);
        }
        else
        {
            float bestValue = float.PositiveInfinity;
            foreach (Move m in legalMoves)
            {
                board.MakeMove(m);
                (float value, _) = Minimax(board, depth - 1, alpha, beta, bestMove);
                board.UndoMove(m); // Undo the move after evaluating

                if (value < bestValue)
                {
                    bestValue = value;
                    bestMove = m;
                }

                beta = Math.Min(beta, bestValue);
                if (alpha >= beta)
                {
                    break;
                }
            }
            return (bestValue, bestMove);
        }
    }


}