import sys
import time
import random
import logging
import pygame

try:
    import curses
except ImportError:
    pass # no pretty logging
 
# ------------------------------------------------------------
# utilities
# ------------------------------------------------------------
def find(coll, pred):
    ''' A helper method to find a value in a collection

    :param coll: The collection to search
    :param pred: The item to find or predicate to match
    :returns: The index of the first found item, or -1 if not found
    '''
    if not hasattr(pred, '__call__'):
        chck = pred
        pred = lambda a: a == chck
    for idx, value in enumerate(coll):
        if pred(value): return idx
    return -1

def setup_logging(debug=False):
    ''' Setup the python logging facility
    '''
    if not debug: return
    color = False
    if curses and sys.stderr.isatty():
        try:
            curses.setupterm()
            color = (curses.tigetnum("colors") > 0)
        except Exception: pass
    root = logging.getLogger()
    root.setLevel(logging.DEBUG)
    stream = logging.StreamHandler()
    stream.setFormatter(_LogFormatter(color=color))
    root.addHandler(stream)

# ------------------------------------------------------------
# pretty logging 
# ------------------------------------------------------------
class _LogFormatter(logging.Formatter):

    def __init__(self, color, *args, **kwargs):
        logging.Formatter.__init__(self, *args, **kwargs)
        self._color = color
        if color:
            # The curses module has some str/bytes confusion in python3.
            # Most methods return bytes, but only accept strings.
            # The explict calls to unicode() below are harmless in python2,
            # but will do the right conversion in python3.
            fg_color = unicode(curses.tigetstr("setaf") or 
                               curses.tigetstr("setf") or "", "ascii")
            self._colors = {
                logging.DEBUG:   unicode(curses.tparm(fg_color, 4), "ascii"),
                logging.INFO:    unicode(curses.tparm(fg_color, 2), "ascii"),
                logging.WARNING: unicode(curses.tparm(fg_color, 3), "ascii"),
                logging.ERROR:   unicode(curses.tparm(fg_color, 1), "ascii"),
            }
            self._normal = unicode(curses.tigetstr("sgr0"), "ascii")

    def format(self, record):
        try:
            record.message = record.getMessage()
        except Exception, e:
            record.message = "Bad message (%r): %r" % (e, record.__dict__)
        record.asctime = time.strftime(
            "%y%m%d %H:%M:%S", self.converter(record.created))
        prefix = '[%(levelname)1.1s %(asctime)s %(module)s:%(lineno)d]' % \
            record.__dict__
        if self._color:
            prefix = (self._colors.get(record.levelno, self._normal) +
                      prefix + self._normal)
        formatted = prefix + " " + record.message
        if record.exc_info:
            if not record.exc_text:
                record.exc_text = self.formatException(record.exc_info)
        if record.exc_text:
            formatted = formatted.rstrip() + "\n" + record.exc_text
        return formatted.replace("\n", "\n    ")

# ------------------------------------------------------------
# options
# ------------------------------------------------------------
def get_options():
    ''' A helper method to retrieve the command line arguments

    :returns: The command line arguments
    '''
    from optparse import OptionParser

    parser = OptionParser()
    parser.add_option("-r", "--red", dest="red", action="store_true", default=True,
        help="Play as the red player (first turn)")
    parser.add_option("-b", "--black", dest="red", action="store_false",
        help="Play as the black player (second turn)")
    parser.add_option("-s", "--size", dest="size", type="int", default=7,
        help="The size of the game board")
    parser.add_option("-c", "--computer", dest="computer", default="random",
        help="The computer strategy to use (random, minimax)")
    parser.add_option("-v", "--verbose", dest="verbose", action="store_true", default=False,
        help="Enable debug logging to stdout")
    parser.add_option("-d", "--difficulty", dest="difficulty", type="int", default=2,
        help="The difficulty of the computer")
    parser.add_option("-l", "--length", dest="length", type="int", default=4,
        help="The number of tokens to connect to win")
    (options, args) = parser.parse_args()
    return options
 

# ------------------------------------------------------------
# exceptions
# ------------------------------------------------------------
class InvalidMoveException(Exception):
    ''' Indicates that an invalid action has been attempted
    '''
    pass
 
