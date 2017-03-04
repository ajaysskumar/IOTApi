using MongoDB.Driver;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Common.DataAccess.Mongo.Connection
{
	[Serializable]
	public class Connect : IDisposable, IConnect
	{
		private bool disposed;

		protected MongoClient Client
		{
			get;
			private set;
		}

		protected IMongoDatabase DataBase
		{
			get;
			private set;
		}

		public MongoDatabaseSettings DatabaseSettings
		{
			get
			{
				return JustDecompileGenerated_get_DatabaseSettings();
			}
			set
			{
				JustDecompileGenerated_set_DatabaseSettings(value);
			}
		}

		private MongoDatabaseSettings JustDecompileGenerated_DatabaseSettings_k__BackingField;

		public MongoDatabaseSettings JustDecompileGenerated_get_DatabaseSettings()
		{
			return this.JustDecompileGenerated_DatabaseSettings_k__BackingField;
		}

		private void JustDecompileGenerated_set_DatabaseSettings(MongoDatabaseSettings value)
		{
			this.JustDecompileGenerated_DatabaseSettings_k__BackingField = value;
		}

		public Connect()
		{
			this.Client = new MongoClient(ConfigurationManager.AppSettings["MongoConnectionString"]);
			this.DataBase = this.Client.GetDatabase(ConfigurationManager.AppSettings["MongoDatabase"], null);
            this.DatabaseSettings = this.DataBase.Settings;
		}

		public Connect(MongoDatabaseSettings databaseSettings)
		{
			this.Client = new MongoClient(ConfigurationManager.AppSettings["MongoConnectionString"]);
			this.DataBase = this.Client.GetDatabase(ConfigurationManager.AppSettings["MongoDatabase"], databaseSettings);
            this.DatabaseSettings = this.DataBase.Settings;
		}

		public Connect(string connectionString, string databaseName)
		{
			this.Client = new MongoClient(connectionString);
			this.DataBase = this.Client.GetDatabase(databaseName, null);
            this.DatabaseSettings = this.DataBase.Settings;
		}

		public Connect(string connectionString, string databaseName, MongoDatabaseSettings databaseSettings)
		{
			this.Client = new MongoClient(connectionString);
			this.DataBase = this.Client.GetDatabase(databaseName, databaseSettings);
			this.DatabaseSettings = databaseSettings;
		}

		public IMongoCollection<T> Collection<T>(string collectionName)
		{
			return this.DataBase.GetCollection<T>(collectionName, null);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.DataBase = null;
					this.Client = null;
				}
				this.disposed = true;
			}
		}

		~Connect()
		{
			this.Dispose(false);
		}
	}
}