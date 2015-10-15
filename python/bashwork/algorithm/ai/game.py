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
import uuid
import random

# ------------------------------------------------------------------ #
# Game Constants
# ------------------------------------------------------------------ #

class Moves(object):
    ''' A collection of common moves used throughout the search
    interfaces.
    '''
    Tie  = 'move:tie' # TODO should these be here
    Quit = 'move:quit'

# ------------------------------------------------------------------ #
# Game State
# ------------------------------------------------------------------ #

class GameState(object):

    def get_legal_moves(self, agent):
        ''' Retrieve the next legal moves for the supplied agent.

        :param agent: The agent to get legal moves for
        :returns: The collection of legal moves for that agent
        '''
        raise NotImplementedError("get_legal_moves")

    def get_next_state(self, agent, move):
        ''' Given the current state and the supplied agent and
        the given move, provide the next state of the game.

        :param agent: The agent to apply the next move to
        :param move: The move for the supplied agent
        :returns: The next game state after this move
        '''
        raise NotImplementedError("get_next_state")

# ------------------------------------------------------------------ #
# Game Logic
# ------------------------------------------------------------------ #

class Agent(object):
    ''' Base class for a game agent. Each agent is another player
    in the current game. The agents can be smart / dump AI or human
    controlled players.
    '''

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the Agent class.

        :param name: The unique identifier for this player (uuid)
        '''
        self.name = kwargs.get('name', str(uuid.uuid4()))

    def evaluate(self, state):
        ''' Given a game state, evaluate the value of that 
        game state to the current agent.

        :param state: The game state to evaluate
        :returns: A score for the supplied game state
        '''
        raise NotImplementedError("evaluate")

    def get_next_move(self, state):
        ''' Given the current state of the game, produce
        the next move for this agent.

        :param state: The state of this agent
        :returns: The next move for this agent
        '''
        raise NotImplementedError("get_next_move")

class GameRules(object):
    ''' The game rules contains a collection of reusable
    rule components like:

    * which player goes first (random)
    * which player goes next (next in line)
    * what is the starting game state (random)
    '''

    def __init__(self, **kwargs):
        pass

    class AgentNext(object):

        @staticmethod
        def random():
            return lambda agent, agents: random.choice(agents)

        @staticmethod 
        def ordered(agents):
            return lambda agent, agents: agent

    class AgentStart(object):

        @staticmethod
        def random():
            return lambda agents: random.choice(agents)

        @staticmethod 
        def static(agent):
            return lambda agents: agent

class Game(object):
    ''' Represents the logical rules of the game. Agents are not allowed
    to mutate the game instance itself, only observe the current state
    and make their next decision as a result.
    '''

    def __init__(self, **kwargs):
        '''
        '''
        # TODO allow game replay and saving
        self.move_history = kwargs.get('move_history', [])
        self.agents = kwargs.get('agents')
        self.rules = kwargs.get('rules', GameRules.Random)

    def play(self):
        '''
        '''
        # TODO add gui callbacks
        state = self.get_start_state()
        while not self.is_finished(state):
            agent = self.get_next_agent(state)
            move  = agent.get_next_move(state)
            state = state.get_next_state(agent, move)
            self.move_history.append((agent.name, move))
        return state

    def is_finished(self, state):
        '''
        '''
        raise NotImplementedError("is_finished")

    def get_start_state(self):
        '''
        '''
        raise NotImplementedError("get_start_state")