# ------------------------------------------------------------
# classes
# ------------------------------------------------------------
class Color(object):
    ''' A collection of common colors
    '''
    black  = (  0,  0,  0)
    white  = (245,245,245)
    green  = (  0,255,  0)
    red    = (255,  0,  0)
    blue   = (  0,  0,255)
    yellow = (255,215,  0)

 
class Piece(object):
    ''' Represents the board pieces for the game
    '''
    empty    = ' '
    player   = 'p'
    computer = 'c'

    @staticmethod
    def switch(piece):
        ''' A helper method to switch piece turns

        :param piece: The piece to switch
        :returns: The opposing players piece
        '''
        if piece == Piece.player:
            return Piece.computer
        return Piece.player
 
 
class Winner(object):
    ''' Represents a winning state
    '''
    player   = ''.join([Piece.player]*4)
    computer = ''.join([Piece.computer]*4)
 
 
class Board(object):
    ''' Represents the current game board state
    '''
 
    @staticmethod
    def clone(board):
        ''' Given an initial board, create a clone
 
        :param board: The board to clone
        :returns: A deep copy of that board
        '''
        clean = Board(size=board.size)
        clean.grid = [list(a) for a in board.grid]
        return clean
 
    def __init__(self, **kwargs):
        ''' Initialize a default game board
 
        :param size: The size of the board
        '''
        self.size = kwargs.get('size', 7)
        self.grid = [[Piece.empty]*self.size for _ in range(self.size)]
 
    def add_piece(self, piece, col):
        ''' Add a new piece to the board
 
        :raises: An exception if the position is illegal
        '''
        row = [g[col] for g in self.grid]
        idx = find(row, Piece.empty)
        if idx != -1:
            self.grid[idx][col] = piece
        else: raise InvalidMoveException()

    def is_valid_move(self, col):
        ''' Checks if a move on the specified column is valid

        :param col: The column to check for a valid move
        :returns: True if valid, False otherwise
        '''
        row = [g[col] for g in self.grid]
        return Piece.empty in row

    def get_valid_moves(self):
        ''' Returns all the currently valid moves

        :returns: A list of valid moves
        '''
        return [i for i in range(self.size)
                if self.is_valid_move(i)]
 
    def is_game_tied(self):
        ''' Check if the game is finished at a tie
 
        :returns: True if tied, False otherwise
        '''
        for row in self.grid:
            for val in row:
                if val == Piece.empty:
                    return False
        return True
 
    def is_game_won(self):
        ''' Check if the game has a winner
 
        :returns: The winner or False otherwise
        '''
        winner  = self.is_row_winner()
        winner  = winner or self.is_col_winner()
        winner  = winner or self.is_dial_winner()
        winner  = winner or self.is_diar_winner()
        return winner
 
    def is_row_winner(self):
        ''' Check if the game has a row winner
 
        :returns: The winner or False otherwise
        '''
        for row in self.grid:
            value = ''.join(row)
            if Winner.player   in value: return Piece.player
            if Winner.computer in value: return Piece.computer
        return False
 
    def is_col_winner(self):
        ''' Check if the game has a column winner
 
        :returns: The winner or False otherwise
        '''
        for idx in range(self.size):
            col = [c[idx] for c in self.grid]
            value = ''.join(col)
            if Winner.player   in value: return Piece.player
            if Winner.computer in value: return Piece.computer
        return False
 
    def is_dial_winner(self):
        ''' Check if the game has a left diaganol winner
 
        :returns: The winner or False otherwise
        '''
        for x in range(self.size - 3):
            for y in range(3, self.size):
                piece      = self.grid[x][y]
                if piece  == Piece.empty: continue
                is_winner  = self.grid[x+1][y-1] == piece
                is_winner &= self.grid[x+2][y-2] == piece
                is_winner &= self.grid[x+3][y-3] == piece
                if is_winner: return piece
        return False

    def is_diar_winner(self):
        ''' Check if the game has a right diaganol winner
 
        :returns: The winner or False otherwise
        '''
        for x in range(self.size - 3):
            for y in range(self.size - 3):
                piece      = self.grid[x][y]
                if piece  == Piece.empty: continue
                is_winner  = self.grid[x+1][y+1] == piece
                is_winner &= self.grid[x+2][y+2] == piece
                is_winner &= self.grid[x+3][y+3] == piece
                if is_winner: return piece
        return False


