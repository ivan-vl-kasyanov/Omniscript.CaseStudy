using System;
using System.Diagnostics;
using System.IO;

using Omniscript.CaseStudy.Server.DataAccess.DAO;

using SQLite;

namespace Omniscript.CaseStudy.Server.DataAccess.Clients
{
    /// <summary>
    /// Database client.
    /// </summary>
    public sealed class DatabaseClient : IDisposable
    {
        private const string DefaultDatabaseFileName = "Database.sqlite3";

        /// <summary>
        /// Database connection instance.
        /// </summary>
        public SQLiteConnection? Connection { get; private set; }

        private readonly object _disposeLock = new();

        private bool _disposedValue;

        /// <summary>
        /// Recreates database as new instance. Wipes out all old data.
        /// </summary>
        public void RecreateDatabase()
        {
            var pathToExe = Process
                .GetCurrentProcess()
                ?.MainModule
                ?.FileName;
            if (String.IsNullOrEmpty(pathToExe))
            {
                var exceptionMessage = "Unable to determine path to the executable.";

                throw new Exception(exceptionMessage);
            }
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            if (String.IsNullOrEmpty(pathToContentRoot))
            {
                var exceptionMessage = "Unable to determine root directory.";

                throw new Exception(exceptionMessage);
            }
            Directory.SetCurrentDirectory(pathToContentRoot);
            var dbFilePath = Path.Combine(
                pathToContentRoot,
                DefaultDatabaseFileName);

            if (File.Exists(dbFilePath))
            {
                try
                {
                    File.Delete(dbFilePath);
                }
                catch (Exception ex)
                {
                    var exceptionMessage =
                        "Unable to delete currently existing database file at: \"" +
                        dbFilePath +
                        "\".";

                    throw new Exception(
                        exceptionMessage,
                        ex);
                }
            }

            try
            {
                Connection = new SQLiteConnection(
                    dbFilePath,
                    SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite);
            }
            catch (Exception ex)
            {
                var exceptionMessage = "Unable to setup database connection.";

                throw new Exception(
                    exceptionMessage,
                    ex);
            }

            try
            {
                Connection.CreateTable<AddressDao>();
                Connection.CreateTable<CustomerDao>();
            }
            catch (Exception ex)
            {
                var exceptionMessage = "Unable to create database tables.";

                throw new Exception(
                    exceptionMessage,
                    ex);
            }
        }

        private void DisposeUnmanaged()
        {
            if (!_disposedValue)
            {
                lock (_disposeLock)
                {
                    DisposeConnection();
                }

                _disposedValue = true;
            }
        }

        private void DisposeConnection()
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Dispose();
                }
                catch { }
                Connection = null;
            }
        }

        /// <summary>
        /// Disposes unmanaged resources.
        /// </summary>
        ~DatabaseClient()
        {
            DisposeUnmanaged();
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public void Dispose()
        {
            DisposeUnmanaged();
            GC.SuppressFinalize(this);
        }
    }
}