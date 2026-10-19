namespace FirstBackendApp.Extensions;

// Sana va vaqtni Toshkent vaqt mintaqasiga o'girish kengaytmasi
public static class DateTimeExtensions
{
    private static readonly TimeZoneInfo TashkentTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Tashkent");

    // UTC vaqtni Toshkent vaqtiga o'tkazish
    public static DateTime ToTashkentTime(this DateTime utcDateTime)
    {
        var safeUtc = utcDateTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc)
            : utcDateTime.ToUniversalTime();
        
        return TimeZoneInfo.ConvertTimeFromUtc(safeUtc, TashkentTimeZone);
    }
}