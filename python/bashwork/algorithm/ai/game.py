#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
Game
- GameState
  - Agent(s)
    - legal_moves -> moves
    - get_next_move -> move
  - get_next_state -> GameState
- play
'''

# ------------------------------------------------------------------ #
# Game Constants
# ------------------------------------------------------------------ #

class Moves(object):
    ''' A collection of common moves used throughout the search
    interfaces.
    '''
    Stop = object()

# ------------------------------------------------------------------ #
# Game State
# ------------------------------------------------------------------ #

class GameState(object):

    def get_agents(self):
        ''' Retrieve the current collection of agents.

        :returns: The current collection of agents
        '''
        raise NotImplementedError("get_agents")

    def get_agent_size(self):
        return len(self.get_agents())

    def get_legal_moves(self, agent):
        ''' Retrieve the next legal moves for the supplied agent.

        :param agent: The agent to get legal moves for
        :returns: The collection of legal moves for that agent
        '''
        raise NotImplementedError("get_legal_moves")

    def get_next_states(self, agent, move):
        ''' Given the current state and the supplied agent and
        the given move, provide the next state of the search.

        :param agent: The agent to apply the next move to
        :param move: The move for the supplied agent
        :returns: The next game state after this move
        '''
        raise NotImplementedError("get_next_states")

# ------------------------------------------------------------------ #
# Game Logic
# ------------------------------------------------------------------ #

class Agent(object):
    pass

class Game(object):

    def play(self):
        '''
        '''
        state = self.get_start_state()
        while not self.is_finished(state):
            agent = self.get_next_agent(state)
            move = agent.get_next_move(state)
            state = state.get_next_state(agent, move)
        return state

    def get_next_agent(self, state):
        pass

    def is_finished(self, state):
        pass

    def get_start_state(self):
        pass
