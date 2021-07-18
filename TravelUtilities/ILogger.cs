namespace Travel.Core
{
    /// <summary>
    /// Esta interfaz define un logger.
    /// </summary>
    public interface ILogger
    {
        public string Message { set; get; }
        public string StackMessage { set; get; }
    }
}
