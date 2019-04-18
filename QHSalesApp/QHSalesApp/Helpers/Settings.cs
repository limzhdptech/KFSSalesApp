// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace QHSalesApp.Helpers
{
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters. 
	/// </summary>
	public static class Settings
	{
		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		#region Setting Constants

		private const string SettingsKey = "settings_key";
		private static readonly string SettingsDefault = string.Empty;
        private const string Email = "user_email";
        private static readonly string EmailDefault = string.Empty;
        private const string AccessKey = "access_key";
        private static readonly string AccessKeyDefault = string.Empty;
        private const string DeviceId = "Device_Id";
        private static readonly string DeviceIDDefault = string.Empty;

        #endregion


        public static string GeneralSettings
		{
			get
			{
				return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(SettingsKey, value);
			}
		}

        public static string UserEmail
        {
            get
            {
                return AppSettings.GetValueOrDefault(Email, EmailDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(Email, value);
            }
        }

        public static string DeviceAccessKey
        {
            get
            {
                return AppSettings.GetValueOrDefault(AccessKey, AccessKeyDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AccessKey, value);
            }
        }

    }
}