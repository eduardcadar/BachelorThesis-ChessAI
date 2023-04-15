using Keras;
using Keras.Layers;
using Keras.Models;
using MachineLearning.ManageData;
using Numpy;

namespace MachineLearning
{
    public class MachineLearningTrain
    {
        private BaseModel _model;

        public MachineLearningTrain()
        {
            Sequential model = new();
            model.Add(new Dense(64, activation: "relu", input_shape: new Shape(70)));
            model.Add(new Dense(64, activation: "relu"));
            model.Add(new Dense(1, activation: "tanh"));
            model.Compile(
                optimizer: "adam",
                loss: "mean_squared_error",
                metrics: new string[] { "accuracy", "msle" }
            );
            _model = model;
        }

        public void SaveModel()
        {
            string json = _model.ToJson();
            File.WriteAllText("model.json", json);
            _model.SaveWeight("model.h5");
        }

        public void LoadModel()
        {
            _model = BaseModel.ModelFromJson(
                File.ReadAllText(DataUtils.ModelsDirectory + "model.json")
            );
        }

        public void Train(float[,] positions, float[] results)
        {
            NDarray x = np.array(positions);
            NDarray y = np.array(results);

            _model.Fit(x, y, batch_size: 16, epochs: 5, verbose: 1);
        }
    }
}
