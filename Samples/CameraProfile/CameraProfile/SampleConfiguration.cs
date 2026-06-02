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

namespace SDKTemplate;

public partial class MainPage
{
    public const string FEATURE_NAME = "CameraProfile";

    List<Scenario> scenarios = new List<Scenario>
    {
        new Scenario() { Title="Locate Record Specific Profile", ClassType=typeof(CameraProfile.Scenario1_SetRecordProfile)},
        new Scenario() { Title="Query Profile for Concurrency", ClassType=typeof(CameraProfile.Scenario2_ConcurrentProfile)},
        new Scenario() { Title="Query Profile for HDR Support", ClassType=typeof(CameraProfile.Scenario3_EnableHdrProfile)}
    };
}

public class Scenario
{
    public string Title { get; set; } = string.Empty;
    public Type ClassType { get; set; } = typeof(object);
}

public enum NotifyType
{
    StatusMessage,
    ErrorMessage
}
