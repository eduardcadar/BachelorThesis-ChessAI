namespace ChessGameLibrary
{
    public class SimpleMove
    {
        public SquareCoords From { get; set; }
        public SquareCoords To { get; set; }

        public SimpleMove(SquareCoords from, SquareCoords to)
        { From = from; To = to; }

        public override string ToString()
        {
            return From.ToString() + "-" + To.ToString();
        }
    }
}
