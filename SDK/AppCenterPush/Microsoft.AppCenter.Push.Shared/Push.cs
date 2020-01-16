// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Push
{
    /// <summary>
    /// Push feature.
    /// </summary>
    public partial class Push
    {
        /// <summary>
        /// Check whether the Push service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            return PlatformIsEnabledAsync();
        }

        /// <summary>
        /// Enable or disable the Push service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            return PlatformSetEnabledAsync(enabled);
        }

        /// <summary>
        /// Occurs when the application receives a push notification.
        /// </summary>
        public static event EventHandler<PushNotificationReceivedEventArgs> PushNotificationReceived;
    }
}
