namespace Infra.Helpers
{
    /// <summary>Interface used to do basic instrumentation.</summary>
    public interface ILog
    {
        /// <summary>Logs an error.</summary>
        /// <param name="msg">The message to log.</param>
        void Error(string msg);

        /// <summary>Logs an error.</summary>
        /// <param name="msg">The message to log.</param>
        void Debug(string msg);
    }
}
