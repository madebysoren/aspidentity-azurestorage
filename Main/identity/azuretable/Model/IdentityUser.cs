﻿// MIT License Copyright 2014 (c) David Melendez. All rights reserved. See License.txt in the project root for license information.
using ElCamino.AspNet.Identity.AzureTable.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElCamino.AspNet.Identity.AzureTable.Model
{
    public class IdentityUser : IdentityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
        , IUser
        , IUser<string>
        , IGenerateKeys
    {
        public IdentityUser() { }

        public IdentityUser(string userName)
            : this()
        {
            this.UserName = userName;
        }

        /// <summary>
        /// Generates Row, Partition and Id keys.
        /// All are the same in this case
        /// </summary>
        public void GenerateKeys()
        {
            Id = PeekRowKey();
            PartitionKey = Id;
            KeyVersion = KeyHelper.KeyVersion;
        }

        /// <summary>
        /// Generates the RowKey without setting it on the object.
        /// In this case, just returns a key based on username
        /// </summary>
        /// <returns></returns>
        public string PeekRowKey()
        {
            return KeyHelper.GenerateRowKeyUserName(UserName);
        }

        public double KeyVersion { get; set; }

        public override string Id
        {
            get
            {
                return RowKey;
            }
            set
            {
                RowKey = value;
            }
        }

        public override string UserName
        {
            get
            {
                return base.UserName;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    base.UserName = value.Trim();
                }
            }
        }
    }

    public class IdentityUser<TKey, TLogin, TRole, TClaim> : TableEntity,
        IUser<TKey>
        where TLogin : IdentityUserLogin<TKey>
        where TRole : IdentityUserRole<TKey>
        where TClaim : IdentityUserClaim<TKey>
    {
        public IdentityUser()
        {
            this.Claims = new List<TClaim>(10);
            this.Roles = new List<TRole>(10);
            this.Logins = new List<TLogin>(10);
        }

        public virtual int AccessFailedCount { get; set; }

        [Microsoft.WindowsAzure.Storage.Table.IgnoreProperty]
        public ICollection<TClaim> Claims { get; private set; }

        public virtual string Email { get; set; }

        public virtual bool EmailConfirmed { get; set; }

        [Microsoft.WindowsAzure.Storage.Table.IgnoreProperty]
        public virtual TKey Id { get; set; }

        public virtual bool LockoutEnabled { get; set; }

        public virtual DateTime? LockoutEndDateUtc { get; set; }

        [Microsoft.WindowsAzure.Storage.Table.IgnoreProperty]
        public ICollection<TLogin> Logins { get; private set; }

        public virtual string PasswordHash { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }

        [Microsoft.WindowsAzure.Storage.Table.IgnoreProperty]
        public ICollection<TRole> Roles { get; private set; }

        public virtual string SecurityStamp { get; set; }

        public virtual bool TwoFactorEnabled { get; set; }

        public virtual string UserName { get; set; }

    }

}
