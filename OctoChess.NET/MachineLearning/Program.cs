using MachineLearning;
using MachineLearning.ManageData;

string player = "player";

//DataManager.GetPositionsFromFile(dataFile);

//foreach (var game in games)
//    dataManager.WritePGNGameToFile(dataFile, game);

//var positionEvals = DataManager.GetPositionsEvalsFromFolder(
//    DataUtils.PositionsEvalsDirectory,
//    type: PositionsFileType.RANDOM_EVALS
//);

// PGN FILE -> PGN GAMES -> ENCODE POSITIONS -> ENCODED POSITIONS FILE -> TRAIN MODEL ON ENCODED POSITIONS
// THEN TEST MODEL ON POSITIONS EVALS

// GET PGN GAMES FROM PGN FILE
//string pgnGamesFile = $"{DataUtils.PgnGamesDirectory}{player}_games.pgn";
//IEnumerable<PGNGame> pgnGames = PGNParser.ParsePGNFile(pgnGamesFile);

// WRITE PGN GAME TO ENCODED POSITIONS FILE
//string encodedPositionsFile = $"{DataUtils.DataDirectory}{player}.csv";
//foreach (var pgnGame in pgnGames)
//    DataManager.WritePGNGameToFile(encodedPositionsFile, pgnGame);

// TRAIN MODEL
//
MachineLearningModel model = new();

//FilePositions fp = DataManager.GetTrainPositionsFromFolder(DataUtils.DataDirectory);

// FOR SAME POSITION MAKE A MEAN OF THE RESULTS (LOSSES+WINS+DRAWS)
//FilePositions fp = DataManager.GetMeanOfPositionResults(DataUtils.DataDirectory);

model.LoadModel();

//model.InitializeModel();
//model.Train(fp.Positions, fp.Results, batch_size: 32, epochs: 100, validation_split: 0.3f);

// SAVE MODEL
//
//model.SaveModel();

// GET POSITIONS EVALS (for testing model) from folder
//
var positionEvals = DataManager.GetPositionsEvalsFromFolder(
    DataUtils.PositionsEvalsDirectory,
    type: PositionsFileType.CHESSDATA
);
positionEvals.Evals = positionEvals.Evals.Select(e => (float)Math.Tanh(e)).ToArray();

// TEST MODEL
//
model.LoadModel();
model.Evaluate(positionEvals.Positions, positionEvals.Evals);
