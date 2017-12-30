using Humanizer;
using Xunit.Abstractions;

namespace Automation.TestFramework
{
    internal static class Extensions
    {
        public static string GetDisplayNameFromName(this IMethodInfo methodInfo)
            => methodInfo.Name.Humanize();
    }
}