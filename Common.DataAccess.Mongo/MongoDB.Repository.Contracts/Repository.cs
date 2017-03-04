using Common.DataAccess.Mongo.Attributes;
using Common.DataAccess.Mongo.Connection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Common.DataAccess.Mongo.Contracts
{
	[Serializable]
	public abstract class Repository<T> : IRepository<T>, IDisposable
	where T : class
	{
		private bool disposed;

		protected IMongoCollection<T> Collection
		{
			get;
			private set;
		}

		protected string CollectionName
		{
			get;
			private set;
		}

		protected IConnect Connect
		{
			get;
			private set;
		}

		public Repository(IConnect connect)
		{
			this.Connect = connect;
			MongoCollectionName mongoCollectionName = (MongoCollectionName)Attribute.GetCustomAttribute(typeof(T), typeof(MongoCollectionName));
			this.CollectionName = (mongoCollectionName != null ? mongoCollectionName.TableName : typeof(T).Name.ToLower());
			mongoCollectionName = null;
			this.Collection = this.Connect.Collection<T>(this.CollectionName);
		}

		public T Add(T model)
		{
			this.Collection.InsertOne(model, null, new CancellationToken());
			return model;
		}

		public T Add(T model, InsertOneOptions options)
		{
			this.Collection.InsertOne(model, options, new CancellationToken());
			return model;
		}

		public T Add(T model, InsertOneOptions options, CancellationToken cancellationToken)
		{
			this.Collection.InsertOne(model, options, cancellationToken);
			return model;
		}

		public IEnumerable<T> Add(IEnumerable<T> models)
		{
			this.Collection.InsertMany(models, null, new CancellationToken());
			return models;
		}

		public IEnumerable<T> Add(IEnumerable<T> models, InsertManyOptions options)
		{
			this.Collection.InsertMany(models, options, new CancellationToken());
			return models;
		}

		public IEnumerable<T> Add(IEnumerable<T> models, InsertManyOptions options, CancellationToken cancellationToken)
		{
			this.Collection.InsertMany(models, options, cancellationToken);
			return models;
		}

		public async Task<T> AddAsync(T model)
		{
			await this.Collection.InsertOneAsync(model, null, new CancellationToken());
			return model;
		}

		public async Task<T> AddAsync(T model, InsertOneOptions options)
		{
			await this.Collection.InsertOneAsync(model, options, new CancellationToken());
			return model;
		}

		public async Task<T> AddAsync(T model, InsertOneOptions options, CancellationToken cancellationToken)
		{
			await this.Collection.InsertOneAsync(model, options, cancellationToken);
			return model;
		}

		public async Task<IEnumerable<T>> AddAsync(IEnumerable<T> models)
		{
			await this.Collection.InsertManyAsync(models, null, new CancellationToken());
			return models;
		}

		public async Task<IEnumerable<T>> AddAsync(IEnumerable<T> models, InsertManyOptions options)
		{
			await this.Collection.InsertManyAsync(models, options, new CancellationToken());
			return models;
		}

		public async Task<IEnumerable<T>> AddAsync(IEnumerable<T> models, InsertManyOptions options, CancellationToken cancellationToken)
		{
			await this.Collection.InsertManyAsync(models, options, cancellationToken);
			return models;
		}

		public IList<T> All()
		{
			return IMongoCollectionExtensions.AsQueryable<T>(this.Collection).ToList<T>();
		}

		public IList<T> All<TKey>(Expression<Func<T, TKey>> orderBy)
		{
			return MongoQueryable.OrderBy<T, TKey>(IMongoCollectionExtensions.AsQueryable<T>(this.Collection), orderBy).ToList<T>();
		}

		public IList<T> All<TKey>(Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> where)
		{
			return MongoQueryable.OrderBy<T, TKey>(MongoQueryable.Where<T>(IMongoCollectionExtensions.AsQueryable<T>(this.Collection), where), orderBy).ToList<T>();
		}

		public IList<T> All<TKey>(int page, int total, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> where)
		{
			return MongoQueryable.Take<T>(MongoQueryable.Skip<T>(MongoQueryable.OrderBy<T, TKey>(MongoQueryable.Where<T>(IMongoCollectionExtensions.AsQueryable<T>(this.Collection), where), orderBy), (page - 1) * total), total).ToList<T>();
		}

		public async Task<IEnumerable<T>> AllAsync(Expression<Func<T, bool>> where, SortDefinition<T> sortBy = null)
		{
			IEnumerable<T> listAsync;
			CancellationToken cancellationToken;
			IFindFluent<T, T> findFluent = IMongoCollectionExtensions.Find<T>(this.Collection, where, null);
			if (sortBy == null)
			{
				IFindFluent<T, T> findFluent1 = findFluent;
				cancellationToken = new CancellationToken();
				listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent1, cancellationToken);
			}
			else
			{
				IFindFluent<T, T> findFluent2 = findFluent.Sort(sortBy);
				cancellationToken = new CancellationToken();
				listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent2, cancellationToken);
			}
			return listAsync;
		}

		public async Task<IEnumerable<T>> AllAsync(FilterDefinition<T> query, SortDefinition<T> sortBy = null)
		{
			IEnumerable<T> listAsync;
			CancellationToken cancellationToken;
			IFindFluent<T, T> findFluent = IMongoCollectionExtensions.Find<T>(this.Collection, query, null);
			if (sortBy == null)
			{
				IFindFluent<T, T> findFluent1 = findFluent;
				cancellationToken = new CancellationToken();
				listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent1, cancellationToken);
			}
			else
			{
				IFindFluent<T, T> findFluent2 = findFluent.Sort(sortBy);
				cancellationToken = new CancellationToken();
				listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent2, cancellationToken);
			}
			return listAsync;
		}

		public async Task<IEnumerable<T>> AllAsync(SortDefinition<T> sortBy)
		{
			IFindFluent<T, T> findFluent = IMongoCollectionExtensions.Find<T>(this.Collection, new BsonDocument(), null).Sort(sortBy);
			return await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent, new CancellationToken());
		}

		public async Task<IEnumerable<T>> AllAsync()
		{
			IFindFluent<T, T> findFluent = IMongoCollectionExtensions.Find<T>(this.Collection, new BsonDocument(), null);
			return await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent, new CancellationToken());
		}

		public async Task<IEnumerable<T>> AllAsync<TKey>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy)
		{
			IOrderedMongoQueryable<T> orderedMongoQueryable = MongoQueryable.OrderBy<T, TKey>(MongoQueryable.Where<T>(IMongoCollectionExtensions.AsQueryable<T>(this.Collection), where), orderBy);
			return await IAsyncCursorSourceExtensions.ToListAsync<T>(orderedMongoQueryable, new CancellationToken());
		}

		public async Task<IEnumerable<T>> AllAsync<TKey>(int page, int total, Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy)
		{
			IOrderedMongoQueryable<T> orderedMongoQueryable = MongoQueryable.OrderBy<T, TKey>(MongoQueryable.Take<T>(MongoQueryable.Skip<T>(MongoQueryable.Where<T>(IMongoCollectionExtensions.AsQueryable<T>(this.Collection), where), (page - 1) * total), total), orderBy);
			return await IAsyncCursorSourceExtensions.ToListAsync<T>(orderedMongoQueryable, new CancellationToken());
		}

		public long Count()
		{
			return IMongoCollectionExtensions.AsQueryable<T>(this.Collection).LongCount<T>();
		}

		public long Count(Expression<Func<T, bool>> query)
		{
			return IMongoCollectionExtensions.AsQueryable<T>(this.Collection).LongCount<T>(query);
		}

		public async Task<long> CountAsync()
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> bsonDocument = new BsonDocument();
			return await collection.CountAsync(bsonDocument, null, new CancellationToken());
		}

		public async Task<long> CountAsync(FilterDefinition<T> query)
		{
			IMongoCollection<T> collection = this.Collection;
			return await collection.CountAsync(query, null, new CancellationToken());
		}

		public async Task<long> CountAsync(FilterDefinition<T> query, CountOptions options)
		{
			IMongoCollection<T> collection = this.Collection;
			return await collection.CountAsync(query, options, new CancellationToken());
		}

		public async Task<long> CountAsync(FilterDefinition<T> query, CountOptions options, CancellationToken cancellationToken)
		{
			return await this.Collection.CountAsync(query, options, cancellationToken);
		}

		public async Task<long> CountAsync(Expression<Func<T, bool>> query)
		{
			IMongoCollection<T> collection = this.Collection;
			return await collection.CountAsync(query, null, new CancellationToken());
		}

		public async Task<long> CountAsync(Expression<Func<T, bool>> query, CountOptions options)
		{
			IMongoCollection<T> collection = this.Collection;
			return await collection.CountAsync(query, options, new CancellationToken());
		}

		public async Task<long> CountAsync(Expression<Func<T, bool>> query, CountOptions options, CancellationToken cancellationToken)
		{
			return await this.Collection.CountAsync(query, options, cancellationToken);
		}

		public T Create()
		{
			return Activator.CreateInstance<T>();
		}

		public ObjectId CreateObjectId(string value)
		{
			return ObjectId.Parse(value);
		}

		public DeleteResult Delete(Expression<Func<T, bool>> where)
		{
			return this.Collection.DeleteOne(where, new CancellationToken());
		}

		public DeleteResult Delete(Expression<Func<T, bool>> where, CancellationToken cancellationToken)
		{
			return this.Collection.DeleteOne(where, cancellationToken);
		}

		public DeleteResult Delete(FilterDefinition<T> query)
		{
			return this.Collection.DeleteOne(query, new CancellationToken());
		}

		public DeleteResult Delete(FilterDefinition<T> query, CancellationToken cancellationToken)
		{
			return this.Collection.DeleteOne(query, cancellationToken);
		}

		public async Task<DeleteResult> DeleteAsync(Expression<Func<T, bool>> where)
		{
			return await this.Collection.DeleteOneAsync(where, new CancellationToken());
		}

		public async Task<DeleteResult> DeleteAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken)
		{
			return await this.Collection.DeleteOneAsync(where, cancellationToken);
		}

		public async Task<DeleteResult> DeleteAsync(FilterDefinition<T> query)
		{
			return await this.Collection.DeleteOneAsync(query, new CancellationToken());
		}

		public async Task<DeleteResult> DeleteAsync(FilterDefinition<T> query, CancellationToken cancellationToken)
		{
			return await this.Collection.DeleteOneAsync(query, cancellationToken);
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
					this.Collection = null;
					this.Connect = null;
				}
				this.disposed = true;
			}
		}

		public ReplaceOneResult Edit(Expression<Func<T, bool>> where, T model)
		{
			return this.Collection.ReplaceOne(where, model, null, new CancellationToken());
		}

		public ReplaceOneResult Edit(Expression<Func<T, bool>> where, T model, UpdateOptions options)
		{
			return this.Collection.ReplaceOne(where, model, options, new CancellationToken());
		}

		public ReplaceOneResult Edit(Expression<Func<T, bool>> where, T model, UpdateOptions options, CancellationToken cancellationToken)
		{
			return this.Collection.ReplaceOne(where, model, options, cancellationToken);
		}

		public ReplaceOneResult Edit(FilterDefinition<T> query, T model)
		{
			return this.Collection.ReplaceOne(query, model, null, new CancellationToken());
		}

		public ReplaceOneResult Edit(FilterDefinition<T> query, T model, UpdateOptions options)
		{
			return this.Collection.ReplaceOne(query, model, options, new CancellationToken());
		}

		public ReplaceOneResult Edit(FilterDefinition<T> query, T model, UpdateOptions options, CancellationToken cancellationToken)
		{
			return this.Collection.ReplaceOne(query, model, options, cancellationToken);
		}

		public async Task<ReplaceOneResult> EditAsync(Expression<Func<T, bool>> where, T model)
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = where;
			T t = model;
			CancellationToken cancellationToken = new CancellationToken();
			return await collection.ReplaceOneAsync(filterDefinition, t, null, cancellationToken);
		}

		public async Task<ReplaceOneResult> EditAsync(Expression<Func<T, bool>> where, T model, UpdateOptions options)
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = where;
			T t = model;
			UpdateOptions updateOption = options;
			CancellationToken cancellationToken = new CancellationToken();
			return await collection.ReplaceOneAsync(filterDefinition, t, updateOption, cancellationToken);
		}

		public async Task<ReplaceOneResult> EditAsync(Expression<Func<T, bool>> where, T model, UpdateOptions options, CancellationToken cancellationToken)
		{
			ReplaceOneResult replaceOneResult = await this.Collection.ReplaceOneAsync(where, model, options, cancellationToken);
			return replaceOneResult;
		}

		public async Task<ReplaceOneResult> EditAsync(FilterDefinition<T> query, T model)
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = query;
			T t = model;
			CancellationToken cancellationToken = new CancellationToken();
			return await collection.ReplaceOneAsync(filterDefinition, t, null, cancellationToken);
		}

		public async Task<ReplaceOneResult> EditAsync(FilterDefinition<T> query, T model, UpdateOptions options)
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = query;
			T t = model;
			UpdateOptions updateOption = options;
			CancellationToken cancellationToken = new CancellationToken();
			return await collection.ReplaceOneAsync(filterDefinition, t, updateOption, cancellationToken);
		}

		public async Task<ReplaceOneResult> EditAsync(FilterDefinition<T> query, T model, UpdateOptions options, CancellationToken cancellationToken)
		{
			ReplaceOneResult replaceOneResult = await this.Collection.ReplaceOneAsync(query, model, options, cancellationToken);
			return replaceOneResult;
		}

		~Repository()
		{
			this.Dispose(false);
		}

		public T Find(Expression<Func<T, bool>> where)
		{
			return MongoQueryable.Where<T>(IMongoCollectionExtensions.AsQueryable<T>(this.Collection), where).FirstOrDefault<T>();
		}

		public async Task<T> FindAsync(ObjectId id)
		{
			return await this.FindAsync<ObjectId>(id, "_id");
		}

		public async Task<T> FindAsync<TKey>(TKey id, string name = "_id")
		{
			FilterDefinition<T> filterDefinition = Builders<T>.Filter.Eq<TKey>(name, id);
			IFindFluent<T, T> findFluent = IMongoCollectionExtensions.Find<T>(this.Collection, filterDefinition, null);
			return await IFindFluentExtensions.FirstOrDefaultAsync<T, T>(findFluent, new CancellationToken());
		}

		public async Task<T> FindAsync(FilterDefinition<T> query)
		{
			IFindFluent<T, T> findFluent = IMongoCollectionExtensions.Find<T>(this.Collection, query, null);
			return await IFindFluentExtensions.FirstOrDefaultAsync<T, T>(findFluent, new CancellationToken());
		}

		public async Task<T> FindAsync(Expression<Func<T, bool>> where)
		{
			IFindFluent<T, T> findFluent = IMongoCollectionExtensions.Find<T>(this.Collection, where, null);
			return await IFindFluentExtensions.FirstOrDefaultAsync<T, T>(findFluent, new CancellationToken());
		}

		public async Task<IAsyncCursor<T>> FindAsync(FilterDefinition<T> query, FindOptions<T, T> options)
		{
			IMongoCollection<T> collection = this.Collection;
			return await collection.FindAsync<T>(query, options, new CancellationToken());
		}

		public async Task<IAsyncCursor<T>> FindAsync(FilterDefinition<T> query, FindOptions<T, T> options, CancellationToken cancellationToken)
		{
			return await this.Collection.FindAsync<T>(query, options, cancellationToken);
		}

		public async Task<IAsyncCursor<T>> FindAsync(Expression<Func<T, bool>> where, FindOptions<T, T> options)
		{
			IMongoCollection<T> collection = this.Collection;
			return await collection.FindAsync<T>(where, options, new CancellationToken());
		}

		public async Task<IAsyncCursor<T>> FindAsync(Expression<Func<T, bool>> where, FindOptions<T, T> options, CancellationToken cancellationToken)
		{
			return await this.Collection.FindAsync<T>(where, options, cancellationToken);
		}

		public IMongoCollection<T> MongoCollection()
		{
			return this.Collection;
		}

		public IConnect MongoConnect()
		{
			return this.Connect;
		}

		public StaticPagedList<T> Pagination<TKey>(int page, int total, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> where = null)
		{
			long count = (long)0;
			IMongoQueryable<T> model = IMongoCollectionExtensions.AsQueryable<T>(this.Collection);
			count = (where == null ? this.Count() : this.Count(where));
			if (where != null)
			{
				model = MongoQueryable.Where<T>(model, where);
			}
			model = MongoQueryable.Take<T>(MongoQueryable.Skip<T>(MongoQueryable.OrderBy<T, TKey>(model, orderBy), (page - 1) * total), total);
			return new StaticPagedList<T>(model.ToList<T>(), page, total, (int)count);
		}

		public async Task<StaticPagedList<T>> PaginationAsync(int page, int total, SortDefinition<T> sortBy, Expression<Func<T, bool>> query = null)
		{
			StaticPagedList<T> staticPagedList;
			CancellationToken cancellationToken;
			long num = (long)0;
			if (query != null)
			{
				IMongoCollection<T> collection = this.Collection;
				FilterDefinition<T> filterDefinition = query;
				cancellationToken = new CancellationToken();
				num = await collection.CountAsync(filterDefinition, null, cancellationToken);
				IFindFluent<T, T> findFluent = IMongoCollectionExtensions.Find<T>(this.Collection, query, null).Skip(new int?((page - 1) * total)).Limit(new int?(total)).Sort(sortBy);
				cancellationToken = new CancellationToken();
				List<T> listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent, cancellationToken);
				staticPagedList = new StaticPagedList<T>(listAsync, page, total, (int)num);
			}
			else
			{
				IMongoCollection<T> mongoCollection = this.Collection;
				FilterDefinition<T> bsonDocument = new BsonDocument();
				cancellationToken = new CancellationToken();
				num = await mongoCollection.CountAsync(bsonDocument, null, cancellationToken);
				IFindFluent<T, T> findFluent1 = IMongoCollectionExtensions.Find<T>(this.Collection, new BsonDocument(), null).Skip(new int?((page - 1) * total)).Limit(new int?(total)).Sort(sortBy);
				cancellationToken = new CancellationToken();
				List<T> ts = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent1, cancellationToken);
				staticPagedList = new StaticPagedList<T>(ts, page, total, (int)num);
			}
			return staticPagedList;
		}

		public async Task<StaticPagedList<T>> PaginationAsync(int page, int total, SortDefinition<T> sortBy, FilterDefinition<T> query)
		{
			long num = (long)0;
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = query;
			CancellationToken cancellationToken = new CancellationToken();
			num = await collection.CountAsync(filterDefinition, null, cancellationToken);
			IFindFluent<T, T> findFluent = IMongoCollectionExtensions.Find<T>(this.Collection, query, null).Skip(new int?((page - 1) * total)).Limit(new int?(total)).Sort(sortBy);
			cancellationToken = new CancellationToken();
			List<T> listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent, cancellationToken);
			return new StaticPagedList<T>(listAsync, page, total, (int)num);
		}

		public async Task<StaticPagedList<T>> PaginationAsync<TKey>(int page, int total, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> where = null)
		{
			long num;
			long num1 = (long)0;
			IMongoQueryable<T> mongoQueryable = IMongoCollectionExtensions.AsQueryable<T>(this.Collection);
			num = (where != null ? await this.CountAsync(where) : await this.CountAsync());
			num1 = num;
			if (where != null)
			{
				mongoQueryable = MongoQueryable.Where<T>(mongoQueryable, where);
			}
			mongoQueryable = MongoQueryable.Take<T>(MongoQueryable.Skip<T>(MongoQueryable.OrderBy<T, TKey>(mongoQueryable, orderBy), (page - 1) * total), total);
			StaticPagedList<T> staticPagedList = new StaticPagedList<T>(mongoQueryable.ToList<T>(), page, total, (int)num1);
			return staticPagedList;
		}

		public IMongoQueryable<T> Query()
		{
			return IMongoCollectionExtensions.AsQueryable<T>(this.Collection);
		}

		public IMongoQueryable<T> Query(params Expression<Func<T, bool>>[] where)
		{
			IMongoQueryable<T> _query = this.Query();
			if (where.Count<Expression<Func<T, bool>>>() > 0)
			{
				Expression<Func<T, bool>>[] expressionArray = where;
				for (int i = 0; i < (int)expressionArray.Length; i++)
				{
					_query = MongoQueryable.Where<T>(_query, expressionArray[i]);
				}
			}
			return _query;
		}

		public IMongoQueryable<T> Query<TKey>(Expression<Func<T, TKey>> orderBy, params Expression<Func<T, bool>>[] where)
		{
			return MongoQueryable.OrderBy<T, TKey>(this.Query(where), orderBy);
		}

		public DeleteResult Remove(Expression<Func<T, bool>> where)
		{
			return this.Collection.DeleteMany(where, new CancellationToken());
		}

		public DeleteResult Remove(Expression<Func<T, bool>> where, CancellationToken cancellationToken)
		{
			return this.Collection.DeleteMany(where, cancellationToken);
		}

		public DeleteResult Remove(FilterDefinition<T> query)
		{
			return this.Collection.DeleteMany(query, new CancellationToken());
		}

		public DeleteResult Remove(FilterDefinition<T> query, CancellationToken cancellationToken)
		{
			return this.Collection.DeleteMany(query, cancellationToken);
		}

		public async Task<DeleteResult> RemoveAsync(Expression<Func<T, bool>> where)
		{
			return await this.Collection.DeleteManyAsync(where, new CancellationToken());
		}

		public async Task<DeleteResult> RemoveAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken)
		{
			return await this.Collection.DeleteManyAsync(where, cancellationToken);
		}

		public async Task<DeleteResult> RemoveAsync(FilterDefinition<T> query)
		{
			return await this.Collection.DeleteManyAsync(query, new CancellationToken());
		}

		public async Task<DeleteResult> RemoveAsync(FilterDefinition<T> query, CancellationToken cancellationToken)
		{
			return await this.Collection.DeleteManyAsync(query, cancellationToken);
		}

		public UpdateResult Replace(Expression<Func<T, bool>> where, UpdateDefinition<T> update)
		{
			return this.Collection.UpdateMany(where, update, null, new CancellationToken());
		}

		public UpdateResult Replace(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options)
		{
			return this.Collection.UpdateMany(where, update, options, new CancellationToken());
		}

		public UpdateResult Replace(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken)
		{
			return this.Collection.UpdateMany(where, update, options, cancellationToken);
		}

		public UpdateResult Replace(FilterDefinition<T> query, UpdateDefinition<T> update)
		{
			return this.Collection.UpdateMany(query, update, null, new CancellationToken());
		}

		public UpdateResult Replace(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options)
		{
			return this.Collection.UpdateMany(query, update, options, new CancellationToken());
		}

		public UpdateResult Replace(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken)
		{
			return this.Collection.UpdateMany(query, update, options, cancellationToken);
		}

		public async Task<UpdateResult> ReplaceAsync(Expression<Func<T, bool>> where, UpdateDefinition<T> update)
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = where;
			UpdateDefinition<T> updateDefinition = update;
			CancellationToken cancellationToken = new CancellationToken();
			return await collection.UpdateManyAsync(filterDefinition, updateDefinition, null, cancellationToken);
		}

		public async Task<UpdateResult> ReplaceAsync(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options)
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = where;
			UpdateDefinition<T> updateDefinition = update;
			UpdateOptions updateOption = options;
			CancellationToken cancellationToken = new CancellationToken();
			return await collection.UpdateManyAsync(filterDefinition, updateDefinition, updateOption, cancellationToken);
		}

		public async Task<UpdateResult> ReplaceAsync(FilterDefinition<T> query, UpdateDefinition<T> update)
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = query;
			UpdateDefinition<T> updateDefinition = update;
			CancellationToken cancellationToken = new CancellationToken();
			return await collection.UpdateManyAsync(filterDefinition, updateDefinition, null, cancellationToken);
		}

		public async Task<UpdateResult> ReplaceAsync(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options)
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = query;
			UpdateDefinition<T> updateDefinition = update;
			UpdateOptions updateOption = options;
			CancellationToken cancellationToken = new CancellationToken();
			return await collection.UpdateManyAsync(filterDefinition, updateDefinition, updateOption, cancellationToken);
		}

		public async Task<UpdateResult> ReplaceAsync(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken)
		{
			UpdateResult updateResult = await this.Collection.UpdateManyAsync(where, update, options, cancellationToken);
			return updateResult;
		}

		public async Task<UpdateResult> ReplaceAsync(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken)
		{
			UpdateResult updateResult = await this.Collection.UpdateManyAsync(query, update, options, cancellationToken);
			return updateResult;
		}

		public UpdateResult Update(Expression<Func<T, bool>> where, UpdateDefinition<T> update)
		{
			return this.Collection.UpdateOne(where, update, null, new CancellationToken());
		}

		public UpdateResult Update(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options)
		{
			return this.Collection.UpdateOne(where, update, options, new CancellationToken());
		}

		public UpdateResult Update(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken)
		{
			return this.Collection.UpdateOne(where, update, options, cancellationToken);
		}

		public UpdateResult Update(FilterDefinition<T> query, UpdateDefinition<T> update)
		{
			return this.Collection.UpdateOne(query, update, null, new CancellationToken());
		}

		public UpdateResult Update(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options)
		{
			return this.Collection.UpdateOne(query, update, options, new CancellationToken());
		}

		public UpdateResult Update(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken)
		{
			return this.Collection.UpdateOne(query, update, options, cancellationToken);
		}

		public async Task<UpdateResult> UpdateAsync(Expression<Func<T, bool>> where, UpdateDefinition<T> update)
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = where;
			UpdateDefinition<T> updateDefinition = update;
			CancellationToken cancellationToken = new CancellationToken();
			return await collection.UpdateOneAsync(filterDefinition, updateDefinition, null, cancellationToken);
		}

		public async Task<UpdateResult> UpdateAsync(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options)
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = where;
			UpdateDefinition<T> updateDefinition = update;
			UpdateOptions updateOption = options;
			CancellationToken cancellationToken = new CancellationToken();
			return await collection.UpdateOneAsync(filterDefinition, updateDefinition, updateOption, cancellationToken);
		}

		public async Task<UpdateResult> UpdateAsync(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken)
		{
			UpdateResult updateResult = await this.Collection.UpdateOneAsync(where, update, options, cancellationToken);
			return updateResult;
		}

		public async Task<UpdateResult> UpdateAsync(FilterDefinition<T> query, UpdateDefinition<T> update)
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = query;
			UpdateDefinition<T> updateDefinition = update;
			CancellationToken cancellationToken = new CancellationToken();
			return await collection.UpdateOneAsync(filterDefinition, updateDefinition, null, cancellationToken);
		}

		public async Task<UpdateResult> UpdateAsync(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options)
		{
			IMongoCollection<T> collection = this.Collection;
			FilterDefinition<T> filterDefinition = query;
			UpdateDefinition<T> updateDefinition = update;
			UpdateOptions updateOption = options;
			CancellationToken cancellationToken = new CancellationToken();
			return await collection.UpdateOneAsync(filterDefinition, updateDefinition, updateOption, cancellationToken);
		}

		public async Task<UpdateResult> UpdateAsync(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken)
		{
			UpdateResult updateResult = await this.Collection.UpdateOneAsync(query, update, options, cancellationToken);
			return updateResult;
		}

        public UpdateResult UpdateMany(FilterDefinition<T> filer, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken)
        {
            UpdateResult updateResult = this.Collection.UpdateMany(filer, update,options,cancellationToken);
            return updateResult;
        }
    }
}