class GameState(object):
    ''' maintains the current state of the game
    '''

    def __init__(self, options, **kwargs):
        ''' Initialize a new instance of the game state
        '''
        self.winner = None
        self.size   = options.size
        self.board  = Board(size=self.size)
        self.margin = kwargs.get('margin',  5)
        self.rate   = kwargs.get('rate',   20)
        self.width  = kwargs.get('width', 100)
        self.tsize  = (self.width + self.margin) * self.size + self.margin
        self.screen = pygame.display.set_mode([self.tsize, self.tsize])
        self.clock  = pygame.time.Clock()
        self.font   = pygame.font.SysFont(None, 70)
        self.difficulty = options.difficulty
        self.computer = self.__get_computer(options.computer)
        setup_logging(options.verbose)
        self.is_running = True

    @staticmethod
    def __get_computer(strategy):
        ''' A helper method to lookup the computer to play

        :param strategy: The strategy for the computer to use
        '''
        g = globals()
        computer = g.get(strategy + '_computer', None)
        computer = computer or random_computer
        logging.info('user competing against %s', computer)
        return computer

# ------------------------------------------------------------
# computer game logic
# ------------------------------------------------------------
def get_state_features(board, piece):
    ''' Get the feature values for the supplied game state

    :param board: The current game state
    :param piece: The piece the computer is using
    :returns: The feature values for this board
    '''
    is_game_won = (board.is_game_won() == piece)
    off_count = 1
    def_count = 1

    return {
        'game_won': is_game_won,
        'offense':  off_count,
        'defense':  def_count,
    }

def get_state_value(board, piece):
    ''' Get the current heuristic value for the supplied game state

    :param board: The current game state
    :param piece: The piece the computer is using
    :returns: The current value for the state
    '''
    features = get_state_features(board, piece)
    weights  = {
        'game_won': 1000,
        'offense':     1,
        'defense':     1,
    }
    return sum(weights[k] * v for k,v in features.items())

def random_computer(state):
    ''' A computer that simply produces a random move

    :param state: The current state of the game
    '''
    choices = state.board.get_valid_moves()
    return random.choice(choices)

def minimax_computer(state):
    ''' A computer that uses minimax to produce the next move

    :param state: The current state of the game
    '''
    def minimax(board, piece, depth):
        if depth == 0 or board.is_game_won():
            return (get_state_value(board, piece), -1)

        value  = (sys.maxint, -1)
        npiece = Piece.switch(piece)
        for choice in state.board.get_valid_moves():
            nboard = Board.clone(board)
            nboard.add_piece(piece, choice)
            value = min(value, (-minimax(nboard, npiece, depth - 1)[0], choice))
        return value
    return minimax(state.board, Piece.computer, state.difficulty)[1]


def abeta_computer(state):
    ''' A computer that uses alphabeta to produce the next move

    :param state: The current state of the game
    '''
    def minimax(board, piece, depth):
        if depth == 0 or board.is_game_won():
            return (get_state_value(board, piece), -1)

        value  = (sys.maxint, -1)
        npiece = Piece.switch(piece)
        for choice in state.board.get_valid_moves():
            nboard = Board.clone(board)
            nboard.add_piece(piece, choice)
            value = min(value, (-minimax(nboard, npiece, depth - 1)[0], choice))
        return value
    return minimax(state.board, Piece.computer, state.difficulty)[1]

# ------------------------------------------------------------
# python game logic
# ------------------------------------------------------------
def handle_game_sound(state, sound):
    ''' Handle playing a game sound

    :param sound: The sound to play
    :param state: The current game state
    '''
    logging.debug('playing sound file %s', sound)
    sound = pygame.mixer.Sound(sound)
    sound.play()
    while pygame.mixer.get_busy():
        state.clock.tick(state.rate)

