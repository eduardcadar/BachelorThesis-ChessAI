using OctoChessEngine.Domain;
using OctoChessEngine.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoChessEngine
{
    public class Evaluation
    {
        private OctoChess _octoChess;

        // key: position fen, value: list of best moves given by StockFish
        private Dictionary<string, List<string>> _evaluationPositions;

        public Evaluation()
        {
            _octoChess = new OctoChess();
            _evaluationPositions = new Dictionary<string, List<string>>();
            AddEvaluationPositions();
        }

        public void Evaluate()
        {
            List<double> materialPrecisions = new List<double>();
            List<double> nnPrecisions = new List<double>();
            List<double> materialPrecisionsAtK = new List<double>();
            List<double> nnPrecisionsAtK = new List<double>();
            var keys = _evaluationPositions.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                string fen = keys[i];
                Console.WriteLine($"Position {i + 1}/{keys.Length}");
                var bestMovesStockfish = _evaluationPositions[fen];
                _octoChess.SetFenPosition(fen);

                _octoChess.ClearPreviousEvals();
                _octoChess.BestMove(
                    evaluationType: EvaluationType.MATERIAL,
                    useIterativeDeepening: false,
                    maxQuiescenceDepth: 4
                );
                _octoChess.OrderMoveEvals();
                var bestMovesMaterial = _octoChess.MoveEvals
                    .Take(bestMovesStockfish.Count)
                    .ToList();

                _octoChess.ClearPreviousEvals();
                _octoChess.BestMove(
                    evaluationType: EvaluationType.TRAINED_MODEL,
                    useIterativeDeepening: false,
                    maxQuiescenceDepth: 4
                );
                _octoChess.OrderMoveEvals();
                var bestMovesNN = _octoChess.MoveEvals.Take(bestMovesStockfish.Count).ToList();

                double matPrec = Precision(bestMovesMaterial, bestMovesStockfish);
                materialPrecisions.Add(matPrec);
                double nnPrec = Precision(bestMovesNN, bestMovesStockfish);
                nnPrecisions.Add(nnPrec);

                double matPrecAtK = PrecisionAtK(bestMovesMaterial, bestMovesStockfish);
                materialPrecisionsAtK.Add(matPrecAtK);
                double nnPrecAtK = PrecisionAtK(bestMovesNN, bestMovesStockfish);
                nnPrecisionsAtK.Add(nnPrecAtK);

                File.AppendAllText(
                    @"C:\facultate\BachelorThesis-ChessAI\OctoChess.NET\OctoChessEngine\materialPrecision.txt",
                    matPrec + " " + fen + Environment.NewLine
                );
                File.AppendAllText(
                    @"C:\facultate\BachelorThesis-ChessAI\OctoChess.NET\OctoChessEngine\nnPrecision.txt",
                    nnPrec + " " + fen + Environment.NewLine
                );

                File.AppendAllText(
                    @"C:\facultate\BachelorThesis-ChessAI\OctoChess.NET\OctoChessEngine\materialPrecisionAtK.txt",
                    matPrecAtK + " " + fen + Environment.NewLine
                );
                File.AppendAllText(
                    @"C:\facultate\BachelorThesis-ChessAI\OctoChess.NET\OctoChessEngine\nnPrecisionAtK.txt",
                    nnPrecAtK + " " + fen + Environment.NewLine
                );
            }

            Console.WriteLine("Material precision: " + materialPrecisions.Average());
            Console.WriteLine("NN precision: " + nnPrecisions.Average());

            Console.WriteLine("Material precision at k: " + materialPrecisionsAtK.Average());
            Console.WriteLine("NN precision at k: " + nnPrecisionsAtK.Average());
        }

        public double Precision(List<MoveEval> moveEvals, List<string> stockfishMoves)
        {
            int okCount = 0;
            double precisionsSum = 0;
            var moveEvalsStrings = moveEvals.Select(m => m.ToStringMoveOnly()).ToArray();
            for (int i = 0; i < stockfishMoves.Count; i++)
            {
                okCount++;
                if (moveEvalsStrings[i] == stockfishMoves[i])
                    precisionsSum += (double)okCount / (i + 1);
            }
            return precisionsSum / stockfishMoves.Count;
        }

        public double PrecisionAtK(List<MoveEval> moveEvals, List<string> stockfishMoves)
        {
            int okCount = 0;
            var moveEvalsStrings = moveEvals.Select(m => m.ToStringMoveOnly()).ToArray();
            foreach (string moveEval in moveEvalsStrings)
                if (stockfishMoves.Contains(moveEval))
                    okCount++;
            return (double)okCount / stockfishMoves.Count;
        }

        public void ShowEvaluationResultsFromFiles()
        {
            string materialPrecFile =
                @"C:\facultate\BachelorThesis-ChessAI\OctoChess.NET\OctoChessEngine\materialPrecision.txt";
            string nnPrecFile =
                @"C:\facultate\BachelorThesis-ChessAI\OctoChess.NET\OctoChessEngine\nnPrecision.txt";
            string materialPrecAtKFile =
                @"C:\facultate\BachelorThesis-ChessAI\OctoChess.NET\OctoChessEngine\materialPrecisionAtK.txt";
            string nnPrecAtKFile =
                @"C:\facultate\BachelorThesis-ChessAI\OctoChess.NET\OctoChessEngine\nnPrecisionAtK.txt";

            double[] materialPrecResults = File.ReadAllLines(materialPrecFile)
                .Select(line => line.Split(' ', 2)[0])
                .Select(double.Parse)
                .ToArray();
            double[] nnPrecResults = File.ReadAllLines(nnPrecFile)
                .Select(line => line.Split(' ', 2)[0])
                .Select(double.Parse)
                .ToArray();
            double[] materialPrecAtKResults = File.ReadAllLines(materialPrecAtKFile)
                .Select(line => line.Split(' ', 2)[0])
                .Select(double.Parse)
                .ToArray();
            double[] nnPrecAtKResults = File.ReadAllLines(nnPrecAtKFile)
                .Select(line => line.Split(' ', 2)[0])
                .Select(double.Parse)
                .ToArray();

            Console.WriteLine(
                "Material evaluation precision: " + Math.Round(materialPrecResults.Average(), 6)
            );
            Console.WriteLine(
                "Neural network evaluation precision: " + Math.Round(nnPrecResults.Average(), 6)
            );
            Console.WriteLine(
                "Material evaluation precision at k: "
                    + Math.Round(materialPrecAtKResults.Average(), 6)
            );
            Console.WriteLine(
                "Neural network evaluation precision at k: "
                    + Math.Round(nnPrecAtKResults.Average(), 6)
            );
        }

        private void AddEvaluationPositions()
        {
            //_evaluationPositions["5B2/6P1/1p6/8/1N6/kP6/2K5/8 w - - 0 1"] = new List<string>()
            //{
            //    "g7g8N",
            //    "g7g8Q",
            //    "b4d5",
            //    "b4d3",
            //    "g7g8R"
            //};
            //_evaluationPositions["rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"] =
            //    new List<string>() { "e2e4", "d2d4", "c2c4", "g1f3", "g2g3" };
            //_evaluationPositions["r1b1kb1r/1p4pp/p2ppn2/8/2qNP3/2N1B3/PPP3PP/R2Q1RK1 w kq - 0 1"] =
            //    new List<string>() { "f1f6", "e4e5", "d4f3", "a2a4", "c3b1" };
            //_evaluationPositions["1r5k/2q2p1p/p2p3B/5PQ1/n1p5/2b4P/PrB3P1/2R1R1K1 w - - 0 1"] =
            //    new List<string>() { "e1e5", "e1e7", "e1e3", "c2a4", "e1e4" };
            //_evaluationPositions["2kr3r/pRp3q1/2Pppn2/4p2p/B2bP1p1/7P/2P2PP1/2BQ1RK1 w - - 0 1"] =
            //    new List<string>() { "c1h6", "d1d4", "c1g5", "d1d3", "h3h4" };
            //_evaluationPositions["r2r2k1/ppqbbppp/4pn2/6N1/n1P4P/3B1N2/PP2QPP1/1KBR3R w - - 0 1"] =
            //    new List<string>() { "e2c2", "g5h7", "d3c2", "f3e5", "g2g4" };
            //_evaluationPositions["r2rn3/1p3pk1/p1pNn1pp/q3P3/P7/1PN4P/2QR1PP1/3R2K1 w - - 0 1"] =
            //    new List<string>() { "d6f7", "d1e1", "d2e2", "d2d3", "d6b7" };
            //_evaluationPositions[
            //    "r1b1kb1r/1p1n1ppp/p2ppn2/6BB/2qNP3/2N5/PPP2PPP/R2Q1RK1 w kq - 0 1"
            //] = new List<string>() { "d4e6", "h5e2", "h5f3", "g5e3", "d4b3" };
            //_evaluationPositions["2rr2k1/4bppp/p1n1p3/3q4/1p1P2N1/2P3R1/P3QPPP/2B2RK1 w - - 0 10"] =
            //    new List<string>() { "g5h6", "a2a3", "f1d1", "g4e3", "a2a4" };
            //_evaluationPositions["3r2k1/p6p/b2r2p1/2qPQp2/2P2P2/8/6BP/R4R1K w - - 0 1"] =
            //    new List<string>() { "a1a6", "f1b1", "a1b1", "f1c1", "a1a4" };
            //_evaluationPositions["r5k1/pn1q1rpp/2pp4/5R1N/bP6/4BQ2/P4PPP/2R3K1 w - - 0 1"] =
            //    new List<string>() { "h5g7", "f5f7", "g2g4", "f5f4", "h5g3" };
            //_evaluationPositions["r2qr3/2p1b1pk/p5pp/1p2p3/nP2P1P1/1BP2RP1/P3QPK1/R1B5 w - - 0 1"] =
            //    new List<string>() { "c1h6", "c1e3", "f3d3", "c1d2", "a1b1" };
            //_evaluationPositions["r4r1k/2q2ppp/1np1b3/p1b1pNB1/2B1P3/8/PPP1Q1PP/3R1R1K w - - 0 1"] =
            //    new List<string>() { "f5g7", "c4e6", "b2b3", "c4d3", "e2h5" };
            //_evaluationPositions[
            //    "r2qrbk1/5p1p/2b1pPp1/2np2P1/ppPpPB1N/3P1R2/PP4B1/1R2Q1K1 w - - 0 1"
            //] = new List<string>() { "h4g6", "c4d5", "f3h3", "e4e5", "f4d2" };
            //_evaluationPositions["3r4/2n1qr2/pkp2pp1/N1p3n1/Q3P1B1/2PP2Pp/1P5P/K2R1R2 w - - 0 1"] =
            //    new List<string>() { "a4c6", "d3d4", "g4h3", "a5c4", "e4e5" };
            //_evaluationPositions["r4rk1/pp3ppp/2p4q/2P5/1b1P4/1B3NPb/PP2Q2P/R4RK1 w - - 0 1"] =
            //    new List<string>() { "f3e5", "a2a3", "a1d1", "f1f2", "e2e7" };
            //_evaluationPositions["8/p2bRpkp/5np1/3p2Q1/p2P4/P4BNP/1qr2PP1/6K1 w - - 0 1"] =
            //    new List<string>() { "g5e5", "f3d5", "g3h5", "g1h2", "g3e2" };
            //_evaluationPositions["r3r1k1/pp3pp1/3p4/2q4p/2P5/1PB2Q1P/n4PP1/3R1RK1 w - - 0 1"] =
            //    new List<string>() { "c3g7", "c3a1", "c3b2", "d1d5", "c3d4" };
            //_evaluationPositions[
            //    "2r1r1k1/pb1n1pp1/1p1qpn1p/4N1B1/2PP4/3B4/P2Q1PPP/3RR1K1 w - - 0 1"
            //] = new List<string>() { "g5h6", "e5d7", "g5f4", "g5h4", "g5e3" };
            //_evaluationPositions["rqbn1rk1/1p3ppp/p3p3/8/4NP2/5Q2/PPP1B1PP/1K1R3R w - - 0 1"] =
            //    new List<string>() { "e4f6", "f3g3", "h1e1", "g2g4", "h1f1" };
            //_evaluationPositions["r3r1k1/p3bppp/q1b2n2/5Q2/1p1B4/1BNR4/PPP3PP/2K2R2 w - - 0 1"] =
            //    new List<string>() { "d3g3", "c3d5", "d4f6", "f1f4", "f1f2" };
            //_evaluationPositions["1q1r3k/3P1pp1/ppBR1n1p/4Q2P/P4P2/8/5PK1/8 w - - 0 1"] =
            //    new List<string>() { "e5e7", "g2f1", "g2g1", "g2f3", "g2h3" };
            //_evaluationPositions[
            //    "4rrk1/2pn2pb/p1p1qp2/1pb1pN2/P3P1PN/1P1P4/1BP2PK1/R2Q3R w - - 0 1"
            //] = new List<string>() { "f5g7", "a4b5", "d1f3", "h1h3", "h1h2" };
            //_evaluationPositions[
            //    "r3kbr1/pb1n1p2/2q1p2p/2P1P3/1p3Q1B/1Pp2N2/P3BPPP/R2R2K1 w q - 0 1"
            //] = new List<string>() { "d1d7", "a2a3", "a1c1", "f4b4", "d1d4" };
            //_evaluationPositions[
            //    "r1br2k1/p1q2pp1/4p1np/2ppP2Q/2n5/2PB1N2/2P2PPP/R1B1R1K1 w - - 0 1"
            //] = new List<string>() { "c1h6", "g1h1", "h2h3", "a1b1", "h2h4" };
            //_evaluationPositions["1rbqnr2/5p1k/p1np3p/1p1Np3/4P2P/1Q2B3/PPP1BP2/2KR3R w - - 0 1"] =
            //    new List<string>() { "e3h6", "h1g1", "d1g1", "f2f4", "e3d2" };
            //_evaluationPositions[
            //    "r1b1k1nr/3n1p1p/1qpBp1p1/pp2b3/4PN2/PBN2Q2/1PP2PPP/2KR3R w kq - 0 1"
            //] = new List<string>() { "f4e6", "f3g3", "h1e1", "b3e6", "c1b1" };
            //_evaluationPositions["r4rk1/1pqbbN1p/p2pp1p1/6Pn/P3P3/2N1B3/1PP2Q1P/R4RK1 w - - 0 1"] =
            //    new List<string>() { "c3d5", "f7h6", "a1d1", "a1e1", "a1c1" };
            //_evaluationPositions["r3r1k1/1bqn1ppp/1pp2p2/8/3P4/1B4N1/PP3PPP/R2QR1K1 w - - 0 1"] =
            //    new List<string>() { "b3f7", "e1e8", "g3f5", "d1g4", "h2h4" };
            //_evaluationPositions["r1bq1rk1/p3bppp/2p1p3/2npP3/5B2/3B4/PPPN1PPP/R2QR1K1 w - - 0 1"] =
            //    new List<string>() { "d3h7", "d2b3", "d1h5", "d3f1", "a1b1" };
            //_evaluationPositions["rn5r/1b1kb2p/pq2pp2/1p1n2BQ/3NN3/3B4/PP3PPP/R4RK1 w - - 0 1"] =
            //    new List<string>() { "e4f6", "g5f6", "g5e3", "d4e6", "f1d1" };

            //_evaluationPositions["r1b2r1k/5ppp/p1qppb1B/8/4P3/1B6/PpP1QP1P/1K1R2R1 w - - 0 1"] =
            //    new List<string>() { "e4e5", "e2f3", "h6f4", "e2h5", "e2e3" };
            //_evaluationPositions["1r3rk1/b1pR1p2/p3p1p1/2q1P1Kp/P1N2P2/1P1Q2P1/7P/3R4 w - - 0 1"] =
            //    new List<string>() { "g5h6", "d1d2", "d3f3", "c4d2", "h2h4" };
            //_evaluationPositions[
            //    "r2qr1k1/pbpp1ppn/1p2p3/n3P1P1/2PPN3/P1P5/5PP1/R2QKB1R w KQ - 0 1"
            //] = new List<string>() { "e4f6", "f1d3", "d1e2", "d1g4", "d1f3" };
            //_evaluationPositions["r4r1k/1bq2pp1/2p1p2p/1pPnP1BQ/4B2P/1p4P1/P4P2/3RR1K1 w - - 0 1"] =
            //    new List<string>() { "d1d5", "a2b3", "g5h6", "a2a3", "g1g2" };

            //_evaluationPositions["2q3r1/2r2ppk/b3p3/p2pPP1P/1p1B2Q1/1P6/1PPR4/4K1R1 w - - 0 1"] =
            //    new List<string>() { "g4g6", "e1d1", "g4g2", "f5e6", "g1g2" };
            //_evaluationPositions["r4k1r/1p3pp1/2b1p1np/p3P3/3PN1q1/4Q1P1/5P1P/R1R2BK1 w - - 0 1"] =
            //    new List<string>() { "c1c6", "f1g2", "f2f3", "e4d6", "f2f4" };
            //_evaluationPositions["r6r/6k1/b2pBpp1/2bP4/p1q1P3/2P2RN1/2Q3PP/3R3K w - - 0 1"] =
            //    new List<string>() { "e4e5", "d1b1", "h2h3", "c2c1", "d1a1" };
            //_evaluationPositions[
            //    "2rrnbk1/1pqb1pp1/p2pp2p/6N1/Pn1BPP2/2N3Q1/1PP1B1PP/3R1R1K w - - 0 1"
            //] = new List<string>() { "e2h5", "a4a5", "g3f2", "g5f7", "g3h4" };
            //_evaluationPositions["r1b1k2r/pp3ppp/4pb2/2n5/3RB3/4BN2/PqP1QPPP/3R2K1 w kq - 0 1"] =
            //    new List<string>() { "d4d8", "f3g5", "e4h7", "h2h3", "e3c1" };

            //_evaluationPositions[
            //    "rq3rk1/1b1n1ppp/ppn1p3/3pP3/5B2/2NBP2P/PP2QPP1/2RR2K1 w - - 0 1"
            //] = new List<string>() { "c3d5", "d3b1", "e2h5", "e2g4", "c3a4" };
            //_evaluationPositions["r4rk1/1pqb1pp1/pn2p2p/b3P3/P4Q1B/1B3N2/1P3PPP/1K1R3R w - - 0 1"] =
            //    new List<string>() { "h4f6", "f4g4", "d1d6", "d1c1", "d1d4" };
            //_evaluationPositions["r2q1rk1/1p2bpp1/p1b2n1p/8/5B2/2NB4/PP1Q1PPP/3R1RK1 w - - 0 1"] =
            //    new List<string>() { "f4h6", "d2e2", "d2e1", "d2e3", "f1e1" };
            //_evaluationPositions[
            //    "r3k2r/2q2p1p/p1b1pb2/3p4/1p1Nn1Q1/8/PPP1NPPP/R1B1R1K1 w kq - 0 1"
            //] = new List<string>() { "d4e6", "g4h5", "c1h6", "c1e3", "c1f4" };
        }
    }
}
