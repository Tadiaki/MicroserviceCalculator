using System.Runtime.CompilerServices;
using ILogger = Serilog.ILogger;


namespace SubtractService.Monitoring
{
    public static class LoggerExtensions
    {
        public static ILogger Here(this ILogger logger,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)

        {
            return logger
                .ForContext("MemberName", memberName)
                .ForContext("FilePath", sourceFilePath)
                .ForContext("LineNumber", sourceLineNumber)
                .ForContext("MachineName", Environment.MachineName);
        }
    }
}
