namespace ChessGameLibrary
{
    public class SquareCoords
    {
        public int File { get; set; }
        public int Rank { get; set; }

        public SquareCoords(int file, int rank)
        {
            File = file;
            Rank = rank;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(GetType() == obj.GetType()))
                return false;
            SquareCoords other = (SquareCoords)obj;
            return File == other.File && Rank == other.Rank;
        }

        public bool Equals(int file, int rank)
        {
            return File == file && Rank == rank;
        }

        public override int GetHashCode()
        {
            return (File + 1) * 10 + Rank;
        }

        public override string ToString()
        {
            return (char)(File + 97) + (Rank + 1).ToString();
        }
    }
}
