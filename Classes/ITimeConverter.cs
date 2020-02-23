using System;

namespace BerlinClock
{
    /// <summary>An interface describing a way to convert time to berlin clock time representation.</summary>
    public interface ITimeConverter
    {
        /// <summary>Converts a time to berlin clock time representation.</summary>
        /// <param name="timeToConvert">Time to convert.</param>
        /// <returns>Converted time.</returns>
        String ConvertTime(String timeToConvert);
    }
}
