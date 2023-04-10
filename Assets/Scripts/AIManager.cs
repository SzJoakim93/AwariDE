using System.Linq;
using UnityEngine;

public enum Difficult
{
    Human = -1, Novice = 0, Rookie = 1, Standard = 2, Advanced = 3, SemiPro = 4, Pro = 5, Master = 7
};

public class AIManager
{
    public enum FindMode
    {
        Empty = 0, Emptyable = 1
    }

    int [,] pointsInTrays;
    int markAsEmptyable = -1;
    bool [] emptyableByEmeny;

    public AIManager(int [,] pointsInTrays)
    {
        this.pointsInTrays = pointsInTrays;
        emptyableByEmeny = new bool[6];
    }

    public int DetermineSelectedPlate(int turningPlayer)
    {
        switch(Global.difficult)
        {
            case Difficult.Novice:
                return DetermineNovice(turningPlayer);
            case Difficult.Rookie:
                return DetermineRookie(turningPlayer);
            case Difficult.Standard:
                return DetermineStandard(turningPlayer);
            case Difficult.Advanced:
                return DetermineAdvanced(turningPlayer);
            case Difficult.SemiPro:
                return DetermineSemiPro(turningPlayer);
            case Difficult.Pro:
                return DeterminePro(turningPlayer);
            case Difficult.Master:
                return DetermineMaster(turningPlayer);

                
        }
        return DetermineNovice(turningPlayer);
    }

    int DetermineNovice(int turningPlayer)
    {
        int enemyPlayer = turningPlayer == 0 ? 1: 0;
        if (pointsInTrays[turningPlayer, 6] < pointsInTrays[enemyPlayer, 6] - 10)
        {
            int ret = findLastDotOrEmptyPlateOrEmptyPlateInEnemy(turningPlayer);
            if (ret != -1)
            {
                return ret;
            }
        }

        return selectRandom(turningPlayer);
    }

    int DetermineRookie(int turningPlayer)
    {
        int enemyPlayer = turningPlayer == 0 ? 1: 0;
        if (pointsInTrays[turningPlayer, 6] < pointsInTrays[enemyPlayer, 6])
        {
            int ret = findLastDotOrEmptyPlateOrEmptyPlateInEnemy(turningPlayer);
            if (ret != -1)
            {
                return ret;
            }
        }

        return selectRandom(turningPlayer);
    }

    int DetermineStandard(int turningPlayer)
    {
        int ret = findLastDotOrEmptyPlateOrEmptyPlateInEnemy(turningPlayer);
        if (ret != -1)
        {
            return ret;
        }

        ret = findEmptyablePlate(turningPlayer);
        if (ret != -1)
        {
            return ret;
        }

        return selectRandom(turningPlayer);
    }

    int DetermineAdvanced(int turningPlayer)
    {
        int ret = findLastDotOrEmptyPlateOrEmptyPlateInEnemy(turningPlayer);
        if (ret != -1)
        {
            return ret;
        }

        ret = findEmptyablePlate(turningPlayer, true);
        if (ret != -1)
        {
            return ret;
        }

        return selectRandom(turningPlayer, true);
    }

    int DetermineSemiPro(int turningPlayer)
    {
        int ret = findLastDotOrEmptyPlateOrEmptyPlateInEnemy(turningPlayer, 50);
        if (ret != -1)
        {
            return ret;
        }

        ret = findEmptyablePlate(turningPlayer, true);
        if (ret != -1)
        {
            return ret;
        }

        return selectRandom(turningPlayer, true);
    }

    int DeterminePro(int turningPlayer)
    {
        int ret = findLastDotOrEmptyPlateOrEmptyPlateInEnemy(turningPlayer, 75, true);
        if (ret != -1)
        {
            return ret;
        }

        ret = findEmptyablePlate(turningPlayer, true);
        if (ret != -1)
        {
            return ret;
        }

        return selectRandom(turningPlayer, true);
    }

    int DetermineMaster(int turningPlayer)
    {
        int ret = findLastDotOrEmptyPlateOrEmptyPlateInEnemy(turningPlayer, 100, true, true);
        if (ret != -1)
        {
            return ret;
        }

        ret = findEmptyablePlate(turningPlayer, true);
        if (ret != -1)
        {
            return ret;
        }

        return selectRandom(turningPlayer, true);
    }

