// MIT License Copyright 2014 (c) David Melendez. All rights reserved. See License.txt in the project root for license information.
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElCamino.AspNet.Identity.AzureTable.Model;
using ElCamino.AspNet.Identity.AzureTable.Configuration;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.Azure;

namespace ElCamino.AspNet.Identity.AzureTable
{
    public class IdentityCloudContext : IdentityCloudContext<IdentityUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public IdentityCloudContext()
            : base() { }

        [System.Obsolete("Please use the default constructor IdentityCloudContext() to load the configSection from web/app.config or " +
            "the constructor IdentityCloudContext(IdentityConfiguration config) for more options.")]
        public IdentityCloudContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }
        public IdentityCloudContext(IdentityConfiguration config) :
            base(config) { }
    }

    public class IdentityCloudContext<TUser> : IdentityCloudContext<TUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim> where TUser : IdentityUser
    {
        public IdentityCloudContext()
            : base()
        {
        }

        [System.Obsolete("Please use the default constructor IdentityCloudContext() to load the configSection from web/app.config or " +
            "the constructor IdentityCloudContext(IdentityConfiguration config) for more options.")]
        public IdentityCloudContext(string connectionStringKey)
            : base(connectionStringKey)
        {
        }

        public IdentityCloudContext(IdentityConfiguration config) :
            base(config) { }
    }

    public class IdentityCloudContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> : IDisposable
        where TUser : IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : IdentityRole<TKey, TUserRole>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
    {
        private CloudTableClient _client = null;
        private bool _disposed = false;
        private IdentityConfiguration _config = null;
        private CloudTable _roleTable;
        private CloudTable _indexTable;
        private CloudTable _userTable;

        public IdentityCloudContext() 
        {
            IdentityConfiguration config = IdentityConfigurationSection.GetCurrent();
            //For backwards compat for those who do not use the new configSection.
            if (config == null)
            {
                config = new IdentityConfiguration()
                {
                    StorageConnectionString =  CloudConfigurationManager.GetSetting(Constants.AppSettingsKeys.DefaultStorageConnectionStringKey),
                    TablePrefix = string.Empty
                };
            }
            Initialize(config);
        }

        [System.Obsolete("Please use the default constructor IdentityCloudContext() to load the configSection from web/app.config or " +
            "the constructor IdentityCloudContext(IdentityConfiguration config) for more options.")]
        public IdentityCloudContext(string connectionStringKey)
        {
            string strConnection = CloudConfigurationManager.GetSetting(connectionStringKey);
            Initialize(new IdentityConfiguration()
            {
                StorageConnectionString = string.IsNullOrWhiteSpace(strConnection) ?
                    connectionStringKey : strConnection,
                TablePrefix = string.Empty
            });
            
        }

        public IdentityCloudContext(IdentityConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            Initialize(config);
        }

        private void Initialize(IdentityConfiguration config)
        {
            _config = config;
            _client = CloudStorageAccount.Parse(_config.StorageConnectionString).CreateCloudTableClient();
            if (!string.IsNullOrWhiteSpace(_config.LocationMode))
            {
                LocationMode mode = LocationMode.PrimaryOnly;
                if (Enum.TryParse<LocationMode>(_config.LocationMode, out mode))
                {
                    _client.DefaultRequestOptions.LocationMode = mode;
                }
                else
                {
                    throw new ArgumentException("Invalid LocationMode defined in config. For more information on geo-replication location modes: http://msdn.microsoft.com/en-us/library/azure/microsoft.windowsazure.storage.retrypolicies.locationmode.aspx", "config.LocationMode");
                }
            }
            _indexTable = _client.GetTableReference(FormatTableNameWithPrefix(Constants.TableNames.IndexTable));
            _roleTable = _client.GetTableReference(FormatTableNameWithPrefix(Constants.TableNames.RolesTable)); 
            _userTable = _client.GetTableReference(FormatTableNameWithPrefix(Constants.TableNames.UsersTable));
        }

        ~IdentityCloudContext()
        {
            this.Dispose(false);
        }

        private string FormatTableNameWithPrefix(string baseTableName)
        {
            if(!string.IsNullOrWhiteSpace(_config.TablePrefix))
            {
                return string.Format("{0}{1}", _config.TablePrefix, baseTableName);
            }
            return baseTableName;
        }

        public CloudTable RoleTable
        {
            get
            {
                ThrowIfDisposed();
                return _roleTable;
            }
        }

        public CloudTable UserTable
        {
            get
            {
                ThrowIfDisposed();
                return _userTable;
            }
        }

        public CloudTable IndexTable
        {
            get
            {
                ThrowIfDisposed();
                return _indexTable;
            }
        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(base.GetType().Name);
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _client = null;
                _indexTable = null;
                _roleTable = null;
                _userTable = null;
                _disposed = true;
            }
        }
    }

}
