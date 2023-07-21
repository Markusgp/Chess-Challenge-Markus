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
        return Minimax(board, 7, float.NegativeInfinity, float.PositiveInfinity, true).Item2;
    }

    public float evaluateBoard(Board board)
    {
        var pieceValues = new Dictionary<PieceType, int> {
            { PieceType.None, 0 },
            { PieceType.King, 900 },
            { PieceType.Queen, 90 },
            { PieceType.Rook, 50 },
            { PieceType.Bishop, 30 },
            { PieceType.Knight, 30 },
            { PieceType.Pawn, 10 }
        };

        float eval = 0;
        foreach (var pList in board.GetAllPieceLists())
        {
            foreach (Piece p in pList)
            {
                float value = pieceValues[p.PieceType];
                eval += p.IsWhite ? value : -value;
            }
        }
        return eval;
    }

    public (float, Move) Minimax(Board board, int depth, float alpha, float beta, bool isMaximizer)
    {
        Move bestMove = new Move();
        if (depth == 0 || board.GetLegalMoves().Length == 0)
        {
            return (evaluateBoard(board), bestMove); // Return the evaluation and no move at leaf nodes
        }

        Move[] legalMoves = board.GetLegalMoves();

        if (isMaximizer)
        {
            float bestValue = float.NegativeInfinity;
            foreach (Move m in legalMoves)
            {
                board.MakeMove(m);
                (float value, _) = Minimax(board, depth - 1, alpha, beta, false);
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
                (float value, _) = Minimax(board, depth - 1, alpha, beta, true);
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