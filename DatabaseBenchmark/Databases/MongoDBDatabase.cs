﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using STS.General.Extensions;
using STS.General.Generators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DatabaseBenchmark.Databases
{
    public class MongoDBDatabase : Database
    {
        private MongoClient[] clients;
        private MongoCollection<Row>[] collections;

        private MongoServer server;
        private MongoDatabase database;

        /// <summary>
        /// Specifies how many records are inserted with every batch.
        /// </summary>
        public int InsertsPerQuery { get; set; }

        public MongoDBDatabase()
        {
            Name = "MongoDB";
            CollectionName = "collection";
            Category = "NoSQL\\Document Store";
            Description = "MongoDB C# Driver 2.0.0beta";
            Website = "http://www.mongodb.org/";
            Color = Color.FromArgb(235, 215, 151);

            Requirements = new string[] 
            { 
                "MongoDB.Bson.dll", 
                "MongoDB.Driver.dll",
                "MongoDB.Driver.Core.dll",
                "MongoDB.Driver.Legacy.dll"
            };

            MongoUrlBuilder builder = new MongoUrlBuilder();
            builder.MaxConnectionIdleTime = TimeSpan.FromHours(1);
            builder.MaxConnectionLifeTime = TimeSpan.FromHours(1);
            builder.Server = new MongoServerAddress("localhost");
            builder.W = 1;

            ConnectionString = builder.ToString();
            InsertsPerQuery = 10000;
        }

        public override void Init(int flowCount, long flowRecordCount)
        {
            clients = new MongoClient[flowCount];
            collections = new MongoCollection<Row>[flowCount];

            for (int i = 0; i < flowCount; i++)
            {
                var client = new MongoClient(ConnectionString);
                server = client.GetServer();
                database = server.GetDatabase("test");

                if (database.CollectionExists(CollectionName))
                    database.DropCollection(CollectionName);

                var collection = database.GetCollection<Row>(CollectionName);
                collection.CreateIndex(new IndexKeysBuilder().Ascending("_id"));

                collections[i] = collection;
                clients[i] = client;
            }
        }

        public override void Write(int flowID, IEnumerable<KeyValuePair<long, Tick>> flow)
        {
            int idx = 0;
            Row[] buffer = new Row[InsertsPerQuery];

            BulkWriteOperation<Row> bulk = collections[flowID].InitializeUnorderedBulkOperation();
            IMongoQuery query = null;

            foreach (var kv in flow)
            {
                buffer[idx] = new Row(kv.Key, kv.Value);

                query = Query.EQ("_id", buffer[idx]._id);
                bulk.Find(query).Upsert().ReplaceOne(buffer[idx]);

                idx++;

                if (idx == buffer.Length)
                {
                    idx = 0;

                    bulk.Execute();
                    bulk = collections[flowID].InitializeUnorderedBulkOperation();
                }
            }

            if (idx > 0)
            {
                bulk = collections[flowID].InitializeUnorderedBulkOperation();
                foreach (var item in buffer.Left(idx))	
                {
                    query = Query.EQ("_id", item._id);
                    bulk.Find(query).Upsert().ReplaceOne(item);
	            }

                bulk.Execute();
            }
        }

        public override IEnumerable<KeyValuePair<long, Tick>> Read()
        {
            var cursor = collections.First().FindAll().SetSortOrder(SortBy.Ascending("_id"));

            foreach (var row in cursor)
            {
                long key = row.FromBytes(row._id.ToByteArray());

                yield return new KeyValuePair<long, Tick>(key, row.Record);
            }
        }

        public override void Finish()
        {
        }

        public override long Size
        {
            get
            {
                long sum = 0;
                var stat = collections.First().GetStats();
                sum += stat.DataSize + stat.TotalIndexSize;

                return sum;
            }
        }

        private class Row
        {
            public ObjectId _id { get; set; }
            public Tick Record { get; set; }

            public Row()
            {
            }

            public Row(long key, Tick record)
            {
                _id = new ObjectId(ToBytes(key));
                Record = record;
            }

            public byte[] ToBytes(long value)
            {
                ulong val = (ulong)(value + Int64.MaxValue + 1);

                byte[] valueBytes = BitConverter.GetBytes(val);
                byte[] keyBytes = new byte[12];

                keyBytes[11] = valueBytes[0];
                keyBytes[10] = valueBytes[1];
                keyBytes[9] = valueBytes[2];
                keyBytes[8] = valueBytes[3];
                keyBytes[7] = valueBytes[4];
                keyBytes[6] = valueBytes[5];
                keyBytes[5] = valueBytes[6];
                keyBytes[4] = valueBytes[7];

                return keyBytes;
            }

            public long FromBytes(byte[] valueBytes)
            {
                byte[] buffer = new byte[8];

                buffer[0] = valueBytes[11];
                buffer[1] = valueBytes[10];
                buffer[2] = valueBytes[9];
                buffer[3] = valueBytes[8];
                buffer[4] = valueBytes[7];
                buffer[5] = valueBytes[6];
                buffer[6] = valueBytes[5];
                buffer[7] = valueBytes[4];

                UInt64 value = BitConverter.ToUInt64(buffer, 0);

                return (Int64)(value - (UInt64)Int64.MaxValue - 1);
            }
        }
    }
}
