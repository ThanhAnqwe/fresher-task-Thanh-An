using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FresherMisa2026.Application.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(
            IBaseRepository<Employee> baseRepository,
            IEmployeeRepository employeeRepository
            ) : base(baseRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Employee> GetEmployeeByCodeAsync(string code)
        {
            var employee = await _employeeRepository.GetEmployeeByCode(code);
            if (employee == null)
                throw new Exception("Employee not found");

            return employee;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(Guid departmentId)
        {
            return await _employeeRepository.GetEmployeesByDepartmentId(departmentId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionIdAsync(Guid positionId)
        {
            return await _employeeRepository.GetEmployeesByPositionId(positionId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByFilterAsync(Guid? departmentId, Guid? positionId, decimal? salaryFrom, decimal? salaryTo, int? gender, DateTime? hireDateFrom, DateTime? hireDateTo)
        {
            return await _employeeRepository.GetEmployeesByFilterAsync(departmentId, positionId, salaryFrom, salaryTo, gender, hireDateFrom, hireDateTo);
        }

        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();

            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự "));
            }

            if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                errors.Add(new ValidationError("EmployeeName", "Tên nhân viên không được để trống"));
            }

            if (employee.DateOfBirth.HasValue && employee.DateOfBirth > DateTime.Now)
            {
                errors.Add(new ValidationError("DateOfBirth", "Ngày sinh không được lớn hơn ngày hiện tại"));
            }

            if (!string.IsNullOrEmpty(employee.Email))
            { 
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(employee.Email, emailPattern))
                {
                    errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
                }
            }

            
            if (!string.IsNullOrEmpty(employee.PhoneNumber))
            {
                var phonePattern = @"^[0-9]{10}$";
                if (!Regex.IsMatch(employee.PhoneNumber, phonePattern))
                {
                    errors.Add(new ValidationError("PhoneNumber", "Số điện thoại phải có 10 số"));
                }
            }

            //Check trùng code
            if (!string.IsNullOrEmpty(employee.EmployeeCode))
            {
                
                Guid? excludeId = employee.State == ModelSate.Update ? employee.EmployeeID : null;

                bool isDuplicate = _employeeRepository.CheckCodeExistsAsync(employee.EmployeeCode, excludeId).GetAwaiter().GetResult();

                if (isDuplicate)
                {
                    errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
                }
            }

            return errors;
        }
    }
}