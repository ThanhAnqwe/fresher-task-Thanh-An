using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IDbConnection dbConnection, IMemoryCache memoryCache)
            : base(dbConnection, memoryCache)
        {
        }

        public async Task<Employee> GetEmployeeByCode(string code)
        {
            string query = SQLExtension.GetQuery("Employee.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@EmployeeCode", code }
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByDepartmentId");
            var param = new Dictionary<string, object>
            {
                {"@DepartmentID", departmentId }
            };
            return await _dbConnection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByPositionId");
            var param = new Dictionary<string, object>
            {
                {"@PositionID", positionId }
            };
            return await _dbConnection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<PagingResponse<Employee>> GetEmployeesByFilterAsync(Guid? departmentId, Guid? positionId, decimal? salaryFrom, decimal? salaryTo, int? gender, DateTime? hireDateFrom, DateTime? hireDateTo, int pageSize, int pageIndex)
        {
            
            var countSql = new System.Text.StringBuilder($"SELECT COUNT(*) FROM {_tableName} WHERE IsDeleted = FALSE");

            
            var sql = new System.Text.StringBuilder($"SELECT * FROM {_tableName} WHERE IsDeleted = FALSE");
            var parameters = new DynamicParameters();


            if (departmentId.HasValue)
            {
                countSql.Append(" AND DepartmentID = @DepartmentID");
                sql.Append(" AND DepartmentID = @DepartmentID");
                parameters.Add("@DepartmentID", departmentId.Value.ToString());
            }

            if (positionId.HasValue)
            {
                countSql.Append(" AND PositionID = @PositionID");
                sql.Append(" AND PositionID = @PositionID");
                parameters.Add("@PositionID", positionId.Value.ToString());
            }

            if (salaryFrom.HasValue)
            {
                countSql.Append(" AND Salary >= @SalaryFrom");
                sql.Append(" AND Salary >= @SalaryFrom");
                parameters.Add("@SalaryFrom", salaryFrom.Value);
            }

            if (salaryTo.HasValue)
            {
                countSql.Append(" AND Salary <= @SalaryTo");
                sql.Append(" AND Salary <= @SalaryTo");
                parameters.Add("@SalaryTo", salaryTo.Value);
            }

            if (gender.HasValue)
            {
                countSql.Append(" AND Gender = @Gender");
                sql.Append(" AND Gender = @Gender");
                parameters.Add("@Gender", gender.Value);
            }

            if (hireDateFrom.HasValue)
            {
                countSql.Append(" AND HireDate >= @HireDateFrom");
                sql.Append(" AND HireDate >= @HireDateFrom");
                parameters.Add("@HireDateFrom", hireDateFrom.Value);
            }

            if (hireDateTo.HasValue)
            {
                countSql.Append(" AND HireDate <= @HireDateTo");
                sql.Append(" AND HireDate <= @HireDateTo");
                parameters.Add("@HireDateTo", hireDateTo.Value);
            }
            
            var total = await _dbConnection.QueryFirstOrDefaultAsync<long>(countSql.ToString(), parameters, commandType: CommandType.Text);


            //Default
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            // Thêm paging vào query
            int offset = (pageIndex - 1) * pageSize;
            sql.Append($" LIMIT {pageSize} OFFSET {offset}");


            var employees = await _dbConnection.QueryAsync<Employee>(sql.ToString(), parameters, commandType: CommandType.Text);

            return new PagingResponse<Employee>
            {
                Total = total,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Data = employees.ToList()
            };
        }

        public async Task<bool> CheckCodeExistsAsync(string employeeCode, Guid? employeeId = null)
        {
            
            var parameters = new DynamicParameters();
            parameters.Add("e_Code", employeeCode);
            parameters.Add("e_Id", employeeId);

            var result = await _dbConnection.QueryFirstOrDefaultAsync<int?>(
                "Proc_Employee_CheckCode",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.HasValue;
        }
    }
}