using MongoDB.Driver;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Common.DataAccess.Mongo.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    [ComVisible(true)]
    [Serializable]
    public sealed class MongoCollectionName : Attribute
    {
        public string TableName
        {
            get;
            private set;
        }

        public MongoCollectionName(string tableName)
        {
            this.TableName = tableName;
        }
    }
}