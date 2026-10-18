namespace LiveYourLife.Common;

public static class GameTimeExtensions
{
    public static string AsGameTimeString(this float gameTime)
    {
        return SessionData.Instance.ShortDateString(gameTime, true) + ' ' + SessionData.Instance.GameTimeToClock24String(gameTime, true);
    }
}