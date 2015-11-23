       ------------------------------------
       # Database Benchmark changelog

       * ADDED: DBMS Linter SQL Server

       ------------------------------------
       ver. 3.0.0 (2015-06-22)
       
       ## Important improvements of Database Benchmark
       
       * ADDED: KeyTypes combo box replaced with Randomness slider in the GUI. 0% value means that all of the generated keys are sequential, and with the increasing of the percentage the generated flow begins to contain random keys. 100% means that all keys are random.
       
       This change allows fine control over the generated keys and more precise tests.
       
       ***
       
       * ADDED: Logging of exceptions in the application. log4Net is used as a log tool. The logs can be found in ../Logs/ApplicationLog.txt
       and ../Logs/BenchmarkTestLog.txt.
       
       The source files of log4net can be found at: http://logging.apache.org/log4net/.
       
       ***
       
       * ADDED: Online report of the test results - the test results from the benchmark can be sent to the dedicated servers of Database Benchmark.
       * 
       The computer configuration allong with the test results is converted to JSON and sent to the servers. No personal user data is sent!
       
       The Json for .NET library used for JSON serialization can be found at: https://sourceforge.net/projects/csjson/.
       
       ***
       
       * ADDED: Monitoring of the memory usage for every database (for a database working on a remote server, the memory usage will be only for the driver that connects the application to the server).
       
       It monitors the following parameters:
       
       * Page File Bytes Peak - Shows the maximum amount of virtual memory, in bytes, that a process has reserved for use in the paging file(s).
       
       * Working Set Peak - Shows the maximum size, in bytes, in the working set of this process. The working set is the set of memory pages that were touched recently by the threads in the process.
       
       * Virtual Bytes Peak - Shows the maximum size, in bytes, of virtual address space that the process has used at any one time.
       
       ***
       
       * ADDED: The GUI now features docking capabilities for the windows. The window layout is saved automatically when the application is closing. It can also be saved from the File menu.
       
       The DockingPanelSuite library used for docking can be found at: https://github.com/dockpanelsuite/dockpanelsuite.
       
       ***
       
       * ADDED: Project management - all of the settings made to the databases can now be saved as a *.dbproj project from the File menu. This allows the users to easily build benchmark suites and then just load them into the application.
       * Because of this feature, all of the public fields and properties of a database implementation are serialized. If you want to serialize a specific database option, just make it a public visible field or property.
       
       * CHANGED: IBenchmark interface is now renamed to IDatabase.
       * CHANGED: Benchmark class is now renamed to Database.
       * CHANGED: IDatabase interface. It now includes the following properties:
       ```C#
       public string Category { get; set; }
       public string Description { get; set; }
       public string Website { get; set; }
       public Color Color { get; set; }
       public string[] Requirements { get; set; }
       ```
       Also, parameter names of Init and the signature of the Read method are changed:
       
       New method signatures are:
       ```C#
       void Init(int flowCount, long flowRecordCount, KeysType keysType);
       IEnumerable<KeyValuePair<long, Tick>> Read();
       ```
       * CHANGED: BenchmarkTest.cs - in Write(), each data flow is inserted from a separate thread into ONE database collection (table and etc.).
       The old implementation was inserting every flow into separate database collection from a separate thread.
       
       * CHANGED: BenchmarkTest.cs - in Read(), only one thread is started for reading data from the database. The old implementation was starting multiple threads depending on the flow count.
       
       * CHANGED: the Finish() time is now added to the secondary read time. The old implementation was adding it
       to the Read() time.
       
       * IMPROVED: Major improvements of the internal architecture and implementations.
       * IMPROVED: Major improvements of code readability.
       * IMPROVED: Major optimizations of the GUI that will make it lighter and faster.
       ***
       
       ## Important improvement of MySQL
       
       * ADDED: InsertsPerQuery property in the implementation of MySQl.
       
       "Increase the batch size for MySQL inserts from 1000 to 10000 (the MongoDB client uses batches of 30000 documents). 
       This will dramatically reduce the number of fsync() operations for ACID engines like InnoDB and TokuDB, 
       allowing them to insert data more quickly, especially on low end hard drives."
       
       * IMPROVED: new size reporting query for TokuDB storage engine.
                   
       Special thanks to Tim Callaghan, Vice President of Engineering at Tokutek for proposing the changes.
       ***
       ## Important improvement of LevelDB
       
       LevelDB engine is now updated to version 1.16 downloaded from http://leveldb.angeloflogic.com/.
       
       The following properties are added in the implementation of LevelDB:
       
       ```C#
       BlockSize (default: 4 * 1024)
       Cache (default: 8 * 1024 * 1024)
       WriteBufferSize (default: 4 * 1024 * 1024)
       MaxOpenFiles (default: 1000)
       Compression (default: NoCompression)
       ```
       ***
       * ADDED: Redis 2.8.19 with .Net Client 3.9.71.
       * ADDED: Couchbase 3.0.2 with Couchbase-Net-Client-2.0.2.
       * ADDED: Aerospike 3.4.1 with .NET Client 3.0.13.
       * ADDED: OrientDB 2.0 with OrientDB-NET.binary.
       * ADDED: Separate implementation for TokuMX.
       * ADDED: Export of the test results into a JSON file.
       * ADDED
       * ADDED: License header in MainForm.cs.
       
       * CHANGED: Removed SetDropDups option from TokuMX and MongoDB implementation (thanks to Tim Callaghan of Tokutek).
       * CHANGED: MongoDB implementation and TokuMX implementations:
       	* ObjectId _id is used as an only index. 
       	* Instead of InsertBatch, the inserts are now performed by the Bulk().Find().Upsert().ReplaceOne(), which 
             queries the database and if no record exists with the specified _id, an insert operation is performed, otherwise it replaces the whole document.
       
       * IMPROVED: TokuMX size query (thanks to Tim Callaghan of Tokutek).
       * IMPROVED: SQLite benchmark.
       
       * REMOVED: SiaqoDB database. 
       * REMOVED: RaptorDB database.
       * REMOVED: H2 database. 
       * REMOVED: DerbyDb database. 
       * REMOVED: NSimpleDB.
       * REMOVED: all IKVM references.
       
       * UPDATED: HamsterDB native library to version 2.1.9 and .NET wrapper recompiled for .NET 4.5.
       * UPDATED: MongoDB C# drivers to 2.0.0beta2.
       * UPDATED: CassandraDB to version 2.1.2.
       * UPDATED: SQLite to 3.8.4.1, ADO.NET Provider for SQLite to 1.0.92.0
       ***
       ver. 2.0.3 (2014-04-23)
       
       * ADDED: STS.General and STS.General.GUI sources have been added to the DatabaseBenchmark solution (thanks to Artemkas).
       * UPDATED: STSdb 4.0 library to STSdb 4.0.3  
       
       ------------------------------------
       ver. 2.0.2 (2014-03-19)
       
       * FIXED: BarChart sometimes draws two identical bars for speed and misses the one for size.
       
       * UPDATED: STSdb 4.0 library to STSdb 4.0.2
       
       ------------------------------------
       ver. 2.0.1 (2014-01-30)
       
       * BUG FIXED: MongoDB: generated rows are always stored in one collection
       * BUG FIXED: MongoDB: key index is not created when SetDropDups option is not supported (in TokuMX distribution)
       * UPDATED: STSdb 4.0 to 4.0.1
       
       ------------------------------------
       ver. 2.0 (2013-12-20)
       
       * CHANGED: the benchmark now uses type long for the keys and type Tick for the records. The records are generated using random-walk algorithm which produces close to real-life ticks. 
       
       The type Tick has the following structure:
       
       public class Tick
       {
          public string Symbol { get; set; }
          public DateTime Timestamp { get; set; }
          public double Bid { get; set; }
          public double Ask { get; set; }
          public int BidSize { get; set; }
          public int AskSize { get; set; }
          public string Provider { get; set; }
       
          public Tick()
          {
          }
       }
       
       * CHANGED: the old chart engine has been replaced with MS Charts control - removed ~1700 lines of code.
       
       * FIXED: the application process remained open after clossing the application.
       
       * UPDATED: STSdb4 library to STSdb 4.0
       * UPDATED: STSdb 3.5 library to 3.5.13
       * UPDATED: VelocityDB Standalone Client library to 3.2.1 released Dec 8, 2013
       * UPDATED: new implementation of VelocityDB (thanks to Mats Persson - founder of VelocityDB);
       * UPDATED: Perst library to 4.42
       * UPDATED: Oracle BerkeleyDB library to 12c Release 1 Library Version 12.1.6.0
       * UPDATED: MongoDB drivers to 1.8.3
       * UPDATED: MySQL connector to 6.7.4
       * UPDATED: MSSQL Server Compact library to 4.0 x64
       * UPDATED: H2 library to H2 1.3.174 and IKVM to 7.2.4630.5
       * UPDATED: HamsterDb library to HamsterDB 2.1.3
       * UPDATED: SQLite library to SQLite 3.8.1 + ADO.NET Provider for SQLite 1.0.89.0
       * UPDATED: Access library to Access 2013
       
       * ADDED: PostgreSQL (with Npgsql 2.0.13.91 .net connector)
       * ADDED: CassandraDB 2.0.2 + CassandraToLinq
       * ADDED: RaptorDB v3.0.6
       * ADDED: Microsoft SQL Server 2012 (SP1) - 11.0.3128.0 (X64) Dec 28 2012
       * ADDED: Volante 0.9

       * ADDED: Derby 10.10.1.1 (via IKVM.NET)
       * ADDED: Siaqodb 3.7
       * ADDED: BrightstarDB 1.5.0.0 Stable - Fri Nov 22, 2013 at 10:00 AM
       * ADDED: RavenDB 2.5.2750 - 2013-10-30 Embedded .NET Client
       
       * STARTED: NSimpleDB implementation
       
       NOTES:
        - MongoDB: somethimes crashes with more than 100 000 000 records in random mode (possible MongoDB bug)
        - RaptorDB: can't store more than 2,000,000 records and after shutdown won't release data file
        - Db4objects: can't store more than 5,000,000 records (on 8 GB RAM)
        - Cassandra: page manualy records on read and can't take database size from client
        - Access: cant handle file bigger than 2 GB
        - SiacoDB: on read the database loads all records into memory
       
       ------------------------------------
       ver. 1.4 (2013-07-12)
       
       Included databases:
       
       -STSdb W4.0 RC3 build 2
       -Oracle Berkeley DB 11g Release 2, library version 11.2.5.3.21: (May 11, 2012)
       -Perst 4.38
       -HamsterDB 2.0.5
       -Db4objects 8.0
       -MySQL 5.1.24-rc-community + .NET Connector 6.2.3
       -MS SQL Server Compact 3.5
       -SQLite 3.6.23.1 + ADO.NET Provider for SQLite 1.0.84.0
       -MS Access 2010
       -Firebird 2.5.2 + ADO.NET Data Provider 3.0.2.0
       -H2 1.3.170
       -ScimoreDB 4.0 + ADO.NET Data Provider
       -MongoDB 2.4.1
       -levelDB 1.2
       -VelocityDB Standalone Client 2.7.2.0
       
       - updated .dll of STSdb4 to version STSdb W4.0 RC3 build 2.
       - added new implementation of VelocityDB (thanks to Mats Persson - founder of VelocityDB).
