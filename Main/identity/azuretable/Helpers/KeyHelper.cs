﻿// MIT License Copyright 2014 (c) David Melendez. All rights reserved. See License.txt in the project root for license information.

using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ElCamino.AspNet.Identity.AzureTable.Helpers
{
    public static class KeyHelper
    {
        private static BaseKeyHelper baseHelper = new UriEncodeKeyHelper();

        public static string GenerateRowKeyUserLoginInfo(this UserLoginInfo info)
        {
            return baseHelper.GenerateRowKeyUserLoginInfo(info);
        }

        public static string GeneratePartitionKeyIndexByLogin(string plainProvider)
        {
            return baseHelper.GeneratePartitionKeyIndexByLogin(plainProvider);
        }

        public static string GenerateRowKeyUserEmail(string plainEmail)
        {
            return baseHelper.GenerateRowKeyUserEmail(plainEmail);
        }

        public static string GeneratePartitionKeyIndexByEmail(string plainEmail)
        {
            return baseHelper.GeneratePartitionKeyIndexByEmail(plainEmail);
        }

        public static string GenerateRowKeyUserName(string plainUserName)
        {
            return baseHelper.GenerateRowKeyUserName(plainUserName);
        }

        public static string GenerateRowKeyIdentityUserRole(string plainRoleName)
        {
            return baseHelper.GenerateRowKeyIdentityUserRole(plainRoleName);
        }

        public static string GenerateRowKeyIdentityRole(string plainRoleName)
        {
            return baseHelper.GenerateRowKeyIdentityRole(plainRoleName);
        }

        public static string GeneratePartitionKeyIdentityRole(string plainRoleName)
        {
            return baseHelper.GeneratePartitionKeyIdentityRole(plainRoleName);
        }

        public static string GenerateRowKeyIdentityUserClaim(string claimType, string claimValue)
        {
            return baseHelper.GenerateRowKeyIdentityUserClaim(claimType, claimValue);
        }

        public static string GenerateRowKeyIdentityUserLogin(string loginProvider, string providerKey)
        {
            return baseHelper.GenerateRowKeyIdentityUserLogin(loginProvider, providerKey);
        }

        public static string ParsePartitionKeyIdentityRoleFromRowKey(string rowKey)
        {
            return baseHelper.ParsePartitionKeyIdentityRoleFromRowKey(rowKey);
        }

        public static double KeyVersion { get { return baseHelper.KeyVersion; } }
    }
}
