//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

namespace PackageSample;

public partial class MainPage
{
    public const string FEATURE_NAME = "Package";

    List<Scenario> scenarios = new List<Scenario>
    {
        new Scenario() { Title = "Identity", ClassType = typeof(Scenario1) },
        new Scenario() { Title = "Installed Location", ClassType = typeof(Scenario2) },
        new Scenario() { Title = "Dependencies", ClassType = typeof(Scenario3) },
    };
}

public class Scenario
{
    public string Title { get; set; } = string.Empty;
    public Type ClassType { get; set; } = typeof(object);
}
