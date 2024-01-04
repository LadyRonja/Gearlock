
public class DataHandler
{
    private static DataHandler instance;

    public bool toggleZoom = false;
    public bool toggleClick = false;
    public bool toggleInverseCamera = false;

    public int musicVolume = 60;
    public int effectVolume = 60;


    public static DataHandler Instance
    {
        get { return GetInstance(); }
        private set { instance = value; }
    }
    private DataHandler()
    {
        if (instance == null || instance == this)
            instance = this;
    }

    public static DataHandler GetInstance()
    {
        if(instance != null)
            return instance;

        return new DataHandler();
    }
}
