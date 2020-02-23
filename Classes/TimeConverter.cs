using Infra.Helpers;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BerlinClock
{
    /// <summary>A class to convert time to berlin clock time representation</summary>
    /// <seealso cref="BerlinClock.ITimeConverter" />
    public class TimeConverter : ITimeConverter
    {
        /// <summary>Index of the Hours in time array.</summary>
        private const int _Hours = 0;
        /// <summary>Index of the Minutes in time array.</summary>
        private const int _Minutes = 1;
        /// <summary>Index of the Seconds in time array.</summary>
        private const int _Seconds = 2;

        /// <summary>Interval used to highlight quarter of an hour in the main minute stripe.</summary>
        private const int _MainMinuteStripeHighlightInterval = 3;

        /// <summary>Error message for unsupported time format.</summary>
        private const string InvalidTimeFormat = "Time must be a string between 0:00:00 (or 00:00:00) and 24:59:59.";

        /// <summary>Logger used to realize basic instrumentation.</summary>
        private readonly ILog _Log;

        /// <summary>Initializes a new instance of the <see cref="TimeConverter"/> class.</summary>
        /// <param name="log">The log dependency.</param>
        public TimeConverter(ILog log)
        {
            _Log = log;
        }

        /// <summary>Converts a time to berlin clock time representation.</summary>
        /// <param name="timeToConvert">Time to convert.</param>
        /// <returns>Converted time.</returns>
        /// <exception cref="System.Exception"></exception>
        public string ConvertTime(string timeToConvert)
        {
            // Validates argument: Throw an exception if time representation is not supported.
            if (!CheckTimeFormatIsSupported(timeToConvert))
            {
                _Log.Error(InvalidTimeFormat);
                throw new Exception(InvalidTimeFormat);
                // Maybe we could returns a completely dark clock instead:
                // return ConvertTime("0:00:01");
            }

            // Converts time into a 3 parts int array: index 0: Time, index 1: Minutes, index 2: Seconds (see constants defined at the top).
            int[] TimeParts = timeToConvert.Split(':').Select(c => int.Parse(c)).ToArray();

            // Build a berlin clock string representation.
            string Result = GetBerlinClock(TimeParts);

            // Traces Result.
            _Log.Debug($"Result: {Result}");
            return Result;
        }

        /// <summary>
        /// Gets the berlin clock string representation.
        /// </summary>
        /// <param name="timeParts">The time to convert.</param>
        /// <returns>A formated string according to berlin clock specs.</returns>
        private string GetBerlinClock(int[] timeParts)
        {
            // Prepares to dynamically creates a 32 characters long string result (including crlf)
            StringBuilder Builder = new StringBuilder(32);

            // Build Berlin clock seconds stripe, with 1 lamp representing 2 units as a substripe..
            BuildBerlinClockStripe(Builder, 1, 2, timeParts[_Seconds], true, GetInactiveIndicator, GetActiveMinuteAndSecondIndicator).AppendLine();
            // Build Berlin clock hours main stripe, with 4 lamps representing 5 units as a main stripe.
            BuildBerlinClockStripe(Builder, 4, 5, timeParts[_Hours], false, GetActiveHourIndicator, GetInactiveIndicator).AppendLine();
            // Build Berlin clock hours sub stripe, with 4 lamps representing 1 units as a main stripe.
            BuildBerlinClockStripe(Builder, 4, 5, timeParts[_Hours], true, GetActiveHourIndicator, GetInactiveIndicator).AppendLine();
            // Build Berlin clock minutes main stripe, with 11 lamps representing 5 units as a main stripe.
            BuildBerlinClockStripe(Builder, 11, 5, timeParts[_Minutes], false, GetActiveMinuteMainStripeIndicator, GetInactiveIndicator).AppendLine();
            // Build Berlin clock minutes sub stripe, with 4 lamps representing 5 units as a main stripe.
            BuildBerlinClockStripe(Builder, 4, 5, timeParts[_Minutes], true, GetActiveMinuteAndSecondIndicator, GetInactiveIndicator);

            return Builder.ToString();
        }

        /// <summary>
        /// Builds a berlin clock stripe according to passed arguments.
        /// </summary>
        /// <param name="builder">The string builder to use.</param>
        /// <param name="lampsCountInStripe">The lamps count in stripe.</param>
        /// <param name="unitPerLamp">The unit per lamp.</param>
        /// <param name="value">The value to display on stripe.</param>
        /// <param name="isSubStripe">if set to <c>true</c> the stripe is a sub stripe.</param>
        /// <param name="getActiveIndicator">Delegate defining the active indicator for a lamp.</param>
        /// <param name="getInactiveIndicator">Delegate defining the inactive indicator for a lamp.</param>
        /// <returns>The passed String Builder enabling chained calls.</returns>
        private StringBuilder BuildBerlinClockStripe(StringBuilder builder, int lampsCountInStripe, int unitPerLamp, int value, bool isSubStripe, Func<int, char> getActiveIndicator, Func<int, char> getInactiveIndicator)
        {
            // Calculates how many lamps are active in stripe.
            // On a main stripe we divide the value to display by the unit count represented by a single lamp.
            // On a sub stripe we calculate the remainder of the division of the value to display by the unit count represented by a single lamp.
            int ActiveLamps = isSubStripe ? value % unitPerLamp : value / unitPerLamp;

            // Browses all stripe's lamps and switch on lamps included in the active lamps count.
            for (int LampIndex = 0; LampIndex < lampsCountInStripe; LampIndex++)
            {
                builder.Append(LampIndex < ActiveLamps ? getActiveIndicator(LampIndex) : getInactiveIndicator(LampIndex));
            }

            return builder;
        }

        /// <summary>
        /// Gets the active indicator for hours according to lamp position.
        /// For Berlin clock hour indicator is always red.
        /// </summary>
        /// <param name="lampIndex">Index of the lamp.</param>
        /// <returns>The active hour indicator.</returns>
        private char GetActiveHourIndicator(int lampIndex)
        {
            return 'R';
        }

        /// <summary>
        /// Gets the active indicator for minute sub stripe and seconds stripe according to lamp position.
        /// For Berlin clock minute sub stripe and seconds stripe indicator is always yellow.
        /// </summary>
        /// <param name="lampIndex">Index of the lamp.</param>
        /// <returns>The yellow lamp color.</returns>
        private char GetActiveMinuteAndSecondIndicator(int lampIndex)
        {
            return 'Y';
        }

        /// <summary>
        /// Gets the inactive indicator for a lamp according to lamp position.
        /// For Berlin clock inactive indicator is always.
        /// </summary>
        /// <param name="lampIndex">Index of the lamp.</param>
        /// <returns>The inactive lamp indicator.</returns>
        private char GetInactiveIndicator(int lampIndex)
        {
            return 'O';
        }

        /// <summary>
        /// Gets the active indicator in the minute main stripe according to lamp position.
        /// For Berlin clock minute main stripe active indicator is Yellow or Red on quarter.
        /// </summary>
        /// <param name="lampIndex">Index of the lamp.</param>
        /// <returns>The active indicator for main minute stripe lamps.</returns>
        private char GetActiveMinuteMainStripeIndicator(int lampIndex)
        {
            return (lampIndex + 1) % _MainMinuteStripeHighlightInterval == 0 ? 'R' : 'Y';
        }

        /// <summary>Checks the passed time string format is supported by this time converter.</summary>
        /// <param name="aTime">Time to convert.</param>
        /// <returns>True is passed time string is supported, otherwise returns false.</returns>
        private bool CheckTimeFormatIsSupported(string aTime)
        {
            // Uses regex to check argument string represents a time between 0:00:00 (or 00:00:00) and 24:59:59
            // (upper limit set to 24:59:59 to be able to handle required use case where midnight can be 24:00:00)
            Match SupportedTimeFormatTest = Regex.Match(aTime, "^(?:0?[0-9]|1[0-9]|2[0-4]):[0-5][0-9]:[0-5][0-9]$");
            return SupportedTimeFormatTest.Success;
        }
    }
}
