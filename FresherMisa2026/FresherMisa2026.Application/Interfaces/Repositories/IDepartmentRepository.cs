using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<Department> GetDepartmentByCode(string code);

        /// <summary>
        /// Kiểm tra xem department có employee nào không
        /// </summary>
        Task<bool> CheckEmployeeInDepartment(Guid departmentId);

        /// <summary>
        /// Lấy danh sách employee theo department code
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByCodeAsync(string code);

        // count số employee theo department code
        Task<int> GetEmployeeCountByCodeAsync(string code);
    }
}
