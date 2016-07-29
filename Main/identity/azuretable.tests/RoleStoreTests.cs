// MIT License Copyright 2014 (c) David Melendez. All rights reserved. See License.txt in the project root for license information.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElCamino.AspNet.Identity.AzureTable;
using Microsoft.AspNet.Identity;
using ElCamino.AspNet.Identity.AzureTable.Model;

namespace ElCamino.AspNet.Identity.AzureTable.Tests
{
    [TestClass]
    public class RoleStoreTests
    {
        private static IdentityRole CurrentRole;
        private static bool TablesCreated = false;

        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            if (!TablesCreated)
            {
                using (RoleStore<IdentityRole> store = new RoleStore<IdentityRole>())
                {
                    var taskCreateTables = store.CreateTableIfNotExistsAsync();
                    taskCreateTables.Wait();
                    TablesCreated = true;
                }
            }
            if (CurrentRole == null)
            {
                CreateRole();
            }
        }

        [TestMethod]
        [TestCategory("Identity.Azure.RoleStore")]
        public void RoleStoreCtors()
        {
            try
            {
                new RoleStore<IdentityRole>(null);
            }
            catch (ArgumentException) { }
        }

        [TestMethod]
        [TestCategory("Identity.Azure.RoleStore")]
        public void RoleStoreGet_Roles()
        {
            using (RoleStore<IdentityRole> store = new RoleStore<IdentityRole>())
            {
                Assert.IsNotNull(store.Roles, "Roles accessor is null");
            }
        }

