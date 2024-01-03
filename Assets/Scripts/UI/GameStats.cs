
public class GameStats
{
    private static GameStats instance;
    public static GameStats Instance { get => GetInstance(); private set => instance = value; }

    private int turnsTaken = 1;
    private int cardsPlayed = 0;
    private int rocksMined = 0;
    private int damageDealt = 0;

    private int damageTaken = 0;
    private int robotsLost = 0;

    private GameStats() { 
        if(instance == null)
        {
            instance = this;
            ResetStats();
        }
    }

    public static GameStats GetInstance()
    {
        if(instance != null)
            return instance;

        instance = new GameStats();
        return instance;
    }

    public void ResetStats()
    {
        turnsTaken = 1;
        cardsPlayed = 0;
        rocksMined = 0;
        damageDealt = 0;

        damageTaken = 0;
        robotsLost = 0;
    }
    public void IncreaseTurnsTaken()
    {
        turnsTaken++;
    }
    public void IncreaseCardsPlayed()
    {
        cardsPlayed++;
    }
    public void IncreaseRocksMined()
    {
        rocksMined++;
    }
    public void IncreaseDamageDealt(int amount)
    {
        if (amount > 0)
            damageDealt += amount;
    }
    public void IncreaseDamageTaken(int amount)
    {
        if(amount> 0) 
            damageTaken += amount;
    }
    public void IncreaseRobotsLost()
    {
        robotsLost++;
    }


    public int GetTurnsTaken()
    {
        return turnsTaken;
    }
    public int GetCardsPlayed()
    {
        return cardsPlayed;
    }
    public int GetRocksMined()
    {
        return rocksMined;
    }
    public int GetDamageDealt()
    {
        return damageDealt;
    }
    public int GetDamageTaken()
    {
        return damageTaken;
    }
    public int GetRobotsLost()
    {
        return robotsLost;
    }
}
