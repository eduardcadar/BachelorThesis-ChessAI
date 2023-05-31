using ChessGameLibrary;
using OctoChessEngine;

namespace TestChessGameLibrary
{
    public class PerftTests
    {
        private readonly OctoChess _octoChess;

        public PerftTests()
        {
            _octoChess = new OctoChess();
        }

        [Fact]
        public void Test1()
        {
            _octoChess.SetFenPosition(Utils.STARTING_FEN);

            int[] positionsCount = _octoChess.Perft(3);

            Assert.Equal(20, positionsCount[0]);
            Assert.Equal(400, positionsCount[1]);
            Assert.Equal(8902, positionsCount[2]);
            Assert.Equal(197281, positionsCount[3]);
        }

        [Fact]
        public void Test2()
        {
            _octoChess.SetFenPosition("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8");

            int[] positionsCount = _octoChess.Perft(3);

            Assert.Equal(44, positionsCount[0]);
            Assert.Equal(1486, positionsCount[1]);
            Assert.Equal(62379, positionsCount[2]);
            Assert.Equal(2103487, positionsCount[3]);
            //Assert.Equal(89941194, positionsCount[4]);
        }
    }
}
