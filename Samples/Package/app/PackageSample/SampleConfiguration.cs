using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;

namespace SDKTemplate;

public class Scenario
{
    public string Title { get; set; } = string.Empty;
    public Type ClassType { get; set; } = typeof(MainPage);
}

public partial class MainPage
{
    public const string FEATURE_NAME = "Package";

    public List<Scenario> Scenarios { get; } = new()
    {
        new Scenario() { Title = "Identity", ClassType = typeof(Scenario1) },
        new Scenario() { Title = "Installed Location", ClassType = typeof(Scenario2) },
        new Scenario() { Title = "Dependencies", ClassType = typeof(Scenario3) },
    };
}
