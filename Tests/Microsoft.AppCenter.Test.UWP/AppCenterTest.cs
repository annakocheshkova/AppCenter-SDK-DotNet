// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Windows.ApplicationModel.Core;

namespace Microsoft.AppCenter.Test.UWP
{
    [TestClass]
    public class AppCenterTest
    {
        private AggregateException _unobservedTaskException;

        [TestInitialize]
        public void InitializeAppCenterTest()
        {
            // Mock the channel and group channel.
            var mockChannel = new Mock<IChannelUnit>();
            var mockGroup = new Mock<IChannelGroup>();
            mockGroup.Setup(mock => mock.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>())).Returns(mockChannel.Object);

            // Mock the channel group factory.
            var mockGroupFactory = new Mock<IChannelGroupFactory>();
            mockGroupFactory.Setup(mock => mock.CreateChannelGroup(It.IsAny<string>(), It.IsAny<INetworkStateAdapter>())).Returns(mockGroup.Object);

            // Replace the channel group factory on mock.
            AppCenter.SetChannelGroupFactory(mockGroupFactory.Object);

            _unobservedTaskException = null;
            AppCenter.Instance = null;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        [TestCleanup]
        public void CleanupAppCenterTest()
        {
            // The UnobservedTaskException will only happen if a Task gets collected by the GC with an exception unobserved
            GC.Collect();
            GC.WaitForPendingFinalizers();
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;

            if (_unobservedTaskException != null)
            {
                throw _unobservedTaskException;
            }
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _unobservedTaskException = e.Exception;
        }

        /// <summary>
        /// Verify configure with UWP platform id
        /// </summary>
        [TestMethod]
        public void VerifyPlatformId()
        {
            CoreApplication.MainView.Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AppCenter.Configure("uwp=appsecret");
            }).AsTask().GetAwaiter().GetResult();

            Assert.IsTrue(AppCenter.Configured);
        }

        /// <summary>
        /// Verify multiple unhandled exceptions handling
        /// </summary>
        [TestMethod]
        public void MultipleUnhandledExceptions()
        {
            AppCenter.Start("uwp=appsecret");
            Assert.IsTrue(AppCenter.Configured);

            // Call unhandled exception handler and wait a bit to make sure handle "fire and forget" calls (if they exist).
            ApplicationLifecycleHelper.Instance.InvokeUnhandledExceptionOccurred(null, new Exception());
            Task.Delay(100).Wait();

            // Call again.
            ApplicationLifecycleHelper.Instance.InvokeUnhandledExceptionOccurred(null, new Exception());
            Task.Delay(100).Wait();

            // No exceptions are thrown.
        }

        [TestMethod]
        public void TestCorrectDatabasePathPassedFromStorageInitialization()
        {
            // Make sure database does not exist before test.
            if (File.Exists(Constants.AppCenterDatabasePath))
            {
                File.Delete(Constants.AppCenterDatabasePath);
            }
            using (var storageMock = new Storage.Storage())
            {
                storageMock.WaitOperationsAsync(TimeSpan.FromSeconds(10)).Wait();

                // Verify that database is created inside local app data folder, and not locally.
                Assert.IsTrue(File.Exists(Path.Combine(Constants.LocalAppData, Constants.AppCenterDatabaseFilename)));
            }
        }

        /// <summary>
        /// Start service after unhandled exception
        /// </summary>
        [TestMethod]
        public void StartServiceAfterUnhandledException()
        {
            AppCenter.Configure("uwp=appsecret");
            Assert.IsTrue(AppCenter.Configured);

            // Some exception occurred.
            ApplicationLifecycleHelper.Instance.InvokeUnhandledExceptionOccurred(null, new Exception());
            Task.Delay(100).Wait();

            // Start any service.
            AppCenter.Start(typeof(TestAppCenterService));

            // No exceptions are thrown.
            // System.Diagnostics.Debug.Listeners is not available here, so not able to verify that there are no errors in logs.
        }

        /// <summary>
        /// Set custom properties after unhandled exception
        /// </summary>
        [TestMethod]
        public void SetPropertiesAfterUnhandledException()
        {
            AppCenter.Start("uwp=appsecret");
            Assert.IsTrue(AppCenter.Configured);

            // Some exception occurred.
            ApplicationLifecycleHelper.Instance.InvokeUnhandledExceptionOccurred(null, new Exception());
            Task.Delay(100).Wait();

            // Set custom properties.
            AppCenter.SetCustomProperties(new CustomProperties().Set("test", 42));
            Task.Delay(100).Wait();

            // No exceptions are thrown.
            // System.Diagnostics.Debug.Listeners is not available here, so not able to verify that there are no errors in logs.
        }
    }
}
