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
        return Minimax(board, 6, isMaximizing, float.NegativeInfinity, float.PositiveInfinity).Item2;
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

        if (board.IsInCheckmate())
        {
            eval = board.IsWhiteToMove ? float.NegativeInfinity : float.PositiveInfinity;
        }
        else if (board.IsDraw())
        {
            eval = 0; // A draw should have an evaluation of 0
        }

        return eval;
    }


    public (float, Move) Minimax(Board board, int depth, bool isMaximizing, float alpha, float beta)
    {
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw())
        {
            return (evaluateBoard(board), new Move());
        }

        Move[] moves = board.GetLegalMoves();
        Random rnd = new Random();
        moves = moves.OrderBy(x => rnd.Next()).ToArray(); // Shuffle the moves to introduce randomness

        Move bestMove = new Move();

        if (isMaximizing)
        {
            float maxEval = float.NegativeInfinity;
            foreach (Move m in moves)
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
            foreach (Move m in moves)
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