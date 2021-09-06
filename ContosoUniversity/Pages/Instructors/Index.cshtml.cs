using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Instructors
{
    public class IndexModel : PageModel
    {
        private readonly SchoolContext _context;

        public IndexModel( SchoolContext context )
        {
            _context = context;
        }

        public InstructorIndexData InstructorData { get; set; }
        public int InstructorId { get; set; }
        public int CourseId { get; set; }
        public async Task OnGetAsync( int? id, int? courseId )
        {
            InstructorData = new InstructorIndexData();
            InstructorData.Instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .ThenInclude(c => c.Department)
                .OrderBy(i => i.LastName)
                .ToListAsync();

            if (id != null)
            {
                InstructorId = id.Value;
                Instructor instructor = InstructorData.Instructors.Where(i => i.Id == id.Value).Single();
                InstructorData.Courses = instructor.Courses;
            }

            if (courseId != null)
            {
                CourseId = courseId.Value;
                var selectedCourse = InstructorData.Courses.Where(x => x.CourseId == courseId).Single();
                await _context.Entry(selectedCourse).Collection(x => x.Enrollments).LoadAsync();
                foreach (Enrollment enrollment in selectedCourse.Enrollments)
                {
                    await _context.Entry(enrollment).Reference(x => x.Student).LoadAsync();
                }
                InstructorData.Enrollments = selectedCourse.Enrollments;
            }
        }
    }
}
