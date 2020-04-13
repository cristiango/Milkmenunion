using System;

namespace MilkmenUnion
{
    public delegate DateTime GetUtcNow();

    public static class SystemClock
    {
        public static readonly GetUtcNow Default = () => DateTime.UtcNow;
        public static GetUtcNow Override(DateTime dateTime) => () => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
}