using System;

namespace Infra.Helpers
{
    /// <summary>
    /// The Log class is an implementation of the ILog interface to realize basic instrumentation.
    /// A production environment would probably encapsulate an existing logging framework (ex: Log4Net, NLog, etc.)
    /// </summary>
    /// <seealso cref="Infra.Helpers.ILog" />
    public class Log : ILog
    {
        
        /// <summary>Logs an error.</summary>
        /// <param name="msg">The message to log.</param>
        public void Debug(string msg)
        {
            Console.WriteLine(msg);
        }

        /// <summary>Logs an error.</summary>
        /// <param name="msg">The message to log.</param>
        public void Error(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
