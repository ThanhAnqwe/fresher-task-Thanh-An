using FresherMisa2026.Entities.Department;
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
        /// <param name="departmentId"></param>
        /// <returns>
        /// True - có nhân viên
        /// False - không có nhân viên n
        /// </returns>
        Task<bool> CheckEmployeeInDepartment(Guid departmentId);
    }
}
