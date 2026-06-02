using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace AssociationLaunching;

public sealed partial class Scenario3_ReceiveFile : Page
{
    private readonly MainPage _rootPage = MainPage.Current;
    private readonly string Extension = "alsdkcs";

    public Scenario3_ReceiveFile()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is IFileActivatedEventArgs args)
        {
            string output = "Files received: " + args.Files.Count + "\n";
            foreach (StorageFile file in args.Files)
            {
                output += file.Name + "\n";
            }

            if (args is IFileActivatedEventArgsWithNeighboringFiles neighboringArgs && neighboringArgs.NeighboringFilesQuery != null)
            {
                IReadOnlyList<StorageFile> neighboringFiles = await neighboringArgs.NeighboringFilesQuery.GetFilesAsync();
                if (neighboringFiles.Count > 0)
                {
                    output += "\nNeighboring files: " + neighboringFiles.Count + "\n";
                    int i;
                    for (i = 0; i < Math.Min(neighboringFiles.Count, 3); i++)
                    {
                        output += neighboringFiles[i].Name + "\n";
                    }
                    int remaining = neighboringFiles.Count - i;
                    if (remaining > 0)
                    {
                        output += "and " + remaining + " more.";
                    }
                }
            }
            _rootPage.NotifyUser(output, NotifyType.StatusMessage);
        }
    }

    private async void CreateTestFile()
    {
        StorageFolder folder = await KnownFolders.GetFolderAsync(KnownFolderId.PicturesLibrary);
        await folder.CreateFileAsync("Test " + Extension + " file." + Extension, CreationCollisionOption.ReplaceExisting);
        await Windows.System.Launcher.LaunchFolderAsync(folder);
    }

    private async void CreateTestFileWithNoExtension()
    {
        StorageFolder folder = await KnownFolders.GetFolderAsync(KnownFolderId.PicturesLibrary);
        await folder.CreateFileAsync("Test file with no extension", CreationCollisionOption.ReplaceExisting);
        await Windows.System.Launcher.LaunchFolderAsync(folder);
    }

    private async void RemoveTestFiles()
    {
        StorageFolder folder = await KnownFolders.GetFolderAsync(KnownFolderId.PicturesLibrary);
        try
        {
            StorageFile file = await folder.GetFileAsync("Test " + Extension + " file." + Extension);
            await file.DeleteAsync();
        }
        catch (Exception)
        {
            // File I/O errors are reported as exceptions. Ignore errors here.
        }

        try
        {
            StorageFile file = await folder.GetFileAsync("Test file with no extension");
            await file.DeleteAsync();
        }
        catch (Exception)
        {
            // File I/O errors are reported as exceptions. Ignore errors here.
        }
    }
}