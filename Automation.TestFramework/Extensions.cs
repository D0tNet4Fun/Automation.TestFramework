using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Automation.TestFramework.Entities;
using Humanizer;
using Xunit;
using Xunit.Abstractions;

namespace Automation.TestFramework
{
    internal static class Extensions
    {
        public static string GetDisplayNameFromName(this IMethodInfo methodInfo)
            => methodInfo.Name.Humanize();

        public static IEnumerable<TestClassDependency> GetDependencies(this ITypeInfo @class)
        {
            // check if the class has a base class
            var baseClass = @class.BaseType;
            if (baseClass.ToRuntimeType() != typeof(object))
            {
                yield return new TestClassDependency { Type = DependencyType.Inheritance, Class = baseClass };
            }

            // check if the class implements interfaces of type IClassFixture
            var classFixtures =
                from @interface in @class.Interfaces
                where @interface.IsGenericType
                let genericTypeDefinition = @interface.ToRuntimeType().GetGenericTypeDefinition()
                where typeof(IClassFixture<>).IsAssignableFrom(genericTypeDefinition)
                let fixtureClass = @interface.GetGenericArguments().ToArray()[0]
                select fixtureClass;

            foreach (var classFixture in classFixtures)
            {
                yield return new TestClassDependency { Type = DependencyType.Aggregation, Class = classFixture };
            }
        }

        public static string GetMessage(this AggregateException aggregateException, string separator)
            => separator + string.Join(separator, aggregateException.InnerExceptions.Select(e => e.Message));

        public static Task ForEachAsync<T>(this IEnumerable<T> source, ParallelOptions parallelOptions, Func<T, Task> body)
        {
            // based on https://blogs.msdn.microsoft.com/pfxteam/2012/03/05/implementing-a-simple-foreachasync-part-2/
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(parallelOptions.MaxDegreeOfParallelism)
                select Task.Run(async () =>
                {
                    using (partition)
                        while (partition.MoveNext())
                        {
                            parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                            await body(partition.Current);
                        }
                }, parallelOptions.CancellationToken));
        }
    }
}