using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Humanizer;

namespace Automation.TestFramework.Dynamic;

internal static class Helpers
{
    private static readonly Lazy<IReadOnlyDictionary<StepType, string>> CachedStepDisplayNames = new(() =>
    {
        return Enum.GetValues(typeof(StepType))
            .Cast<StepType>()
            .ToDictionary(s => s, s => s.Humanize());
    });
    
    public static string GetHumanizedName(this MemberInfo member)
    {
        return member.Name.Humanize();
    }

    public static string GetDisplayName(this StepType stepType)
    {
        return CachedStepDisplayNames.Value[stepType];
    }
}