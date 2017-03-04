using MongoDB.Driver;
using System;

namespace Common.DataAccess.Mongo.Connection
{
	public interface IConnect : IDisposable
	{
		MongoDatabaseSettings DatabaseSettings
		{
			get;
		}

		IMongoCollection<T> Collection<T>(string collectionName);
	}
}