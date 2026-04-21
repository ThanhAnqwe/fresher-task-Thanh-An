using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Department entity
    /// </summary>
    /// Created By: dvhai (09/04/2026)
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IDbConnection dbConnection, IMemoryCache memoryCache) : base(dbConnection, memoryCache)
        {

        }

        public async Task<bool> CheckEmployeeInDepartment(Guid departmentId)
        {
            string query = SQLExtension.GetQuery("Department.CheckHasEmployee");
            var @param = new Dictionary<string, object>
            {
                {"@DepartmentId", departmentId }
            };

            var result = await _dbConnection.QueryFirstOrDefaultAsync<int?>(query, param, commandType: System.Data.CommandType.Text);

            return result.HasValue;
        }

        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <param name="code">Mã department</param>
        /// <returns>Department tìm thấy hoặc null</returns>
        /// CREATED BY: dvhai (09/04/2026)
        public async Task<Department> GetDepartmentByCode(string code)
        {
            string query = SQLExtension.GetQuery("Department.GetByCode");
            var @param = new Dictionary<string, object>
            {
                {"@DepartmentCode", code }
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<Department>(query, @param, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Đếm số employee theo department code
        /// </summary>
        public async Task<int> GetEmployeeCountByCodeAsync(string code)
        {

            string query = SQLExtension.GetQuery("Department.GetEmployeeCountByCode");

            var param = new Dictionary<string, object>
            {
                { "@DepartmentCode", code }
            };

            return await _dbConnection.QuerySingleAsync<int>(query, param, commandType: CommandType.Text);
        }

        /// <summary>
        /// Lấy danh sách employee theo department code
        /// </summary>
        public async Task<IEnumerable<Employee>> GetEmployeesByCodeAsync(string code)
        {
            string query = SQLExtension.GetQuery("Department.GetEmployeesByCode");

            var param = new Dictionary<string, object>
            {
                { "@DepartmentCode", code }
            };

            return await _dbConnection.QueryAsync<Employee>(query, param, commandType: CommandType.Text);
        }


    }
}