        [TestMethod]
        [TestCategory("Identity.Azure.RoleStore")]
        public void CreateRole()
        {    
            using (RoleStore<IdentityRole> store = new RoleStore<IdentityRole>())
            {
                using (RoleManager<IdentityRole> manager = new RoleManager<IdentityRole>(store))
                {
                    string roleNew = string.Format("TestRole_{0}", Guid.NewGuid());
                    var role = new IdentityRole(roleNew);
                    var start = DateTime.UtcNow;
                    var createTask = manager.CreateAsync(role);
                    createTask.Wait();
                    TestContext.WriteLine("CreateRoleAsync: {0} seconds", (DateTime.UtcNow - start).TotalSeconds);

                    CurrentRole = role;
                    WriteLineObject<IdentityRole>(CurrentRole);

                    try
                    {
                        var task = store.CreateAsync(null);
                        task.Wait();
                    }
                    catch (Exception ex)
                    {
                        Assert.IsNotNull(ex, "Argument exception not raised");
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("Identity.Azure.RoleStore")]
        public void ThrowIfDisposed()
        {
            using (RoleStore<IdentityRole> store = new RoleStore<IdentityRole>())
            {
                RoleManager<IdentityRole> manager = new RoleManager<IdentityRole>(store);
                manager.Dispose();

                try
                {
                    var task = store.DeleteAsync(null);
                }
                catch (ArgumentException) { }
            }
        }

        [TestMethod]
        [TestCategory("Identity.Azure.RoleStore")]
        public void UpdateRole()
        {
            using (RoleStore<IdentityRole> store = new RoleStore<IdentityRole>())
            {
                using (RoleManager<IdentityRole> manager = new RoleManager<IdentityRole>(store))
                {
                    string roleNew = string.Format("TestRole_{0}", Guid.NewGuid());

                    var role = new IdentityRole(roleNew);
                    var createTask = manager.CreateAsync(role);
                    createTask.Wait();

                    role.Name = Guid.NewGuid() + role.Name;
                    var updateTask = manager.UpdateAsync(role);
                    updateTask.Wait();

                    var findTask = manager.FindByIdAsync(role.RowKey);

                    Assert.IsNotNull(findTask.Result, "Find Role Result is null");
                    Assert.AreEqual<string>(role.RowKey, findTask.Result.RowKey, "RowKeys don't match.");
                    Assert.AreNotEqual<string>(roleNew, findTask.Result.Name, "Name not updated.");

                    try
                    {
                        var task = store.UpdateAsync(null);
                        task.Wait();
                    }
                    catch (Exception ex)
                    {
                        Assert.IsNotNull(ex, "Argument exception not raised");
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("Identity.Azure.RoleStore")]
        public void UpdateRole2()
        {
            using (RoleStore<IdentityRole> store = new RoleStore<IdentityRole>())
            {
                using (RoleManager<IdentityRole> manager = new RoleManager<IdentityRole>(store))
                {
                    string roleNew = string.Format("{0}_TestRole", Guid.NewGuid());

                    var role = new IdentityRole(roleNew);
                    var createTask = manager.CreateAsync(role);
                    createTask.Wait();

                    role.Name = role.Name + Guid.NewGuid();
                    var updateTask = manager.UpdateAsync(role);
                    updateTask.Wait();

                    var findTask = manager.FindByIdAsync(role.RowKey);
                    findTask.Wait();
                    Assert.IsNotNull(findTask.Result, "Find Role Result is null");
                    Assert.AreEqual<string>(role.RowKey, findTask.Result.RowKey, "RowKeys don't match.");
                    Assert.AreNotEqual<string>(roleNew, findTask.Result.Name, "Name not updated.");
                }
            }
        }

        [TestMethod]
        [TestCategory("Identity.Azure.RoleStore")]
        public void DeleteRole()
        {
            using (RoleStore<IdentityRole> store = new RoleStore<IdentityRole>())
            {
                using (RoleManager<IdentityRole> manager = new RoleManager<IdentityRole>(store))
                {
                    string roleNew = string.Format("TestRole_{0}", Guid.NewGuid());
                    var role = new IdentityRole(roleNew);
                    var createTask = manager.CreateAsync(role);
                    createTask.Wait();

                    var start = DateTime.UtcNow;
                    var delTask = manager.DeleteAsync(role);
                    delTask.Wait();
                    TestContext.WriteLine("DeleteRole: {0} seconds", (DateTime.UtcNow - start).TotalSeconds);

                    var findTask = manager.FindByIdAsync(role.RowKey);
                    findTask.Wait();
                    Assert.IsNull(findTask.Result, "Role not deleted ");

                    try
                    {
                        var task = store.DeleteAsync(null);
                        task.Wait();
                    }
                    catch (Exception ex)
                    {
                        Assert.IsNotNull(ex, "Argument exception not raised");
                    }
                }
            }
        }


        [TestMethod]
        [TestCategory("Identity.Azure.RoleStore")]
        public void FindRoleById()
        {
            using (RoleStore<IdentityRole> store = new RoleStore<IdentityRole>())
            {
                using (RoleManager<IdentityRole> manager = new RoleManager<IdentityRole>(store))
                {
                    DateTime start = DateTime.UtcNow;
                    var findTask = manager.FindByIdAsync(CurrentRole.Id);
                    findTask.Wait();
                    TestContext.WriteLine("FindByIdAsync: {0} seconds", (DateTime.UtcNow - start).TotalSeconds);

                    Assert.IsNotNull(findTask.Result, "Find Role Result is null");
                    WriteLineObject<IdentityRole>(findTask.Result);
                    Assert.AreEqual<string>(CurrentRole.Id, findTask.Result.RowKey, "RowKeys don't match.");
                }
            }
        }

        [TestMethod]
        [TestCategory("Identity.Azure.RoleStore")]
        public void FindRoleByName()
        {
            using (RoleStore<IdentityRole> store = new RoleStore<IdentityRole>())
            {
                using (RoleManager<IdentityRole> manager = new RoleManager<IdentityRole>(store))
                {
                    DateTime start = DateTime.UtcNow;
                    var findTask = manager.FindByNameAsync(CurrentRole.Name);
                    findTask.Wait();
                    TestContext.WriteLine("FindByNameAsync: {0} seconds", (DateTime.UtcNow - start).TotalSeconds);

                    Assert.IsNotNull(findTask.Result, "Find Role Result is null");
                    Assert.AreEqual<string>(CurrentRole.Name, findTask.Result.Name, "Role names don't match.");
                }
            }
        }

        private void WriteLineObject<t>(t obj) where t : class
        {
            TestContext.WriteLine(typeof(t).Name);
            string strLine = obj == null ? "Null" : Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            TestContext.WriteLine("{0}", strLine);
        }

    }
}
