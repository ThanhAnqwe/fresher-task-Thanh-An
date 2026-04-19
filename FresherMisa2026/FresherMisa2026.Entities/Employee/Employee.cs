using FresherMisa2026.Entities.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace FresherMisa2026.Entities.Employee
{
    [ConfigTable("Employee", true, "EmployeeCode")]
    public class Employee : BaseModel
    {
        [Key]
        public Guid EmployeeID { get; set; }

        public string EmployeeCode { get; set; }

        public string EmployeeName { get; set; }

        public int? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public Guid DepartmentID { get; set; }

        public Guid PositionID { get; set; }

        public decimal? Salary { get; set; }

        public DateTime? HireDate { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}