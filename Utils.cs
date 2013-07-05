using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EFMembership
{
    public static class Utils
    {
        public static string HashPassword(string plainMessage)
        {
            var data = Encoding.UTF8.GetBytes(plainMessage);
            using (HashAlgorithm sha = new SHA256Managed())
            {
                sha.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(sha.Hash);
            }
        }

        public static string RandomPassword()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var password = string.Empty;
            var random = new Random();

            for (var i = 0; i < 8; i++)
            {
                var x = random.Next(1, chars.Length);
                if (!password.Contains(chars.GetValue(x).ToString()))
                {
                    password += chars.GetValue(x);
                }
                else
                {
                    i--;
                }
            }

            return password;
        }

        public static string GetConfigDataBaseName(string connStrName) 
        {
            var connStrConfig = ConfigurationManager.ConnectionStrings[connStrName];
            var connStr = connStrConfig.ConnectionString;
            var provider = connStrConfig.ProviderName;
            if (provider == "System.Data.SqlClient")
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connStr);
                return builder.InitialCatalog;
            }
            else if (provider == "Oracle.DataAccess.Client")
            {
                OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder(connStr);
                return builder.UserID;
            }
            else if (provider == "System.Data.EntityClient")
            {
                EntityConnectionStringBuilder builder = new EntityConnectionStringBuilder(connStr);
                if (builder.Provider == "System.Data.SqlClient")
                {
                    SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(builder.ProviderConnectionString);
                    return sb.InitialCatalog;
                }
                else if (builder.Provider == "Oracle.DataAccess.Client")
                {
                    OracleConnectionStringBuilder ob = new OracleConnectionStringBuilder(builder.ProviderConnectionString);
                    return ob.UserID;
                }
            }
            return "efmembership";
        }
    }
}
