using EmployeeDep.Models;
using EmployeeDep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EmployeeDep.Controllers
{
    public class DepartmentController : Controller
    {
        AppDbContext _appDbContext = new AppDbContext();

        private const string RecentDepartmentsSessionKey = "RecentDepartments";
        private const string LastVisitedDepartmentSessionKey = "LastVisitedDepartment";

        // Bonus #1: require the user to be logged in (Session) before reaching any
        // Department page.
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                context.Result = RedirectToAction("Login", "Account");
                return;
            }

            base.OnActionExecuting(context);
        }

        // Part 3.2 / Bonus #4: remembers the last 5 visited departments and the single
        // most-recently visited one, both kept in Session.
        private void TrackRecentDepartment(string departmentName)
        {
            var json = HttpContext.Session.GetString(RecentDepartmentsSessionKey);

            var recent = string.IsNullOrEmpty(json)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();

            recent.Remove(departmentName);
            recent.Insert(0, departmentName);

            if (recent.Count > 5)
            {
                recent = recent.Take(5).ToList();
            }

            HttpContext.Session.SetString(RecentDepartmentsSessionKey, JsonSerializer.Serialize(recent));
            HttpContext.Session.SetString(LastVisitedDepartmentSessionKey, departmentName);
        }

        public async Task<IActionResult> Index()
        {
            var vm = await _appDbContext.Departments
                .Select(r => new DepartmentWithEmpCount
                {
                    Id = r.Id,
                    Name = r.Name,
                    ManagerName = r.ManagerName,
                    EmpCount = r.Employees.Count,
                })
                .ToListAsync();

            // Part 3.2: show the last 5 visited departments.
            var recentJson = HttpContext.Session.GetString(RecentDepartmentsSessionKey);
            ViewBag.RecentDepartments = string.IsNullOrEmpty(recentJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(recentJson);

            // Bonus #4: highlight the last visited department in the table.
            ViewBag.LastVisitedDepartment = HttpContext.Session.GetString(LastVisitedDepartmentSessionKey);

            return View(vm);
        }

        public async Task<IActionResult> GetDepById(int id)
        {
            var dep = await _appDbContext.Departments
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dep == null)
                return NotFound();

            TrackRecentDepartment(dep.Name);

            return View("GetById", dep);
        }

        [HttpGet]
        public IActionResult AddDepartment()
        {
            return View("AddDepartment");
        }

        [HttpPost]
        public async Task<IActionResult> AddDepartment(
            AddDepartmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("AddDepartment", viewModel);
            }

            var dep = new Department
            {
                Name = viewModel.Name,
                ManagerName = viewModel.ManagerName
            };

            _appDbContext.Departments.Add(dep);

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> EditDepartment(int id)
        {
            var dep = await _appDbContext.Departments
                .FirstOrDefaultAsync(x => x.Id == id);
            if (dep == null)
            {
                return NotFound();
            }
            var viewModel = new AddDepartmentViewModel
            {

                Name = dep.Name,
                ManagerName = dep.ManagerName
            };
            return View("EditDepartment", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditDepartment(int id, AddDepartmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("EditDepartment", viewModel);
            }

            var dep = await _appDbContext.Departments
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dep == null)
                return NotFound();

            dep.Name = viewModel.Name;
            dep.ManagerName = viewModel.ManagerName;

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Changed from [HttpDelete] to [HttpPost] so it actually matches the
        // <form method="post"> used in the Departments view (an HTML form can only
        // ever submit GET or POST, never a DELETE verb).
        [HttpPost]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var dep = await _appDbContext.Departments
                .FirstOrDefaultAsync(x => x.Id == id);
            if (dep == null)
                return NotFound();
            _appDbContext.Departments.Remove(dep);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ShowEmployees(int id)
        {
            var department = await _appDbContext.Departments
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
                return NotFound();

            TrackRecentDepartment(department.Name);

            var employees = await _appDbContext.Employees
                .Where(e => e.DepartmentId == id)
                .Select(e => new EmployeeWithDepartmentViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Age = e.Age,
                    Salary = e.Salary,
                    JobTitle = e.JobTitle,
                    DepartmentName = e.Department.Name
                })
                .ToListAsync();

            return View("ShowEmployees", employees);
        }
    }
}
