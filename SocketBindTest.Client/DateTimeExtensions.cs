namespace SocketBindTest.Client;

public static class DateTimeExtensions
{
    private static readonly DateTime StartedTime = new(1970, 1, 1, 8, 0, 0, 0);

    public static long ToTimeStamp13(this DateTime dateTime)
    {
        var timeSpan = dateTime - StartedTime;
        return Convert.ToInt64(timeSpan.TotalMilliseconds);
    }

    public static uint GetTimeStamp10(this DateTime dateTime)
    {
        return Convert.ToUInt32((dateTime - StartedTime).TotalSeconds);
    }
}