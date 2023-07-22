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
        bool isMaximizing = board.IsWhiteToMove ? true : false;
        return Minimax(board, 5, isMaximizing, float.NegativeInfinity, float.PositiveInfinity).Item2;
    }

    private readonly Dictionary<PieceType, int> pieceValues = new Dictionary<PieceType, int> {
        { PieceType.None, 0 },
        { PieceType.King, 900 },
        { PieceType.Queen, 90 },
        { PieceType.Rook, 50 },
        { PieceType.Bishop, 30 },
        { PieceType.Knight, 30 },
        { PieceType.Pawn, 10 }
    };

    public float evaluateBoard(Board board)
    {
        float eval = 0;

        foreach (var pList in board.GetAllPieceLists())
        {
            foreach (Piece p in pList)
            {
                // Base value of piece
                float value = pieceValues[p.PieceType];
                eval += p.IsWhite ? value : -value;
            }
        }

        if (board.IsWhiteToMove)
        {
            if (board.IsInCheckmate())
            {
                eval = float.NegativeInfinity;
            }
            if (board.IsDraw())
            {
                eval = float.NegativeInfinity;
            }
        }
        else
        {
            if (board.IsInCheckmate())
            {
                eval = float.PositiveInfinity;
            }
            if (board.IsDraw())
            {
                eval = float.PositiveInfinity;
            }
        }

        return eval;
    }

    public (float, Move) Minimax(Board board, int depth, bool isMaximizing, float alpha, float beta)
    {

        Move[] moves = board.GetLegalMoves();
        Move[] orderedMoves = moves
            .OrderByDescending(m => m.IsCapture)
            .ThenByDescending(m => pieceValues[m.CapturePieceType])
            .ToArray();


        if (depth == 0 || orderedMoves.Length == 0)
        {
            return (evaluateBoard(board), new Move()); // Best move is not relevant at the leaf node
        }

        Move bestMove = new Move();

        if (isMaximizing)
        {
            float maxEval = float.NegativeInfinity;
            foreach (Move m in orderedMoves)
            {
                board.MakeMove(m);
                (float eval, Move _) = Minimax(board, depth - 1, false, alpha, beta);
                board.UndoMove(m);
                if (eval > maxEval)
                {
                    maxEval = eval;
                    bestMove = m;
                }
                alpha = Math.Max(alpha, maxEval);
                if (beta <= alpha)
                    break;
            }
            return (maxEval, bestMove);
        }
        else
        {
            float minEval = float.PositiveInfinity;
            foreach (Move m in orderedMoves)
            {
                board.MakeMove(m);
                (float eval, Move _) = Minimax(board, depth - 1, true, alpha, beta);
                board.UndoMove(m);
                if (eval < minEval)
                {
                    minEval = eval;
                    bestMove = m;
                }
                beta = Math.Min(beta, minEval);
                if (beta <= alpha)
                    break;
            }
            return (minEval, bestMove);
        }
    }


}