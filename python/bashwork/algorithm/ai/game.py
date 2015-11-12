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
import itertools
import tempfile
import json

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
    ''' Base class that represents the state of the
    game being played. All the state of the game as
    well as the rules should be implemented here.
    '''

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

    def is_finished(self):
        ''' Check if the game has been finished given
        the current state of the game.

        :returns: True if the game is finished, False otherwise
        '''
        raise NotImplementedError("is_finished")

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
        '''
        '''
        self.agent_start = kwargs.get('agent_start', AgentStart.random)
        self.agent_order = kwargs.get('agent_order', AgentOrder.ordered)

    class AgentOrder(object):
        ''' A collection of common rules for choosing which
        agent goes next in the game.
        '''

        @staticmethod
        def random(agents):
            ''' Allows the next player to be totally random
            such that the same player may very well go every
            single time.

            :param agents: The agents to build the order with
            :returns: An iterable of the next player to move
            '''
            while agents:
                yield random.choice(agents)

        @staticmethod 
        def ordered(agents):
            ''' Allows the next player to be chosen in the
            supplied order in the same loop.

            :param agents: The agents to build the order with
            :returns: An iterable of the next player to move
            '''
            return itertools.cycle(agents)

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
        self.move_history = kwargs.get('move_history', [])
        self.agents = kwargs.get('agents')
        self.rules = kwargs.get('rules', GameRules.Default)
        self.agent_order = self.rules.agent_order(self.agents)

    @classmethod
    def load_game(klass, path, **kwargs):
        ''' Given the path of a serialized game and the parameters
        for the game, reload the game.

        :param path: The path to the game state to reload
        :returns: An initialized instance of the game
        '''
        with open(path) as handle:
            kwargs['move_history'] = json.load(handle)
            return klass(**kwargs)

    def save_game(self):
        ''' Save the current game to a random file name that can
        be reloaded later and replayed.

        :returns: The file the game was stored to
        '''
        with tempfile.NamedTemporaryFile(delete=False) as handle:
            json.dump(self.move_history, handle)
            return handle.name

    def play(self):
        ''' Play the game with the current supplied parameters.

        :returns: The final state of the completed game
        '''
        state = self.get_start_state(agents)
        self.start_callback(state)

        while not state.is_finished():
            agent = self.get_next_agent(state)
            move  = agent.get_next_move(state)
            state = state.get_next_state(agent, move)
            self.move_callback(state, agent, move)
            self.move_history.append((agent.name, move))

        self.finish_callback(state)
        return state

    def get_next_agent(state):
        return self.agent_order.next()

    def get_start_state(self, agents):
        ''' Generate the starting board position for
        the current game.

        :param agents: The current agents playing the game
        :returns: The game state to start with
        '''
        raise NotImplementedError("get_start_state")

    def start_callback(self, state):
        ''' Callback for when the game has been started.

        :param state: The starting state of the game
        '''
        pass

    def move_callback(self, state, agent, move):
        ''' Callback for when a new move has been made
        in the game.

        :param state: The next state of the game
        :param agent: The agent that made the move
        :param move: The move made by the supplied agent
        '''
        pass

    def finish_callback(self, state):
        ''' Callback for when the game has been finished.

        :param state: The finished state of the game
        '''
        pass
