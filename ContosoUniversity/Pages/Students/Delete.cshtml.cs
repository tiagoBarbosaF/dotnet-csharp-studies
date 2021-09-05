using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;
using Microsoft.Extensions.Logging;

namespace ContosoUniversity.Pages.Students
{
  public class DeleteModel : PageModel
  {
    private readonly Data.SchoolContext _context;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(Data.SchoolContext context, ILogger<DeleteModel> logger)
    {
      _context = context;
      _logger = logger;
    }

    [BindProperty] public Student Student { get; set; }

    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
    {
      if (id == null)
        return NotFound();

      Student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

      if (Student == null)
        return NotFound();

      if (saveChangesError.GetValueOrDefault())
        ErrorMessage = String.Format($"Delete {id} failed. Try again");

      return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
      if (id == null)
        return NotFound();

      var student = await _context.Students.FindAsync(id);

      if (student == null)
        return NotFound();

      try
      {
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
      }
      catch (DbUpdateException e)
      {
        _logger.LogError(e, ErrorMessage);
        return RedirectToAction("./Delete", new {id, saveChangesError = true});
      }
    }
  }
}