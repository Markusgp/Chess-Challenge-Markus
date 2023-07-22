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
            }
        }

        // Encourage checks and checkmates
        if (board.IsInCheck())
        {
            eval += board.IsWhiteToMove ? -20 : 20;
        }
        if (board.IsInCheckmate())
        {
            eval += board.IsWhiteToMove ? -1000 : 1000; // large reward for checkmate
        }

        // Encourage avoidance of draws
        if (board.IsDraw())
        {
            eval = 0;
        }

        return eval;
    }


    public (float, Move) Minimax(Board board, int depth, bool isMaximizing, float alpha, float beta)
    {
        if (depth == 0 || board.GetLegalMoves().Length == 0)
        {
            return (evaluateBoard(board), new Move()); // Best move is not relevant at the leaf node
        }

        Move bestMove = new Move();

        if (isMaximizing)
        {
            float maxEval = float.NegativeInfinity;
            foreach (Move m in board.GetLegalMoves())
            {
                board.MakeMove(m);
                (float eval, Move _) = Minimax(board, depth - 1, false, alpha, beta);
                board.UndoMove(m);
                if (eval > maxEval)
                {
                    maxEval = eval;
                    bestMove = m;
                }
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                    break;
            }
            return (maxEval, bestMove);
        }
        else
        {
            float minEval = float.PositiveInfinity;
            foreach (Move m in board.GetLegalMoves())
            {
                board.MakeMove(m);
                (float eval, Move _) = Minimax(board, depth - 1, true, alpha, beta);
                board.UndoMove(m);
                if (eval < minEval)
                {
                    minEval = eval;
                    bestMove = m;
                }
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                    break;
            }
            return (minEval, bestMove);
        }
    }


}