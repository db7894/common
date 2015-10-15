import sys
from bashwork.algorithm.ai.game import Agent

class MinimaxAgent(Agent):

  def get_action(self, game_state):
    def min_value(state, depth, agent):
        moves = state.get_legal_moves(agent)
        if Moves.Stop in moves: moves.remove(Moves.Stop)
        if len(moves) == 0 or depth == 0:
            return (self.evaluate(state), Moves.Stop)

        next_value = max_value if agent == 1 else min_value
        successors = ((state.get_next_states(agent, m), m) for m in moves)
        if agent == 1: depth -= 1
        return min((next_value(s, depth, agent - 1)[0], m) for s, m in successors)

    def max_value(state, depth, agent):
        moves = state.get_legal_moves(agent)
        if Moves.Stop in moves: moves.remove(Moves.Stop)
        if len(moves) == 0 or depth == 0:
            return (self.evaluate(state), Moves.Stop)

        agents = state.get_agent_size() - 1
        successors = ((state.get_next_states(agent, m), m) for m in moves)
        return max((min_value(s, depth, agents)[0], m) for s, m in successors)

    return max_value(game_state, self.depth, 0)[1]

class AlphaBetaAgent(Agent):

  def get_action(self, game_state):
    def min_value(state, depth, agent, alpha, beta):
        moves = state.get_legal_moves(agent)
        if Moves.Stop in moves: moves.remove(Moves.Stop)
        if len(moves) == 0 or depth == 0:
            return (self.evaluate(state), Moves.Stop)

        next_value = max_value if agent == 1 else min_value
        successors = ((state.get_next_states(agent, m), m) for m in moves)
        if agent == 1: depth -= 1

        value = (+sys.maxint, Moves.Stop)
        for s, m in successors:
            value = min(value, (next_value(s, depth, agent - 1, alpha, beta)[0], m))
            if value <= alpha: break
            beta = min(beta, value)
        return value

    def max_value(state, depth, agent, alpha, beta):
        moves = state.get_legal_moves(agent)
        if Moves.Stop in moves: moves.remove(Moves.Stop)
        if len(moves) == 0 or depth == 0:
            return (self.evaluate(state), Moves.Stop)

        agents = state.get_agent_size() - 1
        successors = ((state.get_next_states(agent, m), m) for m in moves)

        value = (-sys.maxint, Moves.Stop)
        for s,m in successors:
            value = max(value, (min_value(s, depth, agents, alpha, beta)[0], m))
            if value >= beta: break
            aplha = max(alpha, value)
        return value

    alpha = (-sys.maxint, Moves.Stop)
    beta  = (+sys.maxint, Moves.Stop)
    return max_value(game_state, self.depth, 0, alpha, beta)[1]

class ExpectimaxAgent(Agent):

  def get_action(self, game_state):
    def min_value(state, depth, agent):
        moves = state.get_legal_moves(agent)
        if Moves.Stop in moves: moves.remove(Moves.Stop)
        if len(moves) == 0 or depth == 0:
            return (self.evaluate(state), Moves.Stop)

        next_value = max_value if agent == 1 else min_value
        successors = ((state.get_next_states(agent, m), m) for m in moves)
        if agent == 1: depth -= 1
        return random.choice([(next_value(s, depth, agent - 1)[0], m) for s,m in successors])

    def max_value(state, depth, agent):
        moves = state.get_legal_moves(agent)
        if Moves.Stop in moves: moves.remove(Moves.Stop)
        if len(moves) == 0 or depth == 0:
            return (self.evaluate(state), Moves.Stop)

        agents = state.getNumAgents() - 1
        successors = ((state.get_next_states(agent, m), m) for m in moves)
        return max((min_value(s, depth, agents)[0], m) for s, m in successors)

    return max_value(game_state, self.depth, 0)[1]
