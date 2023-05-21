using System;

namespace MachineLearning.ManageData
{
    public class FilePositions
    {
        public float[,] Positions { get; set; }
        public float[] Results { get; set; }

        public FilePositions()
        {
            Positions = new float[0, 0];
            Results = Array.Empty<float>();
        }

        public FilePositions(float[,] positions)
        {
            Positions = positions;
            Results = Array.Empty<float>();
        }

        public FilePositions(float[,] positions, float[] results)
        {
            if (positions.GetLength(0) != results.Length)
                throw new Exception("Each position should have its result");
            Positions = positions;
            Results = results;
        }

        public void Add(FilePositions fp)
        {
            if (fp.Positions.GetLength(0) != fp.Results.Length)
                throw new Exception("Each position should have its result");
            if (Positions.Length == 0)
                Positions = new float[0, fp.Positions.GetLength(1)];
            Positions = DataUtils.ConcatArrays(Positions, fp.Positions);

            int newLength = Results.Length + fp.Results.Length;
            float[] newResults = new float[newLength];
            Results.CopyTo(newResults, 0);
            fp.Results.CopyTo(newResults, Results.Length);
            Results = newResults;
        }
    }
}
