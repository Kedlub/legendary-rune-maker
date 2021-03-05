﻿using Anotar.Log4Net;
using Legendary_Rune_Maker.Data;
using Legendary_Rune_Maker.Game;
using Legendary_Rune_Maker.Locale;
using Legendary_Rune_Maker.Pages;
using Ninject;
using Ninject.Parameters;
using Onova;
using Onova.Models;
using Onova.Services;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Legendary_Rune_Maker
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public bool IsClosed { get; private set; }

        private bool Shown;
        private CancellationTokenSource CancelSource = new CancellationTokenSource();

        private readonly Lazy<MainWindow> MainWindow;
        private readonly Lazy<MainPage> MainPage;
        private readonly ITeamGuesser TeamGuesser;

        public LoadingWindow(Lazy<MainWindow> mainWindow, Lazy<MainPage> mainPage, ITeamGuesser teamGuesser)
        {
            this.MainWindow = mainWindow;
            this.MainPage = mainPage;
            this.TeamGuesser = teamGuesser;

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            InitializeComponent();
        }

        private void ShowMainWindow()
        {
            if (Shown)
                return;
            Shown = true;

            MainWindow.Value.SetMainPage(MainPage.Value);

            this.IsClosed = true;
            this.Close();
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
#if !DEBUG
            if (Config.Current.CheckUpdatesBeforeStartup)
                await CheckUpdates();
#endif

            await LoadCache();

            ShowMainWindow();
        }

        private async Task LoadCache()
        {
            Progress.IsIndeterminate = false;
            Progress.Value = 0;
            Status.Text = Text.Loading;
            Cancel.Visibility = Visibility.Hidden;
            Hint.Visibility = Visibility.Visible;

            WebCache.Init();
            if (WebCache.CacheGameVersion != await Riot.GetLatestVersionAsync()
                || WebCache.CacheLocale != CultureInfo.CurrentCulture.Name)
            {
                LogTo.Info("Clearing web cache due to a new LoL version being available, or a language change");
                WebCache.Clear();

                WebCache.CacheGameVersion = await Riot.GetLatestVersionAsync();
                WebCache.CacheLocale = CultureInfo.CurrentCulture.Name;
            }

            Riot.SetLanguage(Config.Current.Culture);
            await Riot.CacheAllAsync(o => Dispatcher.Invoke(() => Progress.Value = o * 0.7));
            await TeamGuesser.Load(new Progress<float>(o => Dispatcher.Invoke(() => Progress.Value = 0.7 + (o * 0.3))));
        }

        private async Task CheckUpdates()
        {
            Status.Text = Text.CheckingUpdates;
            Progress.IsIndeterminate = true;

            var manager = new UpdateManager(
                new GithubPackageResolver("pipe01", "legendary-rune-maker", "*.zip"),
                new ZipPackageExtractor());

            var update = await await Task.WhenAny(
                    Task.Delay(3000000).ContinueWith<CheckForUpdatesResult>(_ => null),
                    manager.CheckForUpdatesAsync());

            if (update?.CanUpdate == true)
            {
                Status.Text = Text.Updating;
                Cancel.Visibility = Visibility.Visible;
                Progress.IsIndeterminate = false;

                var progress = new Progress<double>(o => Progress.Value = o);

                try
                {
                    await manager.PrepareUpdateAsync(update.LastVersion, progress, CancelSource.Token);
                }
                catch (TaskCanceledException)
                {
                }

                if (manager.IsUpdatePrepared(update.LastVersion))
                {
                    manager.LaunchUpdater(update.LastVersion);
                    Environment.Exit(0);
                }
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            CancelSource.Cancel();
            Cancel.IsEnabled = false;
        }
    }
}
