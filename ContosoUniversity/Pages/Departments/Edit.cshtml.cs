using System;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Departments
{
  public class EditModel : PageModel
  {
    private readonly SchoolContext _context;

    public EditModel(SchoolContext context)
    {
      _context = context;
    }

    [BindProperty] public Department Department { get; set; }

    public SelectList InstructorNameSl { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
      Department = await _context.Departments
        .Include(d => d.Administrator)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.DepartmentId == id);

      if (Department == null)
        return NotFound();

      InstructorNameSl = new SelectList(_context.Instructors, "Id", "FirstMidName");

      return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync(int id)
    {
      if (!ModelState.IsValid)
        return Page();

      var departmentToUpdate = await _context.Departments
        .Include(i => i.Administrator)
        .FirstOrDefaultAsync(m => m.DepartmentId == id);

      if (departmentToUpdate == null)
        return HandleDeletedDepartment();

      departmentToUpdate.ConcurrencyToken = Guid.NewGuid();

      _context.Entry(departmentToUpdate).Property(d => d.ConcurrencyToken)
        .OriginalValue = Department.ConcurrencyToken;

      if (await TryUpdateModelAsync<Department>(departmentToUpdate, "Department",
        s => s.Name,
        s => s.StartDate,
        s => s.Budget,
        s => s.InstructorId))
      {
        try
        {
          await _context.SaveChangesAsync();
          return RedirectToPage("./Index");
        }
        catch (DbUpdateConcurrencyException e)
        {
          var exceptionEntry = e.Entries.Single();
          var clientValues = (Department) exceptionEntry.Entity;
          var databaseEntry = await exceptionEntry.GetDatabaseValuesAsync();

          if (databaseEntry == null)
          {
            ModelState.AddModelError(string.Empty, "Unable to save. " +
                                                   "The department was deleted by another user.");
            return Page();
          }

          var dbValues = (Department) databaseEntry.ToObject();
          await SetDbErrorMessage(dbValues, clientValues, _context);

          Department.ConcurrencyToken = dbValues.ConcurrencyToken;
          ModelState.Remove($"{nameof(Department)}.{nameof(Department.ConcurrencyToken)}");
        }
      }

      InstructorNameSl = new SelectList(_context.Instructors,
        "Id",
        "FullName",
        departmentToUpdate.InstructorId);
      return Page();
    }

    private IActionResult HandleDeletedDepartment()
    {
      var deletedDepartment = new Department();
      ModelState.AddModelError(string.Empty,
        "Unable to save. The department was deleted by another user.");
      InstructorNameSl = new SelectList(_context.Instructors,
        "Id",
        "FullName",
        Department.InstructorId);
      return Page();
    }

    private async Task SetDbErrorMessage(Department dbValues, Department clientValues, SchoolContext context)
    {
      if (dbValues.Name != clientValues.Name)
        ModelState.AddModelError("Department.Name", $"Current value: {dbValues.Name}");

      if (dbValues.Budget != clientValues.Budget)
        ModelState.AddModelError("Department.Budget", $"Current value: {dbValues.Budget:C}");

      if (dbValues.StartDate != clientValues.StartDate)
        ModelState.AddModelError("Department.StartDate", $"Current value: {dbValues.StartDate:d}");

      if (dbValues.InstructorId != clientValues.InstructorId)
      {
        Instructor dbInstructor = await _context.Instructors.FindAsync(dbValues.InstructorId);
        ModelState.AddModelError("Department.InstructorId", $"Current value: {dbInstructor?.FullName}");
      }

      ModelState.AddModelError(string.Empty,
        "The record you attempted to edit "
        + "was modified by another user after you. The "
        + "edit operation was canceled and the current values in the database "
        + "have been displayed. If you still want to edit this record, click "
        + "the Save button again.");
    }
  }
}