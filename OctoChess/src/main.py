import random

import chess
import chess.engine

from src.smthing import square_to_index, split_dims, build_model, get_dataset


def random_board(max_depth=200):
    board = chess.Board()
    depth = random.randrange(0, max_depth)

    for _ in range(depth):
        all_moves = list(board.legal_moves)
        random_move = random.choice(all_moves)
        board.push(random_move)
        if board.is_game_over():
            break
    return board


def stockfish(board, depth):
    with chess.engine.SimpleEngine.popen_uci('../stockfish/stockfish-windows-2022-x86-64-avx2') as sf:
        result = sf.analyse(board, chess.engine.Limit(depth=depth))
        score = result['score'].white().score()
        return score


gen_board = random_board(max_depth=3)
# print(gen_board.legal_moves)
# print(gen_board)
# print(stockfish(gen_board, 10))

# for move in gen_board.legal_moves:
#     print(move.to_square, end=' ')
#     print(square_to_index(move.to_square))

# print(gen_board)
# print(split_dims(gen_board))

model = build_model(32, 4)

x_train, y_train = get_dataset()
x_train.transpose()
print(x_train.shape)
print(y_train.shape)
