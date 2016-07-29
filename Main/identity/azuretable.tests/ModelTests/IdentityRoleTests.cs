using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElCamino.AspNet.Identity.AzureTable.Model;

namespace ElCamino.Web.Identity.AzureTable.Tests.ModelTests
{
    [TestClass]
    public class IdentityRoleTests
    {
        [TestMethod]
        [TestCategory("Identity.Azure.Model")]
        public void IdentityRoleSet_Id()
        {
            var role = new IdentityRole();
            role.Id = Guid.NewGuid().ToString(); 
            Assert.AreEqual<string>(role.RowKey, role.Id, "Id and RowKey are not equal");
        }
    }
}
