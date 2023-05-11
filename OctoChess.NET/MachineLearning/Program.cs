using MachineLearning;
using MachineLearning.ManageData;

string player = "player";

// PGN FILE -> PGN GAMES -> ENCODE POSITIONS -> ENCODED POSITIONS FILE -> TRAIN MODEL ON ENCODED POSITIONS
// THEN TEST MODEL ON POSITIONS EVALS

// GENERATE POSITIONS FILES FROM PGN FILES
//foreach (string file in Directory.GetFiles(DataUtils.PgnGamesDirectory))
//{
//    Console.WriteLine(file);
//    IEnumerable<PGNGame> pgnGames = PGNParser.ParsePGNFile(file);
//    string encodedPositionsFile = DataUtils.DataDirectory + file.Split('/')[^1][..^4] + ".csv";
//    foreach (var pgnGame in pgnGames)
//        DataManager.WritePGNGameToFile(encodedPositionsFile, pgnGame);
//}

// FOR SAME POSITION MAKE A MEAN OF THE RESULTS (LOSSES+WINS+DRAWS)
FilePositions fp = DataManager.GetMeanOfPositionResults(DataUtils.DataDirectory);

// TRAIN MODEL
MachineLearningModel model = new();

model.LoadModel();

//model.InitializeModel();
model.Train(fp.Positions, fp.Results, batch_size: 64, epochs: 100, validation_split: 0.1f);

// SAVE MODEL
model.SaveModel();

// GET POSITIONS EVALS (for testing model) from folder
//
//var positionEvals = DataManager.GetPositionsEvalsFromFolder(
//    DataUtils.PositionsEvalsDirectory,
//    type: PositionsFileType.CHESSDATA
//);
//positionEvals.Evals = positionEvals.Evals.Select(e => (float)Math.Tanh(e)).ToArray();

// TEST MODEL
//
//model.LoadModel();
//model.Evaluate(positionEvals.Positions, positionEvals.Evals);

// GET OUTPUT FOR FEN
//string fen = "5k2/ppp2p1p/8/8/5n1P/3r2R1/5qPK/8 w - - 1 34";
//Game game = new();
//game.SetPositionFromFEN(fen);
//float[,] positions = DataUtils.GamePositionToFloatPositions(game);
//var prediction = model.Predict(positions);
//Console.WriteLine(prediction);
