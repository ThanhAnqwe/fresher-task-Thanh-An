using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Entities.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration configuration) : base(configuration)
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

        public async Task<IEnumerable<Employee>> GetEmployeesByFilterAsync(Guid? departmentId, Guid? positionId, decimal? salaryFrom, decimal? salaryTo, int? gender, DateTime? hireDateFrom, DateTime? hireDateTo)
        {
            var sql = new System.Text.StringBuilder($"SELECT * FROM {_tableName} WHERE 1=1");
            var parameters = new DynamicParameters();

            
            if (_modelType.GetHasDeletedColumn())
            {
                sql.Append(" AND IsDeleted = FALSE");
            }

            if (departmentId.HasValue)
            {
                sql.Append(" AND DepartmentID = @DepartmentID");
                parameters.Add("@DepartmentID", departmentId.Value.ToString());
            }

            if (positionId.HasValue)
            {
                sql.Append(" AND PositionID = @PositionID");
                parameters.Add("@PositionID", positionId.Value.ToString());
            }

            if (salaryFrom.HasValue)
            {
                sql.Append(" AND Salary >= @SalaryFrom");
                parameters.Add("@SalaryFrom", salaryFrom.Value);
            }

            if (salaryTo.HasValue)
            {
                sql.Append(" AND Salary <= @SalaryTo");
                parameters.Add("@SalaryTo", salaryTo.Value);
            }

            if (gender.HasValue)
            {
                sql.Append(" AND Gender = @Gender");
                parameters.Add("@Gender", gender.Value);
            }

            if (hireDateFrom.HasValue)
            {
                sql.Append(" AND HireDate >= @HireDateFrom");
                parameters.Add("@HireDateFrom", hireDateFrom.Value);
            }

            if (hireDateTo.HasValue)
            {
                sql.Append(" AND HireDate <= @HireDateTo");
                parameters.Add("@HireDateTo", hireDateTo.Value);
            }

            var employees = await _dbConnection.QueryAsync<Employee>(sql.ToString(), parameters, commandType: CommandType.Text);
            return employees.ToList();
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