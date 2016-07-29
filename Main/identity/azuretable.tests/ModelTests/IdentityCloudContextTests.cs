// MIT License Copyright 2014 (c) David Melendez. All rights reserved. See License.txt in the project root for license information.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElCamino.AspNet.Identity.AzureTable.Model;
using System.Resources;
using Microsoft.WindowsAzure;
using System.Configuration;
using ElCamino.AspNet.Identity.AzureTable.Configuration;
using Microsoft.Azure;

namespace ElCamino.AspNet.Identity.AzureTable.Tests.ModelTests
{
    [TestClass]
    public class IdentityCloudContextTests
    {
        [TestMethod]
        [TestCategory("Identity.Azure.Model")]
        public void IdentityCloudContextCtors()
        {
            string strValidConnection = CloudConfigurationManager.GetSetting(
                ElCamino.AspNet.Identity.AzureTable.Constants.AppSettingsKeys.DefaultStorageConnectionStringKey);

            var currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = currentConfig.Sections[IdentityConfigurationSection.Name];
            if (section == null)
            {
                currentConfig.Sections.Add(IdentityConfigurationSection.Name,
                    new IdentityConfigurationSection()
                    {
                        TablePrefix = string.Empty,
                        StorageConnectionString = strValidConnection
                    });
                currentConfig.Save(ConfigurationSaveMode.Modified);
            }
            var ic = new IdentityCloudContext();
            Assert.IsNotNull(ic, "New IdentityCloudContext is null");

            //Pass in valid connection string
            var icc = new IdentityCloudContext(strValidConnection);
            icc.Dispose();

            ic = new IdentityCloudContext(new IdentityConfiguration() 
            { 
                TablePrefix = string.Empty, 
                StorageConnectionString = strValidConnection 
            });

            using (UserStore<IdentityUser> store = new UserStore<IdentityUser>(
                new IdentityCloudContext<IdentityUser>(new IdentityConfiguration()
                    {
                        StorageConnectionString = strValidConnection,
                        TablePrefix = "My"
                    })))
            {
                var task = store.CreateTablesIfNotExists();
                task.Wait();
            }

            currentConfig.Sections.Remove(IdentityConfigurationSection.Name);
            currentConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(IdentityConfigurationSection.Name);

            using (UserStore<IdentityUser> store = new UserStore<IdentityUser>(
                new IdentityCloudContext<IdentityUser>()))
            {
                var task = store.CreateTablesIfNotExists();
                task.Wait();
            }

            currentConfig.Sections.Add(IdentityConfigurationSection.Name,
                new IdentityConfigurationSection()
                {
                    TablePrefix = string.Empty,
                    StorageConnectionString = strValidConnection,
                    LocationMode = "PrimaryThenSecondary"
                });
            currentConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(IdentityConfigurationSection.Name);

            string strInvalidConnectionStringKey = Guid.NewGuid().ToString();

            var testAppsettings = new IdentityCloudContext(ElCamino.AspNet.Identity.AzureTable.Constants.AppSettingsKeys.DefaultStorageConnectionStringKey);
            testAppsettings.Dispose();

            try
            {
                ic = new IdentityCloudContext(new IdentityConfiguration()
                {
                    TablePrefix = string.Empty,
                    StorageConnectionString = strValidConnection,
                    LocationMode = "InvalidLocationMode"
                });
            }
            catch (ArgumentException) { }

            try
            {
                ic = new IdentityCloudContext(strInvalidConnectionStringKey);
            }
            catch (System.FormatException) { }

            try
            {
                ic = new IdentityCloudContext(string.Empty);
            }
            catch (MissingManifestResourceException) {  }

            //----------------------------------------------
            var iucc = new IdentityCloudContext<IdentityUser>();
            Assert.IsNotNull(iucc, "New IdentityCloudContext is null");

            try
            {
                iucc = new IdentityCloudContext<IdentityUser>(strInvalidConnectionStringKey);
            }
            catch (System.FormatException) { }

            try
            {
                iucc = new IdentityCloudContext<IdentityUser>(string.Empty);
            }
            catch (MissingManifestResourceException) { }
            
            //------------------------------------------

            var i2 = new IdentityCloudContext<IdentityUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>();
            Assert.IsNotNull(i2, "New IdentityCloudContext is null");

            try
            {
                i2 = new IdentityCloudContext<IdentityUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>(Guid.NewGuid().ToString());
            }
            catch (System.FormatException) { }

            try
            {
                i2 = new IdentityCloudContext<IdentityUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>(string.Empty);
            }
            catch (MissingManifestResourceException) { }

            try
            {
                var i3 = new IdentityCloudContext<IdentityUser>();
                i3.Dispose();
                var table = i3.RoleTable;
            }
            catch (ObjectDisposedException) { }

            try
            {
                var i4 = new IdentityCloudContext<IdentityUser>();
                i4.Dispose();
                var table = i4.UserTable;
            }
            catch (ObjectDisposedException) { }

            try
            {
                IdentityConfiguration iconfig = null;
                var i5 = new IdentityCloudContext<IdentityUser>(iconfig);
            }
            catch (ArgumentNullException) { }
        }
    }
}
