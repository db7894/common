
class MinimaxAgent(MultiAgentSearchAgent):

  def getAction(self, gameState):
    def min_value(state, depth, agent):
        moves = state.getLegalActions(agent)
        if Directions.STOP in moves:
            moves.remove(Directions.STOP)
        if len(moves) == 0 or depth == 0:
            return (self.evaluationFunction(state), 'STOP')
        next_value = max_value if agent == 1 else min_value
        successors = ((state.generateSuccessor(agent, m), m) for m in moves)
        if agent == 1: depth -= 1
        return min((next_value(s, depth, agent - 1)[0], m) for s,m in successors)

    def max_value(state, depth, agent):
        moves = state.getLegalActions(agent)
        if Directions.STOP in moves:
            moves.remove(Directions.STOP)
        if len(moves) == 0 or depth == 0:
            return (self.evaluationFunction(state), 'STOP')
        ghosts = state.getNumAgents() - 1
        successors = ((state.generateSuccessor(agent, m), m) for m in moves)
        return max((min_value(s, depth, ghosts)[0], m) for s,m in successors)

    return max_value(gameState, self.depth, 0)[1]

class AlphaBetaAgent(MultiAgentSearchAgent):

  def getAction(self, gameState):
    def min_value(state, depth, agent, alpha, beta):
        moves = state.getLegalActions(agent)
        if Directions.STOP in moves:
            moves.remove(Directions.STOP)
        if len(moves) == 0 or depth == 0:
            return (self.evaluationFunction(state), 'STOP')
        next_value = max_value if agent == 1 else min_value
        successors = ((state.generateSuccessor(agent, m), m) for m in moves)
        if agent == 1: depth -= 1

        value = (+1000000, 'STOP')
        for s,m in successors:
            value = min(value, (next_value(s, depth, agent - 1, alpha, beta)[0], m))
            if value <= alpha: break
            beta = min(beta, value)
        return value

    def max_value(state, depth, agent, alpha, beta):
        moves = state.getLegalActions(agent)
        if Directions.STOP in moves:
            moves.remove(Directions.STOP)
        if len(moves) == 0 or depth == 0:
            return (self.evaluationFunction(state), 'STOP')
        ghosts = state.getNumAgents() - 1
        successors = ((state.generateSuccessor(agent, m), m) for m in moves)

        value = (-1000000, 'STOP')
        for s,m in successors:
            value = max(value, (min_value(s, depth, ghosts, alpha, beta)[0], m))
            if value >= beta: break
            aplha = max(alpha, value)
        return value

    alpha = (-1000000, 'STOP')
    beta  = (+1000000, 'STOP')
    return max_value(gameState, self.depth, 0, alpha, beta)[1]

class ExpectimaxAgent(MultiAgentSearchAgent):

  def getAction(self, gameState):
    def min_value(state, depth, agent):
        moves = state.getLegalActions(agent)
        if Directions.STOP in moves:
            moves.remove(Directions.STOP)
        if len(moves) == 0 or depth == 0:
            return (self.evaluationFunction(state), 'STOP')
        next_value = max_value if agent == 1 else min_value
        successors = ((state.generateSuccessor(agent, m), m) for m in moves)
        if agent == 1: depth -= 1
        return random.choice([(next_value(s, depth, agent - 1)[0], m) for s,m in successors])

    def max_value(state, depth, agent):
        moves = state.getLegalActions(agent)
        if Directions.STOP in moves:
            moves.remove(Directions.STOP)
        if len(moves) == 0 or depth == 0:
            return (self.evaluationFunction(state), 'STOP')
        ghosts = state.getNumAgents() - 1
        successors = ((state.generateSuccessor(agent, m), m) for m in moves)
        return max((min_value(s, depth, ghosts)[0], m) for s,m in successors)

    return max_value(gameState, self.depth, 0)[1]
