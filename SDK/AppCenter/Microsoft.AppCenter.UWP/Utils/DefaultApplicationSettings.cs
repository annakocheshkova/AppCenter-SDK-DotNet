// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.Storage;

namespace Microsoft.AppCenter.Utils
{
    public class DefaultApplicationSettings : IApplicationSettings
    {
        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            object result;
            var found = ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out result);
            if (found)
            {
                return (T)result;
            }
            return defaultValue;
        }

        public void SetValue(string key, object value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }

        public bool ContainsKey(string key)
        {
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(key);
        }

        public void Remove(string key)
        {
            ApplicationData.Current.LocalSettings.Values.Remove(key);
        }
    }
}
