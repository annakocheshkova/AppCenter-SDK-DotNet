// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Com.Microsoft.Appcenter.Crashes.Model;

namespace Microsoft.AppCenter.Crashes
{
    static class ErrorReportCache
    {
        readonly static Dictionary<string, ErrorReport> cachedReports = new Dictionary<string, ErrorReport>();

        internal static ErrorReport GetErrorReport(AndroidErrorReport androidReport)
        {
            lock (cachedReports)
            {
                ErrorReport cachedReport;
                if (cachedReports.TryGetValue(androidReport.Id, out cachedReport))
                {
                    return cachedReport;
                }

                var newErrorReport = new ErrorReport(androidReport);
                cachedReports[androidReport.Id] = newErrorReport;
                return newErrorReport;
            }
        }
    }
}
