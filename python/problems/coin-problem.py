'''
The encoding of the grid shape is as follows::

    0b4321 -> 1 2
              4 3
'''
from random import randint, choice
from functools import partial

# --------------------------------------------------------------------------------
# all grid positions and constants
# --------------------------------------------------------------------------------

solved  = {0b0000, 0b1111}
one     = [0b0001, 0b0010, 0b0100, 0b1000]
two_opp = [0b1010, 0b0101]
two_adj = [0b0011, 0b0110, 0b1100, 0b1001]
three   = [0b0111, 0b1110, 0b1101, 0b1011]
four    = [0b0000, 0b1111]
states  = one + two_opp + two_adj + three + four

hi_mask = [0b1111, 0b0111, 0b0011, 0b0001]
lo_mask = [0b0000, 0b1000, 0b1100, 0b1110]

# --------------------------------------------------------------------------------
# all mutations on the current state
# --------------------------------------------------------------------------------

rotate            = lambda state, choice: ((state & hi_mask[choice]) << choice) | ((state & lo_mask[choice]) >> choice)
rotate_rand       = lambda state: rotate(state, randint(0, 3))
rotates           = [partial(rotate, choice=c) for c in range(4)]
flip_none         = lambda state: state
flip_one          = lambda state, choice: state ^ one[choice]
flip_one_rand     = lambda state: flip_one(state, randint(0, 3))
flip_two_opp      = lambda state, choice: state ^ two_opp[choice]
flip_two_opp_rand = lambda state: flip_two_opp(state, randint(0, 1))
flip_two_adj      = lambda state, choice: state ^ two_adj[choice]
flip_two_adj_rand = lambda state: flip_two_adj(state, randint(0, 3))
flip_three        = lambda state, choice: state ^ three[choice]
flip_three_rand   = lambda state: flip_three(state, randint(0, 3))
flip_four         = lambda state: state ^ four[1]
flips             = [flip_one_rand, flip_two_opp_rand, flip_two_adj_rand, flip_three_rand, flip_four]
flip_random       = lambda state: choice(flips)(state)

# --------------------------------------------------------------------------------
# solvers
# --------------------------------------------------------------------------------

all_possible_states = lambda states: { r(s) for s in states for r in rotates }
remove_solutions = lambda states: { s for s in states if s not in solved }
next_round = lambda states: remove_solutions(all_possible_states(states))

def solve(state):
    moves = [state]
    while state not in solved:
        state = flip_random(state)
        state = rotate_rand(state)
        moves.append(state)
    print("state [{:04b}] solved in {:2d} moves".format(moves[0], len(moves)))

def solve_all():
    for state in states:
        solve(state)

if __name__ == "__main__":
    solve_all()