def handle_game_over(state):
    ''' Present the game over screen

    :param state: The current game state
    '''
    texta = state.font.render(state.winner, True, Color.black)
    textb = state.font.render("Game Over",  True, Color.black)

    while not state.is_running:
        for event in pygame.event.get():
            if event.type == pygame.KEYDOWN: break
            if event.type == pygame.QUIT: break

        state.screen.fill(Color.white)
        state.screen.blit(texta, (
            (state.tsize - texta.get_width())  // 2,
            (state.tsize - texta.get_height()) // 2 - texta.get_height()))
        state.screen.blit(textb, (
            (state.tsize - textb.get_width())  // 2,
            (state.tsize - textb.get_height()) // 2))
        state.clock.tick(state.rate)
        pygame.display.flip()
    handle_game_quit(state)

def handle_game_step(state):
    ''' Handle the next game step

    :param state: The current game state
    '''
    if not state.is_running:
        handle_game_over(state)
    else: handle_next_board(state)

def handle_game_events(state):
    ''' Handle the main game events

    :param state: The current game state
    '''
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            handle_game_quit(state)
        elif event.type == pygame.MOUSEBUTTONDOWN:
            handle_drop_token(state)
        else: pass

def handle_game_quit(state):
    ''' event handler for the quit event
 
    :param event: The event associated with this callback
    '''
    logging.debug("user quit the program")
    pygame.quit()
    sys.exit(1)

def handle_drop_token(state):
    ''' event handler for the mouse click event

    :param event: The event associated with this callback
    '''
    pos = pygame.mouse.get_pos()
    row = min(state.size - 1, pos[1] // (state.width + state.margin))
    col = min(state.size - 1, pos[0] // (state.width + state.margin))
    logging.debug("user clicked (%d, %d)", row, col)

    if not state.board.is_valid_move(col):
        return # invalid column, return to main loop

    state.board.add_piece(Piece.player, col)
    if check_game_state(state):
        return # user won, return to main loop

    choice = state.computer(state)
    state.board.add_piece(Piece.computer, choice)
    check_game_state(state)

def check_game_state(state):
    ''' Check the game state between moves
    '''
    is_won  = state.board.is_game_won()
    is_tied = state.board.is_game_tied()
    is_done = is_won or is_tied

    if is_won == Piece.player:
        state.winner = "You Won"
    elif is_won == Piece.computer:
        state.winner = "The Computer Won"
    elif is_tied:
        state.winner = "You Tied The Computer"
    state.is_running = not is_done

    return is_done

def handle_next_board(state):
    ''' Draw the next board to be displayed
    '''
    state.screen.fill(Color.yellow)
    for row in range(state.size):
        for col in range(state.size):
            if state.board.grid[row][col] == Piece.player:
                color = Color.red
            elif state.board.grid[row][col] == Piece.computer:
                color = Color.black
            else: color = Color.white
            cpos = [
                (state.margin + state.width) * col + state.margin + state.width/2,
                (state.margin + state.width) * (state.size - row - 1) + state.margin + state.width/2]
            rpos = [
                (state.margin + state.width) * col + state.margin,
                (state.margin + state.width) * row + state.margin, state.width, state.width]
            pygame.draw.rect(state.screen, Color.black, rpos, 1)
            pygame.draw.circle(state.screen, color, cpos, state.width/2, 0)
            pygame.draw.circle(state.screen, Color.black, cpos, state.width/2, 1)
    state.clock.tick(state.rate)
    pygame.display.flip()
    
 
# ------------------------------------------------------------
# initialization
# ------------------------------------------------------------
def initialize_game():
    ''' Perform all the game initialization

    :returns: The game state used through out the game
    '''
    pygame.init()
    pygame.display.set_caption("Connect Four")
    options = get_options()
    return GameState(options)
    
    #token_size = 50
    #red_token  = pygame.image.load('red_token.png').convert()
    #red_token  = pygame.transform.smoothscale(red_token, (token_size, token_size))
    #blk_token  = pygame.image.load('blk_token.png').convert()
    #blk_token  = pygame.transform.smoothscale(blk_token, (token_size, token_size))
 
 
# ------------------------------------------------------------
# main loop
# ------------------------------------------------------------
def main():
    state = initialize_game()
    while state.is_running:
        handle_game_events(state)
        handle_game_step(state)
    handle_game_quit(state)

if __name__ == "__main__": main()
