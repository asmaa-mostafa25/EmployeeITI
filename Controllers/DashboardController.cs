using EmployeeDep.Models;
using EmployeeDep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDep.Controllers
{
    public class DashboardController : Controller
    {
        AppDbContext _appDbContext = new AppDbContext();

        public async Task<IActionResult> Index()
        {
            // Part 8: retrieve all statistics asynchronously.
            var totalEmployeesTask = _appDbContext.Employees.CountAsync();
            var totalDepartmentsTask = _appDbContext.Departments.CountAsync();

            await Task.WhenAll(totalEmployeesTask, totalDepartmentsTask);

            var vm = new DashboardViewModel
            {
                TotalEmployees = await totalEmployeesTask,
                TotalDepartments = await totalDepartmentsTask
            };

            if (vm.TotalEmployees > 0)
            {
                vm.AverageSalary = await _appDbContext.Employees.AverageAsync(e => e.Salary);
                vm.HighestSalary = await _appDbContext.Employees.MaxAsync(e => e.Salary);
                vm.LowestSalary = await _appDbContext.Employees.MinAsync(e => e.Salary);
            }

            return View(vm);
        }
    }
}
