using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlConfig.Core;
using Microsoft.Win32;
using System.Windows;

namespace CortexCommandModManager.Startup
{
    //TODO encapsulate MessageBox functionality and inject it
    
    public class CCMMInitializer
    {
        private const string SettingsFileName = "settings.xml";

        private CCMMInitialization initialization;

        public CCMMInitialization Initialize()
        {
            initialization = new CCMMInitialization();
            InitializeSettings();
            InitializeCortexCommandDirectory();
            InitializeSingletonScopedObjects();
            return initialization;
        }

        private void InitializeSingletonScopedObjects()
        {
            initialization.ModManager = new ModManager(initialization.Settings);
            initialization.EnhancedSkirmish = new EnhancedSkirmish();
            initialization.SkirmishSettingsManager = new SkirmishSettingsManager();
            initialization.GameSettingsManager = new GameSettingsManager();
            initialization.ModScanner = new ModScanner(initialization.Settings, initialization.ModManager);
            initialization.PresetManager = new PresetManager(initialization.ModScanner, initialization.ModManager);
        }

        private void InitializeCortexCommandDirectory()
        {
            if (String.IsNullOrEmpty(initialization.Settings.Get().CCInstallDirectory) == false)
                return; //Already initialized and in settings object.

            var installDirectory = TryGetCCPathFromRegistryAndAskIfWantsToUse();

            if (installDirectory == null)
                installDirectory = AskForInstallDirectory();

            if (installDirectory.IsNullOrEmpty())
            {
                throw IncorrectInstallation();
            }

            initialization.Settings.Set(x => x.CCInstallDirectory = installDirectory);
        }

        private IncorrectInstallationException IncorrectInstallation()
        {
            return new IncorrectInstallationException("Cortex command must be installed for the mod manager to work. The Mod Manager will now close.");
        }

        private string TryGetCCPathFromRegistryAndAskIfWantsToUse()
        {
            var installDirectory = TryGetCCPathFromRegistry();

            if (installDirectory == null)
                return null;

            var result = MessageBox.Show(
                        @"A valid Cortex Command installation was found at ""{0}"". Would you like you use this?".With(installDirectory),
                        "Cortex Command Install Directory",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.Yes);
            
            if (result == MessageBoxResult.No)
                return null;

            return installDirectory;
        }

        private string AskForInstallDirectory()
        {
            MessageBox.Show("Since this is the first time the Mod Manager has been run, you need to choose the directory that Cortex Command is installed to.", "First Time", MessageBoxButton.OK, MessageBoxImage.Information);

            while (true)
            {
                var browser = new System.Windows.Forms.FolderBrowserDialog();
                browser.Description = "Choose the install location for Cortex Command";
                browser.ShowNewFolderButton = false;
                browser.RootFolder = Environment.SpecialFolder.MyComputer;

                var result = browser.ShowDialog();

                if (result != System.Windows.Forms.DialogResult.OK)
                    return null; //The user cancelled the choosing.

                var installDirectory = browser.SelectedPath;

                if (CortexCommand.IsInstalledTo(installDirectory))
                    return installDirectory;

                MessageBox.Show("The directory you chose does not have a valid Cortex Command installation in it. Please choose the folder where Cortex Command is installed.",
                    "Incorrect Folder", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private string TryGetCCPathFromRegistry()
        {
            return Registry.GetValue("HKEY_CURRENT_USER\\Software\\Data Realms", "Cortex Command", null) as string;
        }

        private void InitializeSettings()
        {
            initialization.Settings = new Settings<SettingsObject>(SettingsFileName);
            Grabber.SetSettings(initialization.Settings);
        }
    }
}
