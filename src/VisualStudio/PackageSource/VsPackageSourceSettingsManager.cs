using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace NuGet.VisualStudio {
    [Export(typeof(IPackageSourceSettingsManager))]
    public class VsPackageSourceSettingsManager : IPackageSourceSettingsManager {
        private const string SettingsRoot = "NuGet";
        private const string PackageSourcesSettingProperty = "PackageSources";
        private const string ActivePackageSourceSettingProperty = "ActivePackageSource";

        private WritableSettingsStore _userSettingsStore;
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        public VsPackageSourceSettingsManager(IServiceProvider serviceProvider) {
            if (serviceProvider == null) {
                throw new ArgumentNullException("serviceProvider");
            }

            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets or sets the string which encodes all PackageSources in the VS setting store.
        /// </summary>
        /// <value>The package sources string.</value>
        public string PackageSourcesString {
            get {
                return UserSettingsStore.GetString(SettingsRoot, PackageSourcesSettingProperty, "");
            }
            set {
                UserSettingsStore.SetString(SettingsRoot, PackageSourcesSettingProperty, value ?? "");
            }
        }

        /// <summary>
        /// Gets or sets the string which encodes the active PackageSource in the VS setting store
        /// </summary>
        /// <value>The active package source string.</value>
        public string ActivePackageSourceString {
            get {
                return UserSettingsStore.GetString(SettingsRoot, ActivePackageSourceSettingProperty, "");
            }
            set {
                _userSettingsStore.SetString(SettingsRoot, ActivePackageSourceSettingProperty, value ?? "");
            }
        }

        private WritableSettingsStore UserSettingsStore {
            get {
                if (_userSettingsStore == null) {
                    SettingsManager settingsManager = new ShellSettingsManager(_serviceProvider);
                    _userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

                    // Ensure that the package collection exists before any callers attempt to use it.
                    if (!_userSettingsStore.CollectionExists(SettingsRoot)) {
                        _userSettingsStore.CreateCollection(SettingsRoot);
                    }
                }
                return _userSettingsStore;
            }
        }
    }
}