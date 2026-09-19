namespace EmployeeDep.ViewModels
{
    public class EmployeeIndexViewModel
    {
        public List<EmployeeWithDepartmentViewModel> Employees { get; set; } = new();

        public string? Search { get; set; }

        public int PageSize { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }
    }
}
