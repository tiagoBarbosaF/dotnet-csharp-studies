using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContosoUniversity.Pages.Instructors
{
  public class CreateModel : InstructorCoursesPageModel
  {
    private readonly Data.SchoolContext _context;
    private readonly ILogger<InstructorCoursesPageModel> _logger;

    public CreateModel(Data.SchoolContext context, ILogger<InstructorCoursesPageModel> logger)
    {
      _context = context;
      _logger = logger;
    }

    public IActionResult OnGet()
    {
      var instructor = new Instructor();
      instructor.Courses = new List<Course>();

      PopulateAssignedCourseData(_context, instructor);
      return Page();
    }

    [BindProperty] public Instructor Instructor { get; set; }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync(string[] selectedCourses)
    {
      var newInstructor = new Instructor();

      if (selectedCourses.Length > 0)
      {
        newInstructor.Courses = new List<Course>();
        _context.Courses.Load();
      }

      foreach (var course in selectedCourses)
      {
        var foundCourse = await _context.Courses.FindAsync(int.Parse(course));
        if (foundCourse != null)
          newInstructor.Courses.Add(foundCourse);
        else
          _logger.LogWarning($"Course {course} not found");
      }

      try
      {
        if (await TryUpdateModelAsync<Instructor>(
          newInstructor,
          "Instructor",
          i => i.LastName,
          i => i.FirstMidName,
          i => i.HireDate,
          i => i.OfficeAssignment))
        {
          _context.Instructors.Add(newInstructor);
          await _context.SaveChangesAsync();
          return RedirectToPage("./Index");
        }
      }
      catch (Exception e)
      {
        _logger.LogError(e.Message);
      }

      PopulateAssignedCourseData(_context, newInstructor);
      return Page();
    }
  }
}