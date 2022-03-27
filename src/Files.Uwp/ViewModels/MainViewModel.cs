﻿using Files.Dialogs;
using Files.Helpers;
using Files.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using CommunityToolkit.Mvvm.DependencyInjection;
using Files.Backend.Services;
using Files.Backend.ViewModels.Dialogs;
using Files.Backend.Extensions;
using Windows.UI.Xaml.Hosting;
using Files.Extensions;

namespace Files.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public IPaneViewModel PaneViewModel { get; } = new PaneViewModel();

        public MainViewModel()
        {
            Clipboard.ContentChanged += Clipboard_ContentChanged;

            DetectFontName();
        }

        public void Clipboard_ContentChanged(object sender, object e)
        {
            try
            {
                // Clipboard.GetContent() will throw UnauthorizedAccessException
                // if the app window is not in the foreground and active
                DataPackageView packageView = Clipboard.GetContent();
                if (packageView.Contains(StandardDataFormats.StorageItems) || packageView.Contains(StandardDataFormats.Bitmap))
                {
                    IsPasteEnabled = true;
                }
                else
                {
                    IsPasteEnabled = false;
                }
            }
            catch
            {
                IsPasteEnabled = false;
            }
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            IsWindowCompactSize = IsWindowResizedToCompactWidth();
        }

        private int tabStripSelectedIndex = 0;

        public int TabStripSelectedIndex
        {
            get => tabStripSelectedIndex;
            set
            {
                if (value >= 0 && tabStripSelectedIndex != value)
                {
                    SetProperty(ref tabStripSelectedIndex, value);
                }
            }
        }

        private bool isFullTrustElevated = false;

        public bool IsFullTrustElevated
        {
            get => isFullTrustElevated;
            set => SetProperty(ref isFullTrustElevated, value);
        }

        private bool isPasteEnabled = false;

        public bool IsPasteEnabled
        {
            get => isPasteEnabled;
            set => SetProperty(ref isPasteEnabled, value);
        }

        private bool isWindowCompactSize = IsWindowResizedToCompactWidth();

        public static bool IsWindowResizedToCompactWidth()
        {
            return false; // Window.Currentt.Bounds.Width <= 750;
        }

        public bool IsWindowCompactSize
        {
            get => isWindowCompactSize;
            set
            {
                SetProperty(ref isWindowCompactSize, value);
            }
        }

        private bool multiselectEnabled;

        public bool MultiselectEnabled
        {
            get => multiselectEnabled;
            set => SetProperty(ref multiselectEnabled, value);
        }

        public bool IsQuickLookEnabled { get; set; }

        private FontFamily fontName;

        public FontFamily FontName
        {
            get => fontName;
            set => SetProperty(ref fontName, value);
        }

        private void DetectFontName()
        {
            var rawVersion = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
            var currentVersion = new Version((int)((rawVersion & 0xFFFF000000000000) >> 48), (int)((rawVersion & 0x0000FFFF00000000) >> 32), (int)((rawVersion & 0x00000000FFFF0000) >> 16), (int)(rawVersion & 0x000000000000FFFF));
            var newIconsMinVersion = new Version(10, 0, 21327, 1000);
            bool isRunningNewIconsVersion = currentVersion >= newIconsMinVersion;

            if (isRunningNewIconsVersion)
            {
                FontName = new FontFamily("Segoe Fluent Icons");
            }
            else
            {
                FontName = new FontFamily("Segoe MDL2 Assets");
            }
        }
    }
}