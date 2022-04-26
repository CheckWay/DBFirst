using System;
using System.Linq;
using System.Text;
using DBFirst.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace DBFirst
{
    class Program
    {
        static private EmployeesContext _context = new EmployeesContext();

        static void Main(string[] args)
        {
            Console.WriteLine(Zadanie7());
        }

        static string Zadanie1()
        {
            var employees = _context.Employees
                .Where(e => e.Salary > 48000)
                .OrderBy(e => e.LastName)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle
                })
                .ToList();
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle}");

            }

            return sb.ToString().TrimEnd();
        }

        static string Zadanie2()
        {
            var newAddress = new Addresses()
            {
                TownId = 30,
                AddressText = "Grove street"
            };
            _context.Addresses.Add(newAddress);
            var employees = _context.Employees.Where(e => e.LastName.Equals("Brown")).ToList();
            foreach (var e in employees)
            {
                e.Address = newAddress;
            }

            _context.SaveChanges();
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine(
                    $"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.DepartmentId} {e.ManagerId} {e.HireDate} {e.Salary} {e.AddressId}");

            }

            return sb.ToString().TrimEnd();
        }

        static string Zadanie3()
        {
            var employees = _context.Employees
                .Join(_context.EmployeesProjects,
                    e => e.EmployeeId,
                    ep => ep.EmployeeId,
                    (e, ep) => new
                    {
                        e.EmployeeId,
                        e.ManagerId,
                        ep.Project
                    })
                .Where(p => p.Project.StartDate.Year >= 2002 && p.Project.StartDate.Year <= 2005)
                .Take(5)
                .ToList();
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.Append($"Employee id = {e.EmployeeId}, Manager id={e.ManagerId}, " + $"{e.Project.StartDate}");
                if (e.Project.EndDate != null) sb.AppendLine($" {e.Project.EndDate}");
                else sb.AppendLine(" Проект не окончен");
            }

            return sb.ToString().TrimEnd();
        }

        static string Zadanie4()
        {
            int id = Convert.ToInt32(Console.ReadLine());
            var employees = _context.Employees
                .Where(e => e.EmployeeId == id)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.EmployeesProjects
                })
                .ToList();
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.Append($"{e.FirstName} {e.LastName}, {e.MiddleName} {e.JobTitle}");
                if (e.EmployeesProjects.Count == 0) sb.AppendLine("У данного рабочего нет проектов");
                int i = 1;
                foreach (var ep in e.EmployeesProjects)
                {
                    sb.Append($"\nПроект {i}: {ep.ProjectId}");
                    i++;
                }
            }

            return sb.ToString().TrimEnd();
        }

        static string Zadanie5()
        {
            var departments = _context.Departments
                .Where(d => d.Employees.Count < 5)
                .OrderByDescending(d => d.Employees.Count)
                .Select(d => new
                {
                    d.Name,
                    d.Employees.Count
                })
                .ToList();
            var sb = new StringBuilder();
            foreach (var d in departments)
            {
                sb.Append($"{d.Name} {d.Count}\n");
            }
            return sb.ToString().TrimEnd();
        }

        static string Zadanie6()
        {
            var departmentId = Convert.ToInt32(Console.ReadLine());
            var percentIncrease = Convert.ToDouble(Console.ReadLine());
            percentIncrease = percentIncrease / 100 + 1;
            var employees = _context.Employees
                .Where(e => e.DepartmentId == departmentId)
                .ToList();
            foreach (var e in employees)
            {
                e.Salary *= (decimal) percentIncrease;
            }

            _context.SaveChanges();
            return "Зарплаты успешно увеличены";
        }

        static string Zadanie7()
        {
            var departmentId = Convert.ToInt32(Console.ReadLine());
            var employees = _context.Employees
                .Where(e => e.DepartmentId == departmentId)
                .ToList();
            var departmentToRelocate = new Departments()
            {
                Name = "Wall street",
                ManagerId = 69
            };
            _context.Departments.Add(departmentToRelocate);
            _context.SaveChanges();
            foreach (var e in employees)
            {
                e.DepartmentId = departmentToRelocate.DepartmentId;
                e.ManagerId = departmentToRelocate.ManagerId;
            }

            var department = _context.Departments
                .First(d => d.DepartmentId == departmentId);
            _context.Remove(department);
            _context.SaveChanges();
            return "Отдел успешно удален";
        }

        static string Zadanie8()
        {
            Console.WriteLine("Введите название города");
            var townName = Convert.ToString(Console.ReadLine());
            var town = _context.Towns
                .Where(t => t.Name.Equals(townName))
                .Include(t => t.Addresses)
                .ToList();
            foreach (var t in town)
            {
                foreach (var a in t.Addresses)
                {
                    var employees = _context.Employees
                        .Where(e => e.AddressId == a.AddressId)
                        .ToList();
                    foreach (var e in employees)
                    {
                        e.AddressId = null;
                    }

                    _context.Remove(a);
                }

                _context.Remove(t);
            }

            _context.SaveChanges();
            return "Город удален";
        }

        static void Zadanie1Linq()
        {
            var employees = from e in _context.Employees
                where e.Salary > 40000
                orderby e.LastName
                select e;
            foreach (var e in employees)
            {
                Console.WriteLine($"{e.EmployeeId}{e.FirstName}{e.LastName}{e.Salary}");
            }
        }
        static string Zadanie2Linq()
        {
            var newAddress = new Addresses()
            {
                TownId = 30,
                AddressText = "Grove street"
            };
            _context.Addresses.Add(newAddress);
            var employees = from e in _context.Employees
                where e.LastName.Equals("Brown")
                select e;
            foreach (var e in employees)
            {
                e.Address = newAddress;
            }

            _context.SaveChanges();
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine(
                    $"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.DepartmentId} {e.ManagerId} {e.HireDate} {e.Salary} {e.AddressId}");

            }

            return sb.ToString().TrimEnd();
        }

        static string Zadanie3Linq()
        {
            var employees = (from e in _context.Employees
                    join ep in _context.EmployeesProjects
                        on e.EmployeeId equals ep.EmployeeId
                    where (ep.Project.StartDate.Year >= 2002 && ep.Project.StartDate.Year <= 2005)
                    select new
                    {
                        e.EmployeeId,
                        e.ManagerId,
                        ep.Project
                    }).Take(5)
                .ToList();

            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.Append($"Employee id = {e.EmployeeId}, Manager id={e.ManagerId}, " + $"{e.Project.StartDate}");
                if (e.Project.EndDate != null) sb.AppendLine($" {e.Project.EndDate}");
                else sb.AppendLine(" Проект не окончен");
            }

            return sb.ToString().TrimEnd();
        }
        
        static string Zadanie4Linq()
        {
            int id = Convert.ToInt32(Console.ReadLine());
            var employees =
                (from e in _context.Employees
                    where (e.EmployeeId == id)
                    select new
                    {
                        e.FirstName,
                        e.LastName,
                        e.MiddleName,
                        e.JobTitle,
                        e.EmployeesProjects
                    }).ToList();
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.Append($"{e.FirstName} {e.LastName}, {e.MiddleName} {e.JobTitle}");
                if (e.EmployeesProjects.Count == 0) sb.AppendLine("У данного рабочего нет проектов");
                int i = 1;
                foreach (var ep in e.EmployeesProjects)
                {
                    sb.Append($"\nПроект {i}: {ep.ProjectId}");
                    i++;
                }
            }
            return sb.ToString().TrimEnd();
        }

        static string Zadanie5Linq()
        {
            var departments =
                (from d in _context.Departments
                    where (d.Employees.Count < 5)
                    select new
                    {
                        d.Name,
                        d.Employees.Count
                    }).ToList();
            var sb = new StringBuilder();
            foreach (var d in departments)
            {
                sb.Append($"{d.Name} {d.Count}\n");
            }

            return sb.ToString().TrimEnd();
        }

        static string Zadanie6Linq()
        {
            var departmentId = Convert.ToInt32(Console.ReadLine());
            var percentIncrease = Convert.ToDouble(Console.ReadLine());
            percentIncrease = percentIncrease / 100 + 1;
            var employees =
                from e in _context.Employees
                where (e.DepartmentId == departmentId)
                select e;
            foreach (var e in employees)
            {
                e.Salary *= (decimal) percentIncrease;
            }

            _context.SaveChanges();
            return "Зарплаты успешно увеличены";
        }

        static string Zadanie7Linq()
        {
            var departmentId = Convert.ToInt32(Console.ReadLine());
            var department =
                from d in _context.Departments
                where (d.DepartmentId == departmentId)
                select d;
            var employees =
                (from e in _context.Employees
                    where (e.DepartmentId == departmentId)
                    select e).ToList();
            var departmentToRelocate = new Departments()
            {
                Name = "Wall street",
                ManagerId = 69
            };
            if (employees == null)
            {
                return "Отдел не найден";
            }
            else
            {
                _context.Departments.Add(departmentToRelocate);
                foreach (var e in employees)
                {
                    e.DepartmentId = departmentToRelocate.DepartmentId;
                    e.DepartmentId = departmentToRelocate.ManagerId;
                    _context.SaveChanges();
                }
                _context.Remove(department);
                _context.SaveChanges();
                return "Отдел успешно удален";
            }
        }

        static void Zadanie8Linq()
        {
            Console.WriteLine("Enter town name");
            var townId = Convert.ToInt32(Console.ReadLine());
            var town =
                (from t in _context.Towns
                    where (t.Name.Equals(townId))
                    select t).ToList();
            if (town.Count != 0)
            {
                foreach (var t in town)
                {
                    Console.WriteLine($"{t.TownId} {t.Name}");
                    foreach (var a in t.Addresses)
                    {
                        var employees =
                            (from e in _context.Employees
                                where (e.AddressId == townId)
                                select e).ToList();
                        foreach (var e in employees)
                        {
                            e.Address = null;
                        }

                        _context.Remove(a);
                    }

                    _context.Remove(t);
                }

                _context.SaveChanges();
            }
            else Console.WriteLine("Города с таким Id не найдено");
        }
    }
}
