// MIT License Copyright 2014 (c) David Melendez. All rights reserved. See License.txt in the project root for license information.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElCamino.AspNet.Identity.AzureTable.Tests.HelperTests
{
    [TestClass]
    public class KeyHelperTests
    {
        [TestMethod]
        [TestCategory("Identity.Azure.Helper.KeyHelper")]
        public void GeneratePartitionKeyIndexByEmail()
        {
            //Only keeping this method around for any backwards compat issues.
            string strEmail = Guid.NewGuid().ToString() + "@.hotmail.com";
            string key = Helpers.KeyHelper.GeneratePartitionKeyIndexByEmail(strEmail);
        }
    }
}
