#Credit to Mohamed Akram, minesweeper source code from his public github https://gist.github.com/mohd-akram

#I made modifications to integrate with my solver and reduce logging. GameData Class, StartGameGorSolver and MakeMoveForSolver were my biggest changes

"""A command line version of Minesweeper"""
import random
import re
import time
from string import ascii_lowercase

ALPHABET = "abcdefghijklmnopqrstuvwxyz"

class GameData:
    gameCurrentGrid = []
    gameFlagsList = []
    gameGrid = []
    gameMines = []
    gameGridSize = 9
    gameNumMines = 10
    gameState = 0
    def __init__(self):
        self.gameCurrentGrid = []
        self.gameFlagsList = []
        self.gameGrid = []
        self.gameMines = []
        self.gameGridSize = 9
        self.gameNumMines = 10
        self.gameState = 0

    def CoveredCells(self) :
        result = []
        for i in range(self.gameGridSize) :
            for j in range(self.gameGridSize) :
                if self.gameCurrentGrid[i][j] == " ":
                    result.append((i, j))
        return result


def setupgrid(gridsize, start, numberofmines):
    emptygrid = [['0' for i in range(gridsize)] for i in range(gridsize)]

    mines = getmines(emptygrid, start, numberofmines)

    for i, j in mines:
        emptygrid[i][j] = 'X'

    grid = getnumbers(emptygrid)

    return (grid, mines)


def showgrid(grid):
    gridsize = len(grid)

    horizontal = '   ' + (4 * gridsize * '-') + '-'

    # Print top column letters
    toplabel = '     '

    for i in ascii_lowercase[:gridsize]:
        toplabel = toplabel + i + '   '

    print(toplabel + '\n' + horizontal)

    # Print left row numbers
    for idx, i in enumerate(grid):
        row = '{0:2} |'.format(idx + 1)

        for j in i:
            row = row + ' ' + j + ' |'

        print(row + '\n' + horizontal)

    print('')


def getrandomcell(grid):
    gridsize = len(grid)

    a = random.randint(0, gridsize - 1)
    b = random.randint(0, gridsize - 1)

    return (a, b)


def getneighbors(grid, rowno, colno):
    gridsize = len(grid)
    neighbors = []

    for i in range(-1, 2):
        for j in range(-1, 2):
            if i == 0 and j == 0:
                continue
            elif -1 < (rowno + i) < gridsize and -1 < (colno + j) < gridsize:
                neighbors.append((rowno + i, colno + j))

    return neighbors


def getmines(grid, start, numberofmines):
    mines = []
    neighbors = getneighbors(grid, *start)

    for i in range(numberofmines):
        cell = getrandomcell(grid)
        while cell == start or cell in mines or cell in neighbors:
            cell = getrandomcell(grid)
        mines.append(cell)

    return mines


def getnumbers(grid):
    for rowno, row in enumerate(grid):
        for colno, cell in enumerate(row):
            if cell != 'X':
                # Gets the values of the neighbors
                values = [grid[r][c] for r, c in getneighbors(grid,
                                                              rowno, colno)]

                # Counts how many are mines
                grid[rowno][colno] = str(values.count('X'))

    return grid


def showcells(grid, currgrid, rowno, colno):
    # Exit function if the cell was already shown
    if currgrid[rowno][colno] != ' ':
        return

    # Show current cell
    currgrid[rowno][colno] = grid[rowno][colno]

    # Get the neighbors if the cell is empty
    if grid[rowno][colno] == '0':
        for r, c in getneighbors(grid, rowno, colno):
            # Repeat function for each neighbor that doesn't have a flag
            if currgrid[r][c] != 'F':
                showcells(grid, currgrid, r, c)


def playagain():
    choice = input('Play again? (y/n): ')

    return choice.lower() == 'y'


def parseinput(inputstring, gridsize, helpmessage):
    cell = ()
    flag = False
    message = "Invalid cell. " + helpmessage

    pattern = r'([a-{}])([0-9]+)(f?)'.format(ascii_lowercase[gridsize - 1])
    validinput = re.match(pattern, inputstring)

    if inputstring == 'help':
        message = helpmessage

    elif validinput:
        rowno = int(validinput.group(2)) - 1
        colno = ascii_lowercase.index(validinput.group(1))
        flag = bool(validinput.group(3))

        if -1 < rowno < gridsize:
            cell = (rowno, colno)
            message = ''

    return {'cell': cell, 'flag': flag, 'message': message}


