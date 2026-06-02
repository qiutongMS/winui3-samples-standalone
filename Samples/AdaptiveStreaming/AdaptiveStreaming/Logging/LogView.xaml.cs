using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace SDKTemplate.Logging;

public enum LogViewLoggingLevel
{
    Verbose = 0,
    Information,
    Warning,
    Error,
    Critical,
    Always,
}

public sealed partial class LogView : UserControl
{
    public LogView()
    {
        this.InitializeComponent();

        LoggingBrushes = new SolidColorBrush[] {
            (SolidColorBrush)Resources["VerboseLogBrush"],
            (SolidColorBrush)Resources["InformationLogBrush"],
            (SolidColorBrush)Resources["WarningLogBrush"],
            (SolidColorBrush)Resources["ErrorLogBrush"],
            (SolidColorBrush)Resources["CriticalLogBrush"],
            (SolidColorBrush)Resources["AlwaysLogBrush"],
        };

        LogLevelComboBox.SelectedIndex = (int)LoggingLevel;
    }

    public static readonly DependencyProperty LogLevelProperty =
        DependencyProperty.Register("LoggingLevel", typeof(LogViewLoggingLevel), typeof(LogView),
            new PropertyMetadata(LogViewLoggingLevel.Warning, OnLoggingLevelChanged));

    public LogViewLoggingLevel LoggingLevel
    {
        get { return (LogViewLoggingLevel)GetValue(LogLevelProperty); }
        set { SetValue(LogLevelProperty, value); }
    }

    private static void OnLoggingLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var logger = (LogView)d;
        var loggingLevel = (LogViewLoggingLevel)e.NewValue;
        logger.LogLevelComboBox.SelectedIndex = (int)loggingLevel;
    }

    private bool _isLogViewFull = false;
    private int _maxItemsInLogView = 500;
    private SolidColorBrush[] LoggingBrushes;

    private void LogLevel_Changed(object sender, SelectionChangedEventArgs e)
    {
        LoggingLevel = (LogViewLoggingLevel)LogLevelComboBox.SelectedIndex;
    }

    public void Log(string message, LogViewLoggingLevel level = LogViewLoggingLevel.Always)
    {
        if (DispatcherQueue.HasThreadAccess)
        {
            LogFromUIThread(message, level);
        }
        else
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                LogFromUIThread(message, level);
            });
        }
    }

    private void LogFromUIThread(string message, LogViewLoggingLevel level)
    {
        string logEntry = $"{DateTime.Now:HH:mm:ss.fff} - {message}";

        if (LoggingLevel <= level)
        {
            SolidColorBrush foregroundBrush = LoggingBrushes[(int)level];

            if (_isLogViewFull)
            {
                System.Diagnostics.Debug.WriteLine(logEntry);
                return;
            }

            if (LoggingListBox.Items.Count >= _maxItemsInLogView)
            {
                _isLogViewFull = true;
                System.Diagnostics.Debug.WriteLine(logEntry);
                logEntry = "Log window is full";
                foregroundBrush = new SolidColorBrush(Colors.Red);
            }

            LoggingListBox.Items.Insert(0, new TextBlock() { Text = logEntry, Foreground = foregroundBrush });
        }
        else if (level != LogViewLoggingLevel.Verbose)
        {
            System.Diagnostics.Debug.WriteLine($"{level}: {logEntry}");
        }
    }

    private void CopyLogToClipboard_Click(object sender, RoutedEventArgs e)
    {
        var content = new DataPackage();
        content.SetText(string.Join("\r\n", LoggingListBox.Items.Select(item => (item as TextBlock)?.Text ?? "")));
        Clipboard.SetContent(content);
    }

    private void ClearLog_Click(object sender, RoutedEventArgs e)
    {
        ClearLog();
    }

    public void ClearLog()
    {
        if (DispatcherQueue.HasThreadAccess)
        {
            LoggingListBox.Items.Clear();
            _isLogViewFull = false;
        }
        else
        {
            DispatcherQueue.TryEnqueue(ClearLog);
        }
    }
}
