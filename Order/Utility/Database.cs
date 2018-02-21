using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Order.Utility
{
    public class Entry
    {
        public long Id { set; get; }
        public string Name { set; get; }
        public string Comment { set; get; }
    }

    public class Client
    {
        public long Id { set; get; }
        public string Name { set; get; }
        public string Phone { set; get; }
        public string Comment { set; get; }
    }

    public class Order
    {
        public long Id { set; get; }
        public DateTime Date { set; get; }
        public long ClientId { set; get; }
        public string ClientName { set; get; }
        public long ProductId { set; get; }
        public string ProductName { set; get; }
        public long UnitId { set; get; }
        public string UnitName { set; get; }
        public long CashId { set; get; }
        public string CashName { set; get; }
        public double Cost { set; get; }
        public double UnitPrice { set; get; }
        public long Number { set; get; }
        public string Comment { set; get; }
        public double TotalPrice => UnitPrice * Number;
        public double Profit => TotalPrice - Cost;
    }

    class Database : IDisposable
    {
        private readonly SQLiteConnection _conn;

        internal Database()
        {
            _conn = new SQLiteConnection($"data source={Environment.DatabasePath}");
            _conn.Open();
        }

        public void Dispose()
        {
            _conn.Close();
        }

        internal void TryInitialize()
        {
            TryExecute("CREATE TABLE UNIT(ID INTEGER PRIMARY KEY AUTOINCREMENT, NAME TEXT NOT NULL, COMMENT TEXT DEFAULT '')");
            TryExecute("CREATE TABLE CLIENT(ID INTEGER PRIMARY KEY AUTOINCREMENT, NAME TEXT NOT NULL, PHONE TEXT NULL, COMMENT TEXT DEFAULT'')");
            TryExecute("CREATE TABLE PRODUCT(ID INTEGER PRIMARY KEY AUTOINCREMENT, NAME TEXT NOT NULL, COMMENT TEXT DEFAULT '')");
            if(TryExecute("CREATE TABLE CASHTYPE(ID INTEGER PRIMARY KEY AUTOINCREMENT, NAME TEXT NOT NULL, COMMENT TEXT DEFAULT '')"))
            {
                TryExecute("INSERT INTO CASHTYPE(NAME) VALUES('现金'),('挂帐')");
            }
            TryExecute("CREATE TABLE DEAL(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                "DATETIME TEXT NOT NULL," +
                "CLIENT INTEGER NOT NULL," +
                "PRODUCT INTEGER NOT NULL," +
                "NUMBER INTEGER NOT NULL," +
                "UNIT INTEGER NOT NULL," +
                "COST REAL DEFAULT 0," +
                "UNITPRICE REAL DEFAULT 0," +
                "CASHTYPE INTEGER NOT NULL," +
                "COMMENT TEXT DEFAULT ''," +
                "FOREIGN KEY(CLIENT) REFERENCES CLIENT(ID)," +
                "FOREIGN KEY(PRODUCT) REFERENCES PRODUCT(ID)," +
                "FOREIGN KEY(UNIT) REFERENCES UNIT(ID)," +
                "FOREIGN KEY(CASHTYPE) REFERENCES CASHTYPE(ID))");
        }

        internal void UpdateDeal(long id, DateTime date, long clientId, long productId, long unitId,
            long number, double cost, double unitPrice, long cashId, string comment)
        {
            var cmd = new SQLiteCommand(
                $"UPDATE DEAL SET DATETIME=@DATETIME, CLIENT=@CLIENT, PRODUCT=@PRODUCT," +
                $"NUMBER=@NUMBER, UNIT=@UNIT, COST=@COST, UNITPRICE=@UNITPRICE," +
                $"CASHTYPE=@CASHTYPE, COMMENT=@COMMENT WHERE ID=@ID", _conn);
            cmd.Parameters.Add(new SQLiteParameter("@ID", id));
            cmd.Parameters.Add(new SQLiteParameter("@DATETIME", date));
            cmd.Parameters.Add(new SQLiteParameter("@CLIENT", clientId));
            cmd.Parameters.Add(new SQLiteParameter("@PRODUCT", productId));
            cmd.Parameters.Add(new SQLiteParameter("@UNIT", unitId));
            cmd.Parameters.Add(new SQLiteParameter("@NUMBER", number));
            cmd.Parameters.Add(new SQLiteParameter("@COST", cost));
            cmd.Parameters.Add(new SQLiteParameter("@UNITPRICE", unitPrice));
            cmd.Parameters.Add(new SQLiteParameter("@CASHTYPE", cashId));
            cmd.Parameters.Add(new SQLiteParameter("@COMMENT", comment));
            cmd.ExecuteNonQuery();
        }

        internal long AddDeal(DateTime date, long clientId, long productId, long unitId,
            long number, double cost, double unitPrice, long cashId, string comment)
        {
            var cmd = new SQLiteCommand(
                $"INSERT INTO DEAL(DATETIME, CLIENT, PRODUCT, NUMBER, UNIT, COST, UNITPRICE, CASHTYPE, COMMENT)" +
                $"VALUES(@DATETIME, @CLIENT, @PRODUCT, @NUMBER, @UNIT, @COST, @UNITPRICE, @CASHTYPE, @COMMENT)",
                _conn);
            cmd.Parameters.Add(new SQLiteParameter("@DATETIME", date));
            cmd.Parameters.Add(new SQLiteParameter("@CLIENT", clientId));
            cmd.Parameters.Add(new SQLiteParameter("@PRODUCT", productId));
            cmd.Parameters.Add(new SQLiteParameter("@UNIT", unitId));
            cmd.Parameters.Add(new SQLiteParameter("@NUMBER", number));
            cmd.Parameters.Add(new SQLiteParameter("@COST", cost));
            cmd.Parameters.Add(new SQLiteParameter("@UNITPRICE", unitPrice));
            cmd.Parameters.Add(new SQLiteParameter("@CASHTYPE", cashId));
            cmd.Parameters.Add(new SQLiteParameter("@COMMENT", comment));
            cmd.ExecuteNonQuery();
            return _conn.LastInsertRowId;
        }

        internal IEnumerable<Order> QueryDeals(string client, DateTime? start, DateTime? end)
        {
            var startSnippet = start.HasValue ? $"AND DEAL.DATETIME >= '{start.Value.ToString("yyyy-MM-dd HH:mm:ss")}'" : string.Empty;
            var endSnippet = end.HasValue ? $"AND DEAL.DATETIME <= '{end.Value.ToString("yyyy-MM-dd HH:mm:ss")}'" : string.Empty;
            var cmd = new SQLiteCommand(
                $"SELECT " +
                $"DEAL.ID AS ID," +
                $"DEAL.DATETIME AS DATETIME," +
                $"DEAL.CLIENT AS CLIENTID," +
                $"DEAL.PRODUCT AS PRODUCTID," +
                $"DEAL.UNIT AS UNITID," +
                $"DEAL.NUMBER AS NUMBER," +
                $"DEAL.COST AS COST," +
                $"DEAL.UNITPRICE AS UNITPRICE," +
                $"DEAL.CASHTYPE AS CASHID," +
                $"DEAL.COMMENT AS COMMENT," +
                $"CLIENT.NAME AS CLIENTNAME," +
                $"UNIT.NAME AS UNITNAME," +
                $"PRODUCT.NAME AS PRODUCTNAME," +
                $"CASHTYPE.NAME AS CASHNAME " +
                $"FROM DEAL " +
                $"JOIN CLIENT ON DEAL.CLIENT = CLIENT.ID " +
                $"JOIN UNIT ON DEAL.UNIT = UNIT.ID " +
                $"JOIN PRODUCT ON DEAL.PRODUCT = PRODUCT.ID " +
                $"JOIN CASHTYPE ON DEAL.CASHTYPE = CASHTYPE.ID " +
                $"WHERE CLIENT.NAME LIKE '%{client}%' {startSnippet} {endSnippet}", _conn);
            var reader = cmd.ExecuteReader();
            var r = new List<Order>();
            while (reader.Read())
            {
                var entry = new Order
                {
                    Id = (long)reader["ID"],
                    ClientId = (long)reader["CLIENTID"],
                    ClientName = (string)reader["CLIENTNAME"],
                    ProductId = (long)reader["PRODUCTID"],
                    ProductName = (string)reader["PRODUCTNAME"],
                    UnitId = (long)reader["UNITID"],
                    UnitName = (string)reader["UNITNAME"],
                    UnitPrice = (double)reader["UNITPRICE"],
                    Cost = (double)reader["COST"],
                    Number = (long)reader["NUMBER"],
                    Date = DateTime.Parse((string)reader["DATETIME"]),
                    CashId = (long)reader["CASHID"],
                    CashName = (string)reader["CASHNAME"],
                    Comment = (string)reader["COMMENT"]
                };
                r.Add(entry);
            }
            return r;
        }

        internal void DeleteDeal(long id)
        {
            DeleteDependentEntry("DEAL", id);
        }

        internal void UpdateClient(long id, string name, string phone, string comment)
        {
            var commentSnippet = comment != null ? ",COMMENT=@COMMENT" : string.Empty;
            var cmd = new SQLiteCommand(
                $"UPDATE CLIENT SET NAME=@NAME, PHONE=@PHONE {commentSnippet} WHERE ID=@ID",
                _conn);
            cmd.Parameters.Add(new SQLiteParameter("@NAME", name));
            cmd.Parameters.Add(new SQLiteParameter("@PHONE", phone));
            if (comment != null)
                cmd.Parameters.Add(new SQLiteParameter("@COMMENT", comment));
            cmd.Parameters.Add(new SQLiteParameter("@ID", id));
            cmd.ExecuteNonQuery();
        }

        internal void AddClient(string name, string phone, string comment)
        {
            var cmd = new SQLiteCommand($"INSERT INTO CLIENT(NAME,PHONE,COMMENT) VALUES(@NAME,@PHONE,@COMMENT)", _conn);
            cmd.Parameters.Add(new SQLiteParameter("@NAME", name));
            cmd.Parameters.Add(new SQLiteParameter("@PHONE", phone));
            cmd.Parameters.Add(new SQLiteParameter("@COMMENT", comment ?? string.Empty));
            cmd.ExecuteNonQuery();
        }

        internal void DeleteClient(long id)
        {
            DeleteDependentEntry("CLIENT", id);
        }

        internal IEnumerable<Client> QueryClients()
        {
            var cmd = new SQLiteCommand($"SELECT * FROM CLIENT", _conn);
            var reader = cmd.ExecuteReader();
            var r = new List<Client>();
            while (reader.Read())
            {
                var entry = new Client
                {
                    Id = (long)reader["ID"],
                    Name = (string)reader["NAME"],
                    Phone = (string)reader["PHONE"],
                    Comment = (string)reader["COMMENT"]
                };
                r.Add(entry);
            }
            return r;
        }

        internal void UpdateDependentEntry(string table, long id, string name, string comment)
        {
            var commentSnippet = comment != null ? ",COMMENT=@COMMENT" : string.Empty;
            var cmd = new SQLiteCommand(
                $"UPDATE {table} SET NAME=@NAME {commentSnippet} WHERE ID=@ID",
                _conn);
            cmd.Parameters.Add(new SQLiteParameter("@NAME", name));
            if (comment != null)
                cmd.Parameters.Add(new SQLiteParameter("@COMMENT", comment));
            cmd.Parameters.Add(new SQLiteParameter("@ID", id));
            cmd.ExecuteNonQuery();
        }

        internal void AddDependentEntry(string table, string name, string comment)
        {
            var cmd = new SQLiteCommand($"INSERT INTO {table}(NAME,COMMENT) VALUES(@NAME,@COMMENT)", _conn);
            cmd.Parameters.Add(new SQLiteParameter("@NAME", name));
            cmd.Parameters.Add(new SQLiteParameter("@COMMENT", comment ?? string.Empty));
            cmd.ExecuteNonQuery();
        }

        internal void DeleteDependentEntry(string table, long id)
        {
            Execute($"DELETE FROM {table} WHERE ID={id}");
        }

        internal IEnumerable<Entry> QueryDependentEntries(string table)
        {
            var cmd = new SQLiteCommand($"SELECT * FROM {table}", _conn);
            var reader = cmd.ExecuteReader();
            var r = new List<Entry>();
            while(reader.Read())
            {
                var entry = new Entry
                {
                    Id = (long)reader["ID"],
                    Name = (string)reader["NAME"],
                    Comment = (string)reader["COMMENT"]
                };
                r.Add(entry);
            }
            return r;
        }

        internal void Execute(string cmdText)
        {
            var cmd = new SQLiteCommand(cmdText, _conn);
            cmd.ExecuteNonQuery();
        }

        internal bool TryExecute(string cmdText)
        {
            try
            {
                Execute(cmdText);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
