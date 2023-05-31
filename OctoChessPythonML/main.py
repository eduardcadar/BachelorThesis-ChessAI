import numpy as np
from flask import Flask, jsonify, request
from tensorflow import keras


def load_keras_model():
    models_dir = "C:\\facultate\\BachelorThesis-ChessAI\\OctoChess.NET\\NeuralNetworkModels"
    model_path = models_dir + "\\model.json"
    model_weights = models_dir + "\\model.h5"

    model_file = open(model_path, mode="r")
    model_json = model_file.read()
    model_file.close()

    loaded_model = keras.models.model_from_json(model_json)
    loaded_model.load_weights(model_weights)
    return loaded_model


model = load_keras_model()
model.compile(optimizer="sgd", loss="mean_squared_error")

app = Flask(__name__)


@app.route("/api/position_nn_eval", methods=["GET"])
def api():
    if request.method == "GET":
        pos = request.args.get("position_encoding")
        inp = np.array([[int(i) for i in pos.split(',')]])
        prediction = model.predict(inp)
        return jsonify({"prediction": str(prediction[0][0])})


if __name__ == '__main__':
    app.run(debug=False)
