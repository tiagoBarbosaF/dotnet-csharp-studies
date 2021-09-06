using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Courses
{
    public class EditModel : DepartmentNamePageModel
    {
        private readonly Data.SchoolContext _context;

        public EditModel( Data.SchoolContext context )
        {
            _context = context;
        }

        [BindProperty]
        public Course Course { get; set; }

        public async Task<IActionResult> OnGetAsync( int? id )
        {
            if (id == null)
                return NotFound();

            Course = await _context.Courses
                .Include(c => c.Department).FirstOrDefaultAsync(m => m.CourseId == id);

            if (Course == null)
                return NotFound();

            PopulateDepartmentsDropDownList(_context, Course.DepartmentId);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync( int? id )
        {
            if (id == null)
                return NotFound();

            var courseToUpdate = await _context.Courses.FindAsync(id);

            if (courseToUpdate == null)
                return NotFound();

            if (await TryUpdateModelAsync<Course>(courseToUpdate, "course", c => c.Credits, c => c.DepartmentId, c => c.Title))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            PopulateDepartmentsDropDownList(_context, courseToUpdate.DepartmentId);
            return Page();
        }
    }
}
