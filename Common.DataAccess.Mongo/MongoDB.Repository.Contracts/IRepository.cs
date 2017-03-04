using Common.DataAccess.Mongo.Connection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Common.DataAccess.Mongo.Contracts
{
	public interface IRepository<T> : IDisposable
	where T : class
	{
		T Add(T model);

		T Add(T model, InsertOneOptions options);

		T Add(T model, InsertOneOptions options, CancellationToken cancellationToken);

		IEnumerable<T> Add(IEnumerable<T> models);

		IEnumerable<T> Add(IEnumerable<T> models, InsertManyOptions options);

		IEnumerable<T> Add(IEnumerable<T> models, InsertManyOptions options, CancellationToken cancellationToken);

		Task<T> AddAsync(T model);

		Task<T> AddAsync(T model, InsertOneOptions options);

		Task<T> AddAsync(T model, InsertOneOptions options, CancellationToken cancellationToken);

		Task<IEnumerable<T>> AddAsync(IEnumerable<T> models);

		Task<IEnumerable<T>> AddAsync(IEnumerable<T> models, InsertManyOptions options);

		Task<IEnumerable<T>> AddAsync(IEnumerable<T> models, InsertManyOptions options, CancellationToken cancellationToken);

		IList<T> All();

		IList<T> All<Tkey>(Expression<Func<T, Tkey>> orderBy);

		IList<T> All<TKey>(Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> where);

		IList<T> All<TKey>(int Page, int Total, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> where);

		Task<IEnumerable<T>> AllAsync();

		Task<IEnumerable<T>> AllAsync(SortDefinition<T> sortBy);

		Task<IEnumerable<T>> AllAsync(FilterDefinition<T> query, SortDefinition<T> sortBy = null);

		Task<IEnumerable<T>> AllAsync(Expression<Func<T, bool>> where, SortDefinition<T> sortBy = null);

		Task<IEnumerable<T>> AllAsync<TKey>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy);

		Task<IEnumerable<T>> AllAsync<TKey>(int page, int total, Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy);

		long Count();

		long Count(Expression<Func<T, bool>> where);

		Task<long> CountAsync();

		Task<long> CountAsync(FilterDefinition<T> query);

		Task<long> CountAsync(FilterDefinition<T> query, CountOptions options);

		Task<long> CountAsync(FilterDefinition<T> query, CountOptions options, CancellationToken cancellationToken);

		Task<long> CountAsync(Expression<Func<T, bool>> where);

		Task<long> CountAsync(Expression<Func<T, bool>> where, CountOptions options);

		Task<long> CountAsync(Expression<Func<T, bool>> where, CountOptions options, CancellationToken cancellationToken);

		T Create();

		ObjectId CreateObjectId(string value);

		DeleteResult Delete(Expression<Func<T, bool>> where);

		DeleteResult Delete(Expression<Func<T, bool>> where, CancellationToken cancellationToken);

		DeleteResult Delete(FilterDefinition<T> query);

		DeleteResult Delete(FilterDefinition<T> query, CancellationToken cancellationToken);

		Task<DeleteResult> DeleteAsync(Expression<Func<T, bool>> where);

		Task<DeleteResult> DeleteAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken);

		Task<DeleteResult> DeleteAsync(FilterDefinition<T> query);

		Task<DeleteResult> DeleteAsync(FilterDefinition<T> query, CancellationToken cancellationToken);

		ReplaceOneResult Edit(FilterDefinition<T> query, T model);

		ReplaceOneResult Edit(FilterDefinition<T> query, T model, UpdateOptions options);

		ReplaceOneResult Edit(FilterDefinition<T> query, T model, UpdateOptions options, CancellationToken cancellationToken);

		ReplaceOneResult Edit(Expression<Func<T, bool>> where, T model);

		ReplaceOneResult Edit(Expression<Func<T, bool>> where, T model, UpdateOptions options);

		ReplaceOneResult Edit(Expression<Func<T, bool>> where, T model, UpdateOptions options, CancellationToken cancellationToken);

		Task<ReplaceOneResult> EditAsync(Expression<Func<T, bool>> where, T model);

		Task<ReplaceOneResult> EditAsync(Expression<Func<T, bool>> where, T model, UpdateOptions options);

		Task<ReplaceOneResult> EditAsync(Expression<Func<T, bool>> where, T model, UpdateOptions options, CancellationToken cancellationToken);

		Task<ReplaceOneResult> EditAsync(FilterDefinition<T> query, T model);

		Task<ReplaceOneResult> EditAsync(FilterDefinition<T> query, T model, UpdateOptions options);

		Task<ReplaceOneResult> EditAsync(FilterDefinition<T> query, T model, UpdateOptions options, CancellationToken cancellationToken);

		T Find(Expression<Func<T, bool>> where);

		Task<T> FindAsync(ObjectId id);

		Task<T> FindAsync<TKey>(TKey id, string Name = "_id");

		Task<T> FindAsync(FilterDefinition<T> query);

		Task<T> FindAsync(Expression<Func<T, bool>> where);

		Task<IAsyncCursor<T>> FindAsync(FilterDefinition<T> query, FindOptions<T, T> options);

		Task<IAsyncCursor<T>> FindAsync(FilterDefinition<T> query, FindOptions<T, T> options, CancellationToken cancellationToken);

		Task<IAsyncCursor<T>> FindAsync(Expression<Func<T, bool>> where, FindOptions<T, T> options);

		Task<IAsyncCursor<T>> FindAsync(Expression<Func<T, bool>> where, FindOptions<T, T> options, CancellationToken cancellationToken);

		IMongoCollection<T> MongoCollection();

		IConnect MongoConnect();

		StaticPagedList<T> Pagination<TKey>(int page, int total, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> where = null);

		Task<StaticPagedList<T>> PaginationAsync<TKey>(int page, int total, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> where = null);

		Task<StaticPagedList<T>> PaginationAsync(int page, int total, SortDefinition<T> sortBy, Expression<Func<T, bool>> where = null);

		Task<StaticPagedList<T>> PaginationAsync(int page, int total, SortDefinition<T> sortBy, FilterDefinition<T> query);

		IMongoQueryable<T> Query();

		IMongoQueryable<T> Query(params Expression<Func<T, bool>>[] where);

		IMongoQueryable<T> Query<TKey>(Expression<Func<T, TKey>> orderBy, params Expression<Func<T, bool>>[] where);

		DeleteResult Remove(Expression<Func<T, bool>> where);

		DeleteResult Remove(Expression<Func<T, bool>> where, CancellationToken cancellationToken);

		DeleteResult Remove(FilterDefinition<T> query);

		DeleteResult Remove(FilterDefinition<T> query, CancellationToken cancellationToken);

		Task<DeleteResult> RemoveAsync(Expression<Func<T, bool>> where);

		Task<DeleteResult> RemoveAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken);

		Task<DeleteResult> RemoveAsync(FilterDefinition<T> query);

		Task<DeleteResult> RemoveAsync(FilterDefinition<T> query, CancellationToken cancellationToken);

		UpdateResult Replace(Expression<Func<T, bool>> where, UpdateDefinition<T> update);

		UpdateResult Replace(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options);

		UpdateResult Replace(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken);

		UpdateResult Replace(FilterDefinition<T> query, UpdateDefinition<T> update);

		UpdateResult Replace(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options);

		UpdateResult Replace(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken);

		Task<UpdateResult> ReplaceAsync(Expression<Func<T, bool>> where, UpdateDefinition<T> update);

		Task<UpdateResult> ReplaceAsync(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options);

		Task<UpdateResult> ReplaceAsync(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken);

		Task<UpdateResult> ReplaceAsync(FilterDefinition<T> query, UpdateDefinition<T> update);

		Task<UpdateResult> ReplaceAsync(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options);

		Task<UpdateResult> ReplaceAsync(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken);

		UpdateResult Update(Expression<Func<T, bool>> where, UpdateDefinition<T> update);

		UpdateResult Update(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options);

		UpdateResult Update(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken);

		UpdateResult Update(FilterDefinition<T> query, UpdateDefinition<T> update);

		UpdateResult Update(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options);

		UpdateResult Update(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken);

		Task<UpdateResult> UpdateAsync(Expression<Func<T, bool>> where, UpdateDefinition<T> update);

		Task<UpdateResult> UpdateAsync(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options);

		Task<UpdateResult> UpdateAsync(Expression<Func<T, bool>> where, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken);

		Task<UpdateResult> UpdateAsync(FilterDefinition<T> query, UpdateDefinition<T> update);

		Task<UpdateResult> UpdateAsync(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options);

		Task<UpdateResult> UpdateAsync(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken);

		UpdateResult UpdateMany(FilterDefinition<T> query, UpdateDefinition<T> update, UpdateOptions options, CancellationToken cancellationToken);
	}
}