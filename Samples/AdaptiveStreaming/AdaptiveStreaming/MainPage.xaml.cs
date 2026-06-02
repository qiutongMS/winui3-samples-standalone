using SDKTemplate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;

namespace SDKTemplate;

public sealed partial class MainPage : Page
{
    public const string FEATURE_NAME = "AdaptiveStreaming";

    public static MainPage? Current { get; private set; }

    private readonly List<Scenario> _scenarios = new()
    {
        new Scenario { Title = "Simplest Adaptive Streaming", ClassType = typeof(Scenario1_SimplestAdaptiveStreaming) },
        new Scenario { Title = "Event Handlers", ClassType = typeof(Scenario2_EventHandlers) },
        new Scenario { Title = "Network Request Modification", ClassType = typeof(Scenario3_RequestModification) },
        new Scenario { Title = "Adaptive Streaming Tuning", ClassType = typeof(Scenario4_Tuning) },
        new Scenario { Title = "Metadata", ClassType = typeof(Scenario5_Metadata) },
        new Scenario { Title = "Ad Insertion", ClassType = typeof(Scenario6_AdInsertion) },
        new Scenario { Title = "Live Seekable Range", ClassType = typeof(Scenario7_LiveSeekableRange) },
    };

    public static IReadOnlyList<AdaptiveContentModel> ContentManagementSystemStub = AdaptiveContentModel.GetKnownAzureMediaServicesModels();

    public static AdaptiveContentModel FindContentById(int id)
    {
        return ContentManagementSystemStub.First(model => model.Id == id);
    }

    public MainPage()
    {
        InitializeComponent();
        Current = this;
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item)
        {
            string? tag = item.Tag?.ToString();
            Type? pageType = tag switch
            {
                "Scenario1" => typeof(Scenario1_SimplestAdaptiveStreaming),
                "Scenario2" => typeof(Scenario2_EventHandlers),
                "Scenario3" => typeof(Scenario3_RequestModification),
                "Scenario4" => typeof(Scenario4_Tuning),
                "Scenario5" => typeof(Scenario5_Metadata),
                "Scenario6" => typeof(Scenario6_AdInsertion),
                "Scenario7" => typeof(Scenario7_LiveSeekableRange),
                _ => null
            };

            if (pageType != null)
            {
                ScenarioFrame.Navigate(pageType);
            }
        }
    }

    public void NotifyUser(string message, NotifyType type)
    {
        // Simple status notification - log to debug
        System.Diagnostics.Debug.WriteLine($"[{type}] {message}");
    }
}

public class Scenario
{
    public string Title { get; set; } = string.Empty;
    public Type ClassType { get; set; } = typeof(MainPage);
}

public enum NotifyType
{
    StatusMessage,
    ErrorMessage
}
