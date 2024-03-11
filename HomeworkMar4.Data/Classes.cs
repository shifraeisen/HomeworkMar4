using System.Data.SqlClient;
using System.Security.Cryptography;

namespace HomeworkMar4.Data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellNum { get; set; }
        public DateTime DateCreated { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal Balance { get; set; }
    }
    public class Actn
    {
        public string ActionName { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
    public class Contribution
    {
        public int SimchaID { get; set; }
        public int ContributorID { get; set; }
        public decimal Amount { get; set; }
    }
    public class SimchaFundDbMngr
    {
        private readonly string _connectionString;

        public SimchaFundDbMngr(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Simcha> GetAllSimchos()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "select * from Simchos";

            var simchos = new List<Simcha>();

            connection.Open();

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                simchos.Add(new Simcha
                {
                    Id = (int)reader["ID"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"]
                });
            }
            return simchos;
        }
        public int GetTotalContributorCount()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "select COUNT(*) from Contributors";

            connection.Open();

            return (int)cmd.ExecuteScalar();
        }
        public int GetContributorCountForSimcha(int simchaID)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"select COUNT(*) from Contributions
                                where SimchaID = @simchaID";
            cmd.Parameters.AddWithValue("@simchaID", simchaID);

            connection.Open();

            return (int)cmd.ExecuteScalar();
        }
        public decimal GetTotalforSimcha(int simchaID)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"select SUM(Amount) from Contributions
                                where SimchaID = @simchaID";
            cmd.Parameters.AddWithValue("@simchaID", simchaID);

            connection.Open();

            object total = cmd.ExecuteScalar();

            return total == DBNull.Value ? 0 : (decimal)total;
        }
        public void AddSimcha(Simcha s)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"insert into Simchos
                                values (@name, @date)";
            cmd.Parameters.AddWithValue("@name", s.Name);
            cmd.Parameters.AddWithValue("@date", s.Date);

            connection.Open();

            cmd.ExecuteNonQuery();
        }
        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "select * from Contributors";

            List<Contributor> contributors = new();

            connection.Open();

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                contributors.Add(new Contributor
                {
                    Id = (int)reader["ID"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNum = (string)reader["CellNum"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Balance = GetBalance((int)reader["ID"]),
                    DateCreated = (DateTime)reader["DateCreated"]
                });
            }
            return contributors;
        }
        public decimal GetTotal()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"create table #temp(amt money)

                                insert into #temp
                                select SUM(DepositAmount) from Deposits
                                insert into #temp
                                select -(isnull(SUM(Amount), 0)) from Contributions

                                select SUM(amt) from #temp

                                drop table #temp";

            connection.Open();

            return (decimal)cmd.ExecuteScalar();
        }
        public decimal GetBalance(int cID)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"create table #temp(amt money)

                                insert into #temp
                                select SUM(DepositAmount) from Deposits where ContributorID = @cID

                                insert into #temp
                                select -(isnull(SUM(Amount), 0)) from Contributions where ContributorID = @cID

                                select SUM(amt) from #temp

                                drop table #temp";
            cmd.Parameters.AddWithValue("@cID", cID);

            connection.Open();

            //object total = cmd.ExecuteScalar();

            return (decimal)cmd.ExecuteScalar();
        }
        public void AddContributor(Contributor c, decimal depositAmt)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"insert into Contributors 
                                values (@firstName, @lastName, @cellNum, @dateCreated, @alwaysInclude) 
                                select SCOPE_IDENTITY() 
                                insert into Deposits 
                                values (SCOPE_IDENTITY(), @depositAmt, @date)";
            cmd.Parameters.AddWithValue("@firstName", c.FirstName);
            cmd.Parameters.AddWithValue("@lastName", c.LastName);
            cmd.Parameters.AddWithValue("@cellNum", c.CellNum);
            cmd.Parameters.AddWithValue("@dateCreated", c.DateCreated);
            cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            cmd.Parameters.AddWithValue("@depositAmt", depositAmt);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);

            connection.Open();

            c.Id = (int)(decimal)cmd.ExecuteScalar();
        }
        public void AddDeposit(int contributorID, decimal depositAmt, DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"insert into Deposits 
                                values (@contID, @amt, @date)";
            cmd.Parameters.AddWithValue("@contID", contributorID);
            cmd.Parameters.AddWithValue("@amt", depositAmt);
            cmd.Parameters.AddWithValue("@date", date);

            connection.Open();

            cmd.ExecuteNonQuery();
        }
        public string GetContributorNameByID(int contributorID)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"select FirstName, LastName from Contributors
                                where ID = @id";
            cmd.Parameters.AddWithValue("@id", contributorID);

            connection.Open();

            var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }
            return (string)reader["FirstName"] + ' ' + (string)reader["LastName"];
        }
        public List<Actn> GetActions(int contributorID)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"select 'Deposit' as Name, DepositAmount as Amount, Date from Deposits where ContributorID = @contID
                                union
                                select s.Name, c.Amount, s.Date from Contributions c join Simchos s on s.ID = c.SimchaID where ContributorID = @contID";
            cmd.Parameters.AddWithValue("@contID", contributorID);

            List<Actn> actions = new();

            connection.Open();

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var action = new Actn();
                if ((string)reader["Name"] == "Deposit")
                {
                    action.ActionName = (string)reader["Name"];
                }
                else
                {
                    action.ActionName = $"Contribution for the {(string)reader["Name"]}";
                }
                action.Amount = (decimal)reader["Amount"];
                action.Date = (DateTime)reader["Date"];
                actions.Add(action);
            }
            return actions;
        }
        public void EditContributor(Contributor c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"update Contributors
                                set FirstName = @firstName, LastName = @lastName, CellNum = @cell, AlwaysInclude = @alwaysInclude, DateCreated = @date
                                where ID = @contID";
            cmd.Parameters.AddWithValue("@firstName", c.FirstName);
            cmd.Parameters.AddWithValue("@lastName", c.LastName);
            cmd.Parameters.AddWithValue("@cell", c.CellNum);
            cmd.Parameters.AddWithValue("@date", c.DateCreated);
            cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            cmd.Parameters.AddWithValue("@contID", c.Id);

            connection.Open();

            cmd.ExecuteNonQuery();
        }
        public string GetSimchaNameByID(int simchaID)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"select Name from Simchos
                                where ID = @id";
            cmd.Parameters.AddWithValue("@id", simchaID);

            connection.Open();

            return (string)cmd.ExecuteScalar();
        }
        public List<Contribution> GetContributionsForSimcha(int simchaID)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"select c.ContributorID, c.Amount 
                                from Contributions c join Simchos s on s.ID = c.SimchaID
                                where s.ID = @sID";
            cmd.Parameters.AddWithValue("@sID", simchaID);

            List<Contribution> contributions = new();

            connection.Open();

            var reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                contributions.Add(new Contribution
                {
                    SimchaID = simchaID,
                    ContributorID = (int)reader["ContributorID"],
                    Amount = (decimal)reader["Amount"]                    
                });
            }
            return contributions;
        }
        public void AddContributions(List<Contribution> contributions)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();

            cmd.CommandText = @"delete from Contributions
                                where SimchaID = @simchaID";
            cmd.Parameters.AddWithValue("@simchaID", contributions[0].SimchaID);
            connection.Open() ;
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            cmd.CommandText = @"insert into Contributions
                                values (@simchaID, @contributorID, @amt)";

            foreach(Contribution c in contributions)
            {
                cmd.Parameters.AddWithValue("@simchaID", c.SimchaID);
                cmd.Parameters.AddWithValue("@contributorID", c.ContributorID);
                cmd.Parameters.AddWithValue("@amt", c.Amount);

                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
            }
            
        }
    }
    public static class Extension
    {
        public static T GetValOrNull<T>(this SqlDataReader reader, string columnName)
        {
            var val = reader[columnName];
            if(val == DBNull.Value)
            {
                return default(T);
            }
            return (T)val;
        }
    }
}