def playgame(gridsize = 9, numberofmines = 10):

    currgrid = [[' ' for i in range(gridsize)] for i in range(gridsize)]

    grid = []
    flags = []
    starttime = 0

    helpmessage = ("Type the column followed by the row (eg. a5). "
                   "To put or remove a flag, add 'f' to the cell (eg. a5f).")

    #showgrid(currgrid)
    print(helpmessage + " Type 'help' to show this message again.\n")

    while True:
        minesleft = numberofmines - len(flags)
        prompt = input('Enter the cell ({} mines left): '.format(minesleft))
        result = parseinput(prompt, gridsize, helpmessage + '\n')

        message = result['message']
        cell = result['cell']

        if cell:
            print('\n\n')
            rowno, colno = cell
            currcell = currgrid[rowno][colno]
            flag = result['flag']

            if not grid:
                grid, mines = setupgrid(gridsize, cell, numberofmines)
            if not starttime:
                starttime = time.time()

            if flag:
                # Add a flag if the cell is empty
                if currcell == ' ':
                    currgrid[rowno][colno] = 'F'
                    flags.append(cell)
                # Remove the flag if there is one
                elif currcell == 'F':
                    currgrid[rowno][colno] = ' '
                    flags.remove(cell)
                else:
                    message = 'Cannot put a flag there'

            # If there is a flag there, show a message
            elif cell in flags:
                message = 'There is a flag there'

            elif grid[rowno][colno] == 'X':
                print('Game Over\n')
                #showgrid(grid)
                if playagain():
                    playgame()
                return

            elif currcell == ' ':
                showcells(grid, currgrid, rowno, colno)

            else:
                message = "That cell is already shown"

            if set(flags) == set(mines):
                minutes, seconds = divmod(int(time.time() - starttime), 60)
                print(
                    'You Win. '
                    'It took you {} minutes and {} seconds.\n'.format(minutes,
                                                                      seconds))
                #showgrid(grid)
                if playagain():
                    playgame()
                return

        #showgrid(currgrid)
        print(message)

def startGameForSolver(gameData, gridsize = 9, numberofmines = 10):

    gameData.gameGridSize = gridsize
    gameData.gameNumMines = numberofmines
    gameData.gameCurrentGrid = [[' ' for i in range(gameData.gameGridSize)] for i in range(gameData.gameGridSize)]

    centerStr = ALPHABET[int(gridsize/2) - 1] + str(int(gridsize/2))
    center = parseinput(centerStr, gridsize, "")

    gameData.gameGrid, gameData.gameMines = setupgrid(gridsize, center["cell"], numberofmines)
    gameData.gameFlags = []

    #showgrid(gameData.gameCurrentGrid)

    minesleft = numberofmines - len(gameData.gameFlags)
    print('{} mines left: '.format(minesleft))
    return gameData

def makeMoveForSolver(gameData, move):

    print('Move made: {}'.format(move))
    print('\n\n')
    parsedMove = parseinput(move, gameData.gameGridSize, "")
    rowno, colno = parsedMove['cell']
    currcell = gameData.gameCurrentGrid[rowno][colno]
    flag = parsedMove['flag']
    message = ""

    if flag:
        # Add a flag if the cell is empty
        if currcell == ' ':
            gameData.gameCurrentGrid[rowno][colno] = 'F'
            gameData.gameFlagsList.append(parsedMove['cell'])
        # Remove the flag if there is one
        elif currcell == 'F':
            gameData.gameCurrentGrid[rowno][colno] = ' '
            gameData.gameFlagsList.remove(parsedMove['cell'])
        else:
            message = 'Cannot put a flag there'

    # If there is a flag there, show a message
    elif move in gameData.gameFlagsList:
        message = 'There is a flag there'

    elif gameData.gameGrid[rowno][colno] == 'X':
        print('Game Over\n')
        showgrid(gameData.gameGrid)
        gameData.gameState = 2
        return gameData

    elif currcell == ' ':
        showcells(gameData.gameGrid, gameData.gameCurrentGrid, rowno, colno)

    else:
        message = "That cell is already shown"

    coveredCells = gameData.CoveredCells()
    flaggedAndCovered = set()
    for flag in gameData.gameFlagsList :
        flaggedAndCovered.add(flag)
    for covered in coveredCells :
        flaggedAndCovered.add(covered)
    if flaggedAndCovered == set(gameData.gameMines):
        print('You Win.')
        showgrid(gameData.gameGrid)
        #if playagain():
        #    startGameForSolver()
        gameData.gameState = 1
        return gameData
    if(len(message) > 0) :
        print(message)
    #showgrid(gameData.gameCurrentGrid)
    return gameData

#playgame(13, 20)
