using System.Globalization;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing
{
    static class ITextEventSourceExtensions
    {
        internal static void WriteError(this ITextEventSource eventSource, string type, string message) =>
            eventSource.ErrorText(string.Empty, type, message);

        internal static void WriteError(this ITextEventSource eventSource, string type, string format, params object[] args) =>
            eventSource.ErrorText(string.Empty, type, Message(format, args));

        internal static void WriteErrorWithId(this ITextEventSource eventSource, string type, string id, string message) =>
            eventSource.ErrorText(id, type, message);

        internal static void WriteErrorWithId(this ITextEventSource eventSource, string type, string id, string format, params object[] args) =>
            eventSource.ErrorText(id, type, Message(format, args));

        internal static void WriteInfo(this ITextEventSource eventSource, string type, string message) =>
            eventSource.InfoText(string.Empty, type, message);

        internal static void WriteInfo(this ITextEventSource eventSource, string type, string format, params object[] args) =>
            eventSource.InfoText(string.Empty, type, Message(format, args));

        internal static void WriteInfoWithId(this ITextEventSource eventSource, string type, string id, string message) =>
            eventSource.InfoText(id, type, message);

        internal static void WriteInfoWithId(this ITextEventSource eventSource, string type, string id, string format, params object[] args) =>
            eventSource.InfoText(id, type, Message(format, args));

        internal static void WriteNoise(this ITextEventSource eventSource, string type, string message) =>
            eventSource.NoiseText(string.Empty, type, message);

        internal static void WriteNoise(this ITextEventSource eventSource, string type, string format, params object[] args) =>
            eventSource.NoiseText(string.Empty, type, Message(format, args));

        internal static void WriteNoiseWithId(this ITextEventSource eventSource, string type, string id, string message) =>
            eventSource.NoiseText(id, type, message);

        internal static void WriteNoiseWithId(this ITextEventSource eventSource, string type, string id, string format, params object[] args) =>
            eventSource.NoiseText(id, type, Message(format, args));

        internal static void WriteWarning(this ITextEventSource eventSource, string type, string message) =>
            eventSource.WarningText(string.Empty, type, message);

        internal static void WriteWarning(this ITextEventSource eventSource, string type, string format, params object[] args) =>
            eventSource.WarningText(string.Empty, type, Message(format, args));

        internal static void WriteWarningWithId(this ITextEventSource eventSource, string type, string id, string message) =>
            eventSource.WarningText(id, type, message);

        internal static void WriteWarningWithId(this ITextEventSource eventSource, string type, string id, string format, params object[] args) =>
            eventSource.WarningText(id, type, Message(format, args));

        static string Message(string format, object[] args) =>
            string.Format(CultureInfo.InvariantCulture, format, args);
    }
}
