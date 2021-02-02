﻿using System;
using System.Collections.Generic;
using System.Linq;
using GHPRS.Core.Entities;
using GHPRS.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using static GHPRS.Core.Entities.Template;

namespace GHPRS.Persistence.Repositories
{
    public class TemplateRepository : Repository<Template>, ITemplateRepository
    {
        private readonly GhprsContext _context;
        private readonly DbSet<Template> _entities;
        private readonly ILogger<TemplateRepository> _logger;

        public TemplateRepository(GhprsContext context, ILogger<TemplateRepository> logger) : base(context)
        {
            _entities = context.Set<Template>();
            _context = context;
            _logger = logger;
        }

        public void CreateTemplateTable(WorkSheet workSheet)
        {
            var columnsValues = string.Empty;
            foreach (var column in workSheet.Columns) columnsValues += $"\"{column.Name}\" {column.Type}, ";
            var createScript =
                $"CREATE TABLE If Not Exists uploads.\"{workSheet.TableName}\" ( \"UserId\" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY (INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1), \"Id\" integer, {columnsValues} CONSTRAINT \"PK_{workSheet.TableName}\" PRIMARY KEY (\"Id\"));";

            try
            {
                var connectionString = _context.Database.GetConnectionString();
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "CREATE SCHEMA if not exists uploads;";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = createScript;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }

        public IEnumerable<Template> GetAllFull()
        {
            return _entities;
        }

        public Template GetAllFullById(int id)
        {
            return _entities.SingleOrDefault(s => s.Id == id);
        }

        public object GetDetailsById(int id)
        {
            var result = _entities.Select(s => new
                    {s.Id, s.Name, s.Description, s.ContentType, s.Version, s.Frequency, s.Status, s.CreatedAt})
                .FirstOrDefault(x => x.Id == id);
            return result;
        }

        public IEnumerable<object> GetList(string role)
        {
            if (role == "Administrator")
                return _entities.Select(s => new
                        {s.Id, s.Name, s.Description, s.ContentType, s.Version, s.Frequency, s.Status, s.CreatedAt})
                    .ToList();
            return _entities
                .Select(s => new
                    {s.Id, s.Name, s.Description, s.ContentType, s.Version, s.Frequency, s.Status, s.CreatedAt})
                .Where(x => x.Status == TemplateStatus.Active).ToList();
        }
    }
}