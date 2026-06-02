using SDKTemplate.Models;
using SDKTemplate.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Web.Http;

namespace SDKTemplate.Controls;

public sealed partial class ContentSelector : UserControl
{
    public ContentSelector()
    {
        this.InitializeComponent();
    }

    private MediaPlayer? mediaPlayer;
    private IEnumerable<AdaptiveContentModel>? adaptiveContentModels;
    public AdaptiveContentModel? SelectedModel;
    private LogView? loggerControl;
    private Func<Uri, HttpClient?, Task<MediaPlaybackItem?>>? CreateMediaPlaybackItem;
    public MediaPlaybackItem? MediaPlaybackItem;
    public HttpClient? optionalHttpClient;

    internal void Initialize(MediaPlayer mediaPlayer,
        IEnumerable<AdaptiveContentModel> adaptiveContentModels,
        HttpClient? optionalHttpClient,
        LogView loggerControl,
        Func<Uri, HttpClient?, Task<MediaPlaybackItem?>> loadSourceFromUriAsync)
    {
        this.mediaPlayer = mediaPlayer ?? throw new ArgumentNullException(nameof(mediaPlayer));
        this.adaptiveContentModels = adaptiveContentModels ?? throw new ArgumentNullException(nameof(adaptiveContentModels));
        this.loggerControl = loggerControl ?? throw new ArgumentNullException(nameof(loggerControl));
        this.CreateMediaPlaybackItem = loadSourceFromUriAsync ?? throw new ArgumentNullException(nameof(loadSourceFromUriAsync));
        this.optionalHttpClient = optionalHttpClient;

        SelectedContent.ItemsSource = this.adaptiveContentModels;
        SetSelectedModel(this.adaptiveContentModels.First());
    }

    public void SetSelectedModel(AdaptiveContentModel model)
    {
        SelectedModel = model;
        SelectedContent.SelectedItem = SelectedModel;
    }

    public void HideLoadUri()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            LoadUriPanel.Visibility = Visibility.Collapsed;
        });
    }

    public void SetAutoPlay(bool isChecked)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            AutoPlay.IsChecked = isChecked;
        });
    }

    private async void LoadId_Click(object sender, RoutedEventArgs e)
    {
        if (mediaPlayer == null || adaptiveContentModels == null || loggerControl == null)
            return;

        SelectedModel = (AdaptiveContentModel)SelectedContent.SelectedItem;
        UriBox.Text = SelectedModel.ManifestUri.ToString();
        await CreateNewMediaPlaybackItemAsync(SelectedModel.ManifestUri);
    }

    private async void LoadUri_Click(object sender, RoutedEventArgs e)
    {
        if (mediaPlayer == null || adaptiveContentModels == null || loggerControl == null)
            return;

        if (!Uri.TryCreate(UriBox.Text, UriKind.Absolute, out Uri? uri))
        {
            loggerControl.Log("Malformed Uri in Load text box.", LogViewLoggingLevel.Critical);
            return;
        }
        await CreateNewMediaPlaybackItemAsync(uri);
    }

    private async Task CreateNewMediaPlaybackItemAsync(Uri uri)
    {
        SetSourceEnabled(false);
        MediaPlaybackItem = null;
        MediaPlaybackItem = await CreateMediaPlaybackItem!(uri, optionalHttpClient);
        if (MediaPlaybackItem != null)
        {
            loggerControl!.Log($"Loaded Uri: {uri}");
            SetSourceEnabled(true);
        }
    }

    private void SetSourceEnabled(bool enabled)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            SetSource.IsEnabled = enabled;
        });
    }

    private void AutoPlay_Checked(object sender, RoutedEventArgs e)
    {
        if (mediaPlayer == null) return;
        mediaPlayer.AutoPlay = (bool)(sender as CheckBox)!.IsChecked!;
    }

    private void SetSource_Click(object sender, RoutedEventArgs e)
    {
        if (mediaPlayer == null || adaptiveContentModels == null || loggerControl == null)
            return;

        mediaPlayer.Source = MediaPlaybackItem;
    }
}
