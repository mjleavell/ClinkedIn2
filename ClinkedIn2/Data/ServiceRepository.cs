using System;
using ClinkedIn2.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Linq;

namespace ClinkedIn2.Data
{
    public class ServiceRepository
    {
        const string ConnectionString = "Server = localhost; Database = ClinkedIn; Trusted_Connection = True;";

        public Service AddService(string name, string description, decimal price)
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                var repository = new ServiceRepository();

                var insertQuery = @"
                        INSERT INTO [dbo].[Services]
                            ([Name]
                            ,[Description]
                            ,[Price])
                        OUTPUT inserted.*
                        VALUES
                            (@name
		                    ,@description
		                    ,@price)";

                var parameters = new
                {
                    Name = name,
                    Description = description,
                    Price = price
                };

                var newService = db.QueryFirstOrDefault<Service>(insertQuery, parameters);

                if (newService != null)
                {
                    return newService;
                }
            }
            throw new Exception("No Service found");
        }

        public IEnumerable<Service> GetAllServices()
        {
            using (var db = new SqlConnection(ConnectionString))
            {
                var getQuery = "SELECT * FROM Services";

                var services = db.Query<Service>(getQuery).ToList();

                return services;
            }          
        }
    }
}
