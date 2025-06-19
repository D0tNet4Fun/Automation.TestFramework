using System;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests.SourceGenerators;

public partial class TestCase_Complex_Description
{
    [Summary("Test steps can have complex descriptions")]
    public partial void Summary();

    [Input(1, """
              This
              description
              is
              multiline
              """)]
    private void Input()
    {
        // do nothing
    }

    [Input(2, """
              This description is 'single quoted' and "double quoted"
              """)]
    private void Input2()
    {
        // do nothing
    }

    [Input(3, @"This description contains a path: C:\Work")]
    private void Input3()
    {
        // do nothing
    }
}