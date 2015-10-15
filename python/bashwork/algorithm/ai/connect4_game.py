from bashwork.algorithm.ai.game import *

class Connect4GameState(GameState):
    '''
    '''

    __slots__ = ['size', 'grid']

    def __init__(self, **kwargs):
        self.size = kwargs.get('size', 10)
        self.grid = kwargs.get('grid', None)

        if not self.grid:
            self.grid = [[] for _ in range(self.size)]

    def get_legal_moves(self, agent):
        return [column for column in range(self.size) if len(self.grid[column]) < self.size]

    def get_next_states(self, agent, move):
        if '.' in self.grid[move]:
            raise GameException("invalid move {} for player {}".format(move, agent))

        new_grid = [list(column) for column in self.grid]
        new_grid[move].append(agent)
        return Connect4GameState(size=self.size, grid=new_grid)
