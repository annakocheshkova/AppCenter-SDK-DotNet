// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Test.UWP.Utils
{
    [TestClass]
    public class DeviceInformationHelperTest
    {
        /// <summary>
        /// Verify sdk name in device information
        /// </summary>
        [TestMethod]
        public void VerifySdkName()
        {
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreEqual(device.SdkName, "appcenter.uwp");
        }

        /// <summary>
        /// Verify carrier country in device information
        /// </summary>
        [TestMethod]
        public void VerifyCarrierCountry()
        {
            const string CountryCode = "US";
            AppCenter.SetCountryCode(CountryCode);

            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreEqual(device.CarrierCountry, CountryCode);
        }

        /// <summary>
        /// Verify device oem name in device information
        /// </summary>
        [TestMethod]
        public void VerifyDeviceOemName()
        {
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreNotEqual(device.OemName, AbstractDeviceInformationHelper.DefaultSystemManufacturer);
        }

        /// <summary>
        /// Verify device model in device model.
        /// </summary>
        [TestMethod]
        public void VerifyDeviceModel()
        {
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreNotEqual(device.Model, AbstractDeviceInformationHelper.DefaultSystemProductName);
        }

        /// <summary>
        /// Verify screen size provider
        /// </summary>
        [TestMethod]
        public void VerifyScreenSizeProvider()
        {
            const string testScreenSize = "screen_size";
            var informationInvalidated = false;
            var screenSizeProviderMock = new Mock<IScreenSizeProvider>();
            var screenSizeProviderFactoryMock = new Mock<IScreenSizeProviderFactory>();
            screenSizeProviderFactoryMock.Setup(factory => factory.CreateScreenSizeProvider()).Returns(screenSizeProviderMock.Object);
            screenSizeProviderMock.Setup(provider => provider.ScreenSize).Returns(testScreenSize);
            DeviceInformationHelper.SetScreenSizeProviderFactory(screenSizeProviderFactoryMock.Object);

            // Screen size is returned from screen size provider
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreEqual(testScreenSize, device.ScreenSize);

            // InformationInvalidated is invoked when ScreenSizeChanged event is raised
            DeviceInformationHelper.InformationInvalidated += (sender, args) => { informationInvalidated = true; };
            screenSizeProviderMock.Raise(provider => provider.ScreenSizeChanged += null, EventArgs.Empty);
            Assert.IsTrue(informationInvalidated);
        }
    }
}
