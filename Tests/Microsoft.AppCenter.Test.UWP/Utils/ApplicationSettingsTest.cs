// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Storage;

namespace Microsoft.AppCenter.Test.UWP.Utils
{
    [TestClass]
    public class ApplicationSettingsTest
    {
        private IApplicationSettings settings;

        [TestInitialize]
        public void InitializeAppCenterTest()
        {
            ApplicationData.Current.LocalSettings.Values.Clear();
            settings = new DefaultApplicationSettings();
        }

        /// <summary>
        /// Verify SetValue generic method behaviour
        /// </summary>
        [TestMethod]
        public void VerifySetValue()
        {
            const string key = "test";
            Assert.IsFalse(settings.ContainsKey(key));
            settings.SetValue(key, 42);
            Assert.IsTrue(settings.ContainsKey(key));
            Assert.AreEqual(42, settings.GetValue<int>(key));
        }

        /// <summary>
        /// Verify GetValue and SetValue generic method behaviour
        /// </summary>
        [TestMethod]
        public void VerifyGetValue()
        {
            const string key = "test";
            Assert.IsFalse(settings.ContainsKey(key));
            Assert.AreEqual(42, settings.GetValue(key, 42));
            Assert.AreEqual(0, settings.GetValue<int>(key));
            Assert.AreEqual(0, settings.GetValue(key, 0));
        }

        /// <summary>
        /// Verify remove values from settings
        /// </summary>
        [TestMethod]
        public void VerifyRemove()
        {
            const string key = "test"; Assert.IsFalse(settings.ContainsKey(key));
            settings.SetValue(key, 42);
            Assert.IsTrue(settings.ContainsKey(key));
            Assert.AreEqual(42, settings.GetValue<int>(key));
            settings.Remove(key);
            Assert.IsFalse(settings.ContainsKey(key));
        }
    }
}
