using System.Text;

namespace MachineLearning.PGN
{
    public class PGNGame
    {
        public PGNTag[] Tags { get; set; }
        public PGNMove[] Moves { get; set; }
        public GameResult GameResult { get; set; }

        public PGNGame(PGNTag[] tags, PGNMove[] moves, GameResult gameResult)
        {
            Tags = tags;
            Moves = moves;
            GameResult = gameResult;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Moves.Length).Append(" moves: ");
            foreach (PGNMove move in Moves)
            {
                sb.Append(move.ToString()).Append(' ');
            }
            sb.Append(GameResult.ToString());
            return sb.ToString();
        }
    }
}
