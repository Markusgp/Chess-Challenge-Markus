using ChessChallenge.API;
using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    /// <summary>
    /// <para> Submission by Markus Grand Petersen </para>
    /// The main entry point for the bot to think about the move.
    /// The bot uses the Minimax algorithm with Alpha-Beta pruning.
    /// </summary>
    public Move Think(Board board, Timer timer)
    {
        //White is the maximizer (goal is a positive score)
        //Black is the minimizer (goal is a negative score)
        //Console.WriteLine("Actual eval: "+evaluateBoard(board));
        var result = Minimax(board, 5, board.IsWhiteToMove, float.NegativeInfinity, float.PositiveInfinity);
        Console.WriteLine("Bot eval: "+ result.Item1);
        return result.Item2;
    }

    // Dictionary to assign values to each piece type.
    private readonly Dictionary<PieceType, int> pieceValues = new Dictionary<PieceType, int> {
        { PieceType.None, 0 },
        { PieceType.King, 900 },
        { PieceType.Queen, 90 },
        { PieceType.Rook, 50 },
        { PieceType.Bishop, 30 },
        { PieceType.Knight, 30 },
        { PieceType.Pawn, 10 }
    };

    /// <summary>
    /// Evaluates the board position based on heuristic
    /// </summary>
    public float evaluateBoard(Board board)
    {
        float eval = 0;

        foreach (PieceList pList in board.GetAllPieceLists())
        {
            foreach (Piece p in pList)
            {
                // Base value of piece heuristic
                float value = pieceValues[p.PieceType];

                // Mobility bonus
                // value += 0.1f * board.GetLegalMoves().Length;

                //Material evaluation sum (white is positive, black is negative).
                eval += p.IsWhite ? value : -value;
            }
        }

        if(board.IsInCheckmate()) {
            eval = board.IsWhiteToMove ? float.NegativeInfinity : float.PositiveInfinity;
        }

        if(board.IsDraw()) {
            eval = 0;
        }

        return eval;
    }

    /// <summary>
    /// Minimax function with Alpha-Beta pruning.
    /// This function runs recursively until it reaches the given depth or finds a terminal state (Checkmate or Draw).
    /// It returns the evaluation of the board and the best move to reach that evaluation.
    /// </summary>
    public (float, Move) Minimax(Board board, int depth, bool isMaximizing, float alpha, float beta)
    {
        if (board.IsInCheckmate() || board.IsDraw() || depth == 0)
        {
            return (evaluateBoard(board), new Move());
        }

        Move[] moves = board.GetLegalMoves();

        if (isMaximizing)
        {
            float maxEval = float.NegativeInfinity;
            Move bestWhiteMove = new Move();
            
            foreach (Move m in moves)
            {
                board.MakeMove(m);
                (float eval, Move _) = Minimax(board, depth - 1, false, alpha, beta);

                // Checking if the evaluation of this move is better than the current maxEval
                if (eval > maxEval)
                {
                    maxEval = eval;
                    bestWhiteMove = m;
                }
                board.UndoMove(m);
                // Alpha-Beta pruning
                alpha = Math.Max(alpha, maxEval);
                if (beta <= alpha) {
                    break;
                }
            }
            return (maxEval, bestWhiteMove);
        }
        else
        {
            float minEval = float.PositiveInfinity;
            Move bestBlackMove = new Move();
            foreach (Move m in moves)
            {
                board.MakeMove(m);
                (float eval, Move _) = Minimax(board, depth - 1, true, alpha, beta);

                // Checking if the evaluation of this move is better than the current minEval
                if (eval < minEval)
                {
                    minEval = eval;
                    bestBlackMove = m;
                }
                board.UndoMove(m);

                //Alpha-Beta pruning
                beta = Math.Min(beta, minEval);
                if (beta <= alpha) {
                    break;
                }
            }
            return (minEval, bestBlackMove);
        }
    }
}
