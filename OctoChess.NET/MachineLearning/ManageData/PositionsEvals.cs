using System;

namespace MachineLearning.ManageData
{
    public class PositionsEvals
    {
        public float[,] Positions { get; set; }
        public float[] Evals { get; set; }
        public string BestMove { get; set; }

        public PositionsEvals()
        {
            Positions = new float[0, 0];
            Evals = Array.Empty<float>();
        }

        public PositionsEvals(float[,] positions, float[] evals)
        {
            if (positions.GetLength(0) != evals.Length)
                throw new Exception("Each position should have its eval");
            Positions = positions;
            Evals = evals;
        }

        public void Add(PositionsEvals fp)
        {
            if (fp.Positions.GetLength(0) != fp.Evals.Length)
                throw new Exception("Each position should have its eval");
            if (Positions.Length == 0)
                Positions = new float[0, fp.Positions.GetLength(1)];
            Positions = DataUtils.ConcatArrays(Positions, fp.Positions);

            int newLength = Evals.Length + fp.Evals.Length;
            float[] newEvals = new float[newLength];
            Evals.CopyTo(newEvals, 0);
            fp.Evals.CopyTo(newEvals, Evals.Length);
            Evals = newEvals;
        }
    }
}
