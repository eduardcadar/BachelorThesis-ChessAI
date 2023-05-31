using Keras;
using Keras.Layers;
using Keras.Models;
using MachineLearning.ManageData;
using Numpy;
using System;
using System.IO;

namespace MachineLearning
{
    public class MachineLearningModel
    {
        public string Optimizer { get; } = "sgd";
        public string Loss { get; } = "mean_squared_error";
        private BaseModel _model;

        public MachineLearningModel()
        {
            _model = new BaseModel();
        }

        public void InitializeModel()
        {
            Sequential model = new Sequential();
            model.Add(new Dense(32, activation: "relu", input_shape: new Shape(70)));
            model.Add(new Dense(1, activation: "tanh"));
            model.Compile(optimizer: Optimizer, loss: Loss);
            _model = model;
        }

        public void Train(
            float[,] positions,
            float[] results,
            int batch_size = 32,
            int epochs = 4,
            float validation_split = 0.2f
        )
        {
            NDarray x = np.array(positions);
            NDarray y = np.array(results);

            _model.Fit(
                x,
                y,
                batch_size: batch_size,
                epochs: epochs,
                validation_split: validation_split,
                verbose: 1
            );
        }

        public void SaveModel()
        {
            string json = _model.ToJson();
            File.WriteAllText(DataUtils.ModelsDirectory + "model.json", json);
            _model.SaveWeight(DataUtils.ModelsDirectory + "model.h5");
        }

        public void LoadModel()
        {
            _model = BaseModel.ModelFromJson(
                File.ReadAllText(DataUtils.ModelsDirectory + "model.json")
            );
            _model.LoadWeight(DataUtils.ModelsDirectory + "model.h5");
            _model.Compile(optimizer: Optimizer, loss: Loss);
        }

        public float[] Predict(float[,] positions)
        {
            return _model.Predict(positions, verbose: 0).GetData<float>();
        }

        public void Evaluate(float[,] positions, float[] results, int batch_size = 2)
        {
            var score = _model.Evaluate(positions, results, batch_size: batch_size);
            Console.WriteLine($"Test loss: {score[0]}");
        }
    }
}
