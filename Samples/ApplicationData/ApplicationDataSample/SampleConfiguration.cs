//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;

namespace ApplicationDataSample;

public class Scenario
{
    public string Title { get; set; } = string.Empty;
    public Type ClassType { get; set; } = typeof(object);

    public override string ToString()
    {
        return Title;
    }
}

public static class SampleConfiguration
{
    public const string FEATURE_NAME = "ApplicationData";

    public static List<Scenario> Scenarios { get; } = new List<Scenario>
    {
        new Scenario() { Title = "Files", ClassType = typeof(Scenario1_Files) },
        new Scenario() { Title = "Settings", ClassType = typeof(Scenario2_Settings) },
        new Scenario() { Title = "Setting Container", ClassType = typeof(Scenario3_SettingContainer) },
        new Scenario() { Title = "Composite Settings", ClassType = typeof(Scenario4_CompositeSettings) },
        new Scenario() { Title = "Clear", ClassType = typeof(Scenario6_ClearScenario) },
        new Scenario() { Title = "SetVersion", ClassType = typeof(Scenario7_SetVersion) },
    };
}