    int findLastDotOrEmptyPlateOrEmptyPlateInEnemy(int turningPlayer,
        int watchEmptyableByEnemy = 0,
        bool watchEmptyableSelectedPlateByEnemy = false,
        bool watchEmptyableByEnemyInNextStep = false)
    {
        int destPlayer1 = -1, destPlate1 = -1, destPlayer2 = -1, destPlate2 = -1, destPlayerInEnemy1 = -1, destPlateInEnemy1 = -1, destPlayerInEnemy2 = -1, destPlateInEnemy2 = -1;
        int dotToBigPlateIndex = findLastDotInBigPlate(turningPlayer);
        int emptyPlateIndex = findEmptyPlateWithDest(turningPlayer, ref destPlayer1, ref destPlate1, ref destPlayer2, ref destPlate2,
            watchEmptyableSelectedPlateByEnemy);

        
        if (dotToBigPlateIndex != -1 && (destPlayer1 != turningPlayer || destPlate1 < emptyPlateIndex))
        {
            return dotToBigPlateIndex;
        }

        if (watchEmptyableByEnemy > 0 && (watchEmptyableByEnemy == 100 || Random.Range(0.0f, 100.0f) < watchEmptyableByEnemy))
        {
            determineEmptyableByEnemy(turningPlayer);
            int enemyPlayer = turningPlayer == 0 ? 1 : 0;
            markAsEmptyable = findLastDotInBigPlate(enemyPlayer);
            int emptyPlateInEmnemyIndex = findEmptyPlateWithDest(enemyPlayer, ref destPlayerInEnemy1, ref destPlateInEnemy1, ref destPlayerInEnemy2, ref destPlateInEnemy2,
                watchEmptyableSelectedPlateByEnemy, watchEmptyableByEnemyInNextStep);

            if (emptyPlateInEmnemyIndex != -1 && emptyPlateIndex != -1
                && !between(destPlayerInEnemy1, destPlateInEnemy1, turningPlayer, emptyPlateIndex, destPlayer1, destPlate1)
                && !between(enemyPlayer, emptyPlateInEmnemyIndex, turningPlayer, emptyPlateIndex, destPlayer1, destPlate1)
                && pointsInTrays[destPlayerInEnemy2, destPlateInEnemy2] > pointsInTrays[destPlayer2, destPlate2])
            {
                return emptyPlateInEmnemyIndex;
            }
        }

        if (emptyPlateIndex != -1)
        {
            return emptyPlateIndex;
        }

        return -1;
    }

    /*int findLastDotOrEmptyPlate(int turningPlayer)
    {
        int destPlayer1 = 0, destPlate1 = 0, destPlayer2 = 0, destPlate2 = 0;
        int dotToBigPlateIndex = findLastDotInBigPlate(turningPlayer);
        int emptyPlateIndex = findEmptyPlateWithDest(turningPlayer, ref destPlayer1, ref destPlate1, ref destPlayer2, ref destPlate2);

        if (dotToBigPlateIndex != -1 && (destPlayer1 != turningPlayer || destPlate1 < emptyPlateIndex))
        {
            return dotToBigPlateIndex;
        }
        
        if (emptyPlateIndex != -1)
        {
            return emptyPlateIndex;
        }

        return -1;
    }*/

    int findLastDotInBigPlate(int x)
    {
        for (int i = 0; i < 6; i++)
        {
            int playerDest = x, plateDest = 0;
            stepPlateIndex(x, i, ref playerDest, ref plateDest);
            if (playerDest == x && plateDest == 6)
            {
                return i;
            }
        }

        return -1;
    }

    int findEmptyablePlate(int x, bool watchItCanFail = false)
    {
        int retIndex = -1, _playerDest1 = 0, _plateDest1 = 0, _playerDest2 = 0, _plateDest2 = 0;
        for (int i = 0; i < 6; i++)
        {
            if ((!watchItCanFail || !isReachableByEnemy(x, i)) && getEmptyPlate(x, i, ref _playerDest1, ref _plateDest1, ref _playerDest2, ref _plateDest2, FindMode.Emptyable))
            {
                retIndex = _plateDest1;
            }
        }

        return retIndex;
    }

    int findEmptyPlateWithDest(int x, ref int _playerDest1, ref int _plateDest1, ref int _playerDest2, ref int _plateDest2,
        bool watchEmptyableSelectedPlateByEnemy = false, bool watchEmptyableByEnemyInNextStep = false)
    {
        int retIndex = -1;
        for (int i = 0; i < 6; i++)
        {
            if ((!watchEmptyableSelectedPlateByEnemy || !isEmptyByEnemy(x, i))
                && getEmptyPlate(x, i, ref _playerDest1, ref _plateDest1, ref _playerDest2, ref _plateDest2, FindMode.Empty, watchEmptyableByEnemyInNextStep))
            {
                retIndex = i;
            }
        }

        return retIndex;
    }

