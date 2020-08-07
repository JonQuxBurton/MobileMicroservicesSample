using System;

namespace Utils.DateTimes
{
    public class DateTimeCreator : IDateTimeCreator
    {
        public DateTime Create()
        {
            return DateTime.Now;
        }
    }
}
