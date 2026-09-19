using EmployeeDep.Models;
using EmployeeDep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDep.Controllers
{

    public class EmployeeController : Controller
    {
        AppDbContext _appDbContext = new AppDbContext();

        private const string SearchCookieKey = "EmployeeSearch";
        private const string PageSizeCookieKey = "PageSize";

        // Bonus #1: require the user to be logged in (Session) before reaching any
        // Employee page.
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                context.Result = RedirectToAction("Login", "Account");
                return;
            }

            base.OnActionExecuting(context);
        }

        // Part 6: search box (by Employee Name) whose text survives a page refresh
        // via a Cookie. Bonus #6: page size (5/10/20) is also remembered in a Cookie.
        public async Task<IActionResult> Index(string? search, int? pageSize, int page = 1)
        {
            // --- Search text ---
            if (search != null)
            {
                // The user actively searched (even with an empty box, to clear it):
                // save the new value so it survives a refresh.
                Response.Cookies.Append(
                    SearchCookieKey,
                    search,
                    new CookieOptions { Expires = DateTimeOffset.Now.AddDays(7) });
            }
            else
            {
                // No query string (e.g. plain refresh or first visit): fall back to
                // whatever was previously saved in the cookie.
                search = Request.Cookies[SearchCookieKey];
            }

            // --- Page size ---
            if (pageSize.HasValue)
            {
                Response.Cookies.Append(
                    PageSizeCookieKey,
                    pageSize.Value.ToString(),
                    new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });
            }
            else if (int.TryParse(Request.Cookies[PageSizeCookieKey], out var savedPageSize))
            {
                pageSize = savedPageSize;
            }
            else
            {
                pageSize = 10;
            }

            var query = _appDbContext.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e => e.Name.ToLower().Contains(search.ToLower()));
            }

            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize.Value);
            if (page < 1) page = 1;

            var employees = await query
                .OrderBy(e => e.Id)
                .Skip((page - 1) * pageSize.Value)
                .Take(pageSize.Value)
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

            var vm = new EmployeeIndexViewModel
            {
                Employees = employees,
                Search = search,
                PageSize = pageSize.Value,
                Page = page,
                TotalPages = totalPages
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AddEmployee()
        {
            var departments = await _appDbContext.Departments.ToListAsync();

            var vm = new EmployeeViewModel
            {
                Departments = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList()
            };

            return View("AddEmployee", vm);
        }


        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _appDbContext.Departments.ToListAsync();

                viewModel.Departments = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();

                return View("AddEmployee", viewModel);
            }


            var employeeExists = await _appDbContext.Employees
                .AnyAsync(e =>
                    e.Name.ToLower() == viewModel.Name.ToLower() &&
                    e.DepartmentId == viewModel.DepartmentId);

            if (employeeExists)
            {
                ModelState.AddModelError(
                    "Name",
                    "An employee with the same name already exists in this department.");

                var departments = await _appDbContext.Departments.ToListAsync();

                viewModel.Departments = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();

                return View("AddEmployee", viewModel);
            }


            var employee = new Employee
            {
                Name = viewModel.Name,
                Age = viewModel.Age,
                Salary = viewModel.Salary,
                JobTitle = viewModel.JobTitle,
                DepartmentId = viewModel.DepartmentId
            };


            _appDbContext.Employees.Add(employee);

            await _appDbContext.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> EditEmployee(int id)
        {
            var employee = await _appDbContext.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound();

            var departments = await _appDbContext.Departments
                .ToListAsync();

            var vm = new EmployeeViewModel
            {
                Name = employee.Name,
                Age = employee.Age,
                Salary = employee.Salary,
                JobTitle = employee.JobTitle,
                DepartmentId = employee.DepartmentId,

                Departments = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                    Selected = d.Id == employee.DepartmentId
                }).ToList()
            };

            return View("EditEmployee", vm);
        }
        [HttpPost]
        public async Task<IActionResult> EditEmployee(
    int id,
    EmployeeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _appDbContext.Departments
                    .ToListAsync();

                viewModel.Departments = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                    Selected = d.Id == viewModel.DepartmentId
                }).ToList();

                return View("EditEmployee", viewModel);
            }

            var employee = await _appDbContext.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound();

            var employeeExists = await _appDbContext.Employees
                .AnyAsync(e =>
                    e.Id != id &&
                    e.Name.ToLower() == viewModel.Name.ToLower() &&
                    e.DepartmentId == viewModel.DepartmentId);

            if (employeeExists)
            {
                ModelState.AddModelError(
                    "Name",
                    "An employee with the same name already exists in this department.");

                var departments = await _appDbContext.Departments
                    .ToListAsync();

                viewModel.Departments = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                    Selected = d.Id == viewModel.DepartmentId
                }).ToList();

                return View("EditEmployee", viewModel);
            }

            employee.Name = viewModel.Name;
            employee.Age = viewModel.Age;
            employee.Salary = viewModel.Salary;
            employee.JobTitle = viewModel.JobTitle;
            employee.DepartmentId = viewModel.DepartmentId;

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _appDbContext.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound();

            _appDbContext.Employees.Remove(employee);

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
