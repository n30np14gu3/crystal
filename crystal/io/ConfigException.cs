using System;

namespace crystal.io
{
    /// <summary>
    /// Base exception for class Config
    /// </summary>
    /// <see cref="Config"/>
    public class ConfigException : Exception
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="message">Message of exception</param>
        public ConfigException(string message = "") : base(message)
        {

        }
    }
}