    int selectRandom(int x, bool watchEmptyableByEnemyInNextStep = false)
    {
        int retValue;
        bool isExistsNotEmptyableByEnemy = emptyableByEmeny.Count(_ => false) < getNotEmptyPlates(x);
        do
        {
            retValue = Random.Range(0, 6);
        }
        while ((isExistsNotEmptyableByEnemy || !watchEmptyableByEnemyInNextStep || isEmptyByEnemy(x, retValue))
            && pointsInTrays[x, retValue] == 0);

        return retValue;
    }

    bool isEmptyByEnemy(int player, int plate)
    {
        int enemyPlayer = player == 0 ? 1 : 0;
        int playerDest = 0, plateDest = 0;
        for (int i = 0; i < 6; i++)
        {
            stepPlateIndex(enemyPlayer, i, ref playerDest, ref plateDest);
            if (playerDest == player && plate == plateDest)
            {
                return true;
            }
        }

        return false;
    }

    bool isReachableByEnemy(int player, int plate)
    {
        int enemyPlayer = player == 0 ? 1 : 0;
        int playerDest = 0, plateDest = 0;
        for (int i = 0; i < 6; i++)
        {
            stepPlateIndex(enemyPlayer, i, ref playerDest, ref plateDest);
            if (pointsInTrays[enemyPlayer, i] > 13 || (playerDest == player && plateDest >= plate))
            {
                return true;
            }
        }

        return false;
    }

    bool getEmptyPlate(int _playerSrc, int _plateSrc, ref int _playerDest1, ref int _plateDest1, ref int _playerDest2, ref int _plateDest2,
        FindMode findMode,
        bool watchEmptyableByEnemyInNextStep = false)
    {
        int playerDest = _playerSrc, plateDest = 0, maxValue = 0;
        stepPlateIndex(_playerSrc, _plateSrc, ref playerDest, ref plateDest);
        if (plateDest >= 7)
        {
            Debug.Log("Invalid index of plate: " + plateDest);
            return false;
        }

        if (findMode == FindMode.Empty &&  (pointsInTrays[playerDest, plateDest] == 0 || (watchEmptyableByEnemyInNextStep && playerDest == _playerSrc && plateDest == markAsEmptyable))
        || findMode == FindMode.Emptyable && playerDest == _playerSrc)
        {
            if (pointsInTrays[_playerSrc, _plateSrc] == 0 || plateDest == 6)
            {
                return false;
            }
            int playerDest2 = _playerSrc, plateDest2 = 0;
            
            stepToFrontOfCurrentPlate(playerDest, plateDest, ref playerDest2, ref plateDest2);
            if (plateDest >= 7)
            {
                Debug.Log("Invalid index of plate: " + plateDest2);
                return false;
            }

            if (pointsInTrays[playerDest2, plateDest2] > maxValue && (playerDest2 != _playerSrc || plateDest2 != _plateSrc))
            {
                maxValue = pointsInTrays[playerDest2, plateDest2];
                _playerDest1 = playerDest;
                _plateDest1 = plateDest;
                _playerDest2 = playerDest2;
                _plateDest2 = plateDest2;
                return true;
            }
        }

        return false;
    }

    void stepPlateIndex(int playerSrc, int plateSrc, ref int playerDest, ref int plateDest)
    {
        playerDest = playerSrc;
        plateDest = plateSrc;

        plateDest += pointsInTrays[playerSrc, plateSrc];

        if (plateDest > 20)
        {
            plateDest -= 21;
        }
        else if (plateDest > 13)
        {
            plateDest -= 14;
        }
        else if (plateDest > 6)
        {
            playerDest = playerDest == 0 ? 1 : 0;
            plateDest -= 7;
        }
    }

    void stepToFrontOfCurrentPlate(int playerSrc, int plateSrc, ref int playerDest, ref int plateDest)
    {
        playerDest = playerSrc;
        plateDest = plateSrc;
        playerDest = playerDest == 0 ? 1 : 0;
        plateDest = 5 - plateDest;
    }

    bool between(int px, int x, int pfrom, int from, int pto, int to)
    {
        if (px == pfrom && px == pto)
        {
            return x >= from && x <= to;
        }
        else if (px == pfrom)
        {
            return x >= from;
        }
        else // px == pto
        {
            return x <= to;
        }
    }

    void determineEmptyableByEnemy(int x)
    {
        for (int i = 0; i < 6; i++)
        {
            emptyableByEmeny[i] = isEmptyByEnemy(x, i);
        }
    }

    int getNotEmptyPlates(int x)
    {
        int sum = 0;
        for (int i = 0; i < 6; i++)
        {
            if (pointsInTrays[x, i] != 0)
            {
                sum++;
            }
        }

        return sum;
    }
}