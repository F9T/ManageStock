﻿using Application.Common.Settings;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Application.Common
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : CustomWindow
    {
        private SettingsBase m_SaveSettings;
        private bool m_ManualClosing = true;

        public SettingsWindow(Settings.Settings _Settings)
        {
            NeedRestart = false;
            m_SaveSettings = (Settings.Settings)_Settings.Clone();
            Settings = _Settings;

            InitializeComponent();
            DataContext = this;
        }

        public Settings.Settings Settings { get; set; }

        public bool NeedRestart { get; private set; }

        private void DatabasePathButtonOnClick(object sender, RoutedEventArgs e)
        {
            string initialDirectory = Path.GetDirectoryName(Settings.DatabasePath);
            string fileName = Path.GetFileName(Settings.DatabasePath);

            OpenFileDialog dialog = new OpenFileDialog
            {
                InitialDirectory = initialDirectory,
                FileName = fileName,
                Filter = "Database files|*.db",
                CheckFileExists = true
            };

            if (dialog.ShowDialog() == true)
            {
                Settings.DatabasePath = dialog.FileName;
                NeedRestart = true;
            }
        }

        private void CancelButtonOnClick(object sender, RoutedEventArgs e)
        {
            m_ManualClosing = true;
            Settings.CopyTo(m_SaveSettings);
            DialogResult = false;
        }

        private void ConfirmButtonOnClick(object sender, RoutedEventArgs e)
        {
            m_ManualClosing = true;
            DialogResult = true;
        }

        private void SettingsWindowOnClosing(object sender, CancelEventArgs e)
        {
            if (!m_ManualClosing)
            {
                Settings.CopyTo(m_SaveSettings);
                DialogResult = false;
            }
        }
    }
}
