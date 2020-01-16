// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Crashes
{
    /// <summary>
    /// Error report containing information about a particular crash.
    /// </summary>
    public partial class ErrorReport
    {
        /// <summary>
        /// Gets the report identifier.
        /// </summary>
        /// <value>UUID for the report.</value>
        public string Id { get; }

        /// <summary>
        /// Gets the app start time.
        /// </summary>
        /// <value>Date and time the app started</value>
        public DateTimeOffset AppStartTime { get; }

        /// <summary>
        /// Gets the app error time.
        /// </summary>
        /// <value>Date and time the error occured</value>
        public DateTimeOffset AppErrorTime { get; }

        /// <summary>
        /// Gets the device that the crashed app was being run on.
        /// </summary>
        /// <value>Device information at the crash time.</value>
        public Device Device { get; }

        /// <summary>
        /// Gets the C# Exception object that caused the crash.
        /// </summary>
        /// <value>The exception.</value>
        [ObsoleteAttribute("This property is no longer set due to a security issue, use StackTrace as an alternative.")]
        public Exception Exception { get; }

        /// <summary>
        /// Gets the C# exception stack trace captured at crash time.
        /// </summary>
        /// <value>The exception.</value>
        public string StackTrace { get; }

        /// <summary>
        /// Gets details specific to Android.
        /// </summary>
        /// <value>Android error report details. <c>null</c> if the OS is not Android.</value>
        public AndroidErrorDetails AndroidDetails { get; }

        /// <summary>
        /// Gets details specific to iOS.
        /// </summary>
        /// <value>iOS error report details. <c>null</c> if the OS is not iOS.</value>
        public iOSErrorDetails iOSDetails { get; }
    }
}
