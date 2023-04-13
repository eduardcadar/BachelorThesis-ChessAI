using Keras;
using Keras.Layers;
using Keras.Models;
using Numpy;

namespace MachineLearning
{
    public class MachineLearningTrain
    {
        public void Train(float[,] positions, float[] results)
        {
            NDarray x = np.array(positions);
            NDarray y = np.array(results);

            var model = new Sequential();
            model.Add(new Dense(64, activation: "relu", input_shape: new Shape(70)));
            model.Add(new Dense(64, activation: "relu"));
            model.Add(new Dense(1, activation: "sigmoid"));

            model.Compile(optimizer: "adam", loss: "mean_squared_error", metrics: new string[] { "accuracy", "msle" });
            model.Fit(x, y, batch_size: 10, epochs: 5, verbose: 1);
        }
    }
}
