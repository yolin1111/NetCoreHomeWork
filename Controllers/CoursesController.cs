using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreHomeWork.Models;

namespace NetCoreHomeWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ContosoUniversityContext _context;

        public CoursesController(ContosoUniversityContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourse()
        {
            return await _context.Course.Where(a => a.IsDeleted == false).ToListAsync();
        }

        [HttpGet("vwcs")]
        public async Task<ActionResult<IEnumerable<VwCourseStudents>>> GetvwcsCourse()
        {
            return await _context.VwCourseStudents.ToListAsync();
            //return await _context.Course.ToListAsync();
        }

        //要加入 IEnumerable 這似乎是LINQ的東西
        [HttpGet("vwcs/{id}")]
        public async Task<ActionResult<IEnumerable<VwCourseStudents>>> GetvwcsCourse(int id)
        //public async Task<ActionResult<List<VwCourseStudents>>> GetvwcsCourse(int id)
        {
            var course = await _context.VwCourseStudents.Where(a => a.CourseId == id).ToListAsync();

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        [HttpGet("vwcsc")]
        public async Task<ActionResult<IEnumerable<VwCourseStudentCount>>> GetvwcscCourse()
        {
            return await _context.VwCourseStudentCount.ToListAsync();
            //return await _context.Course.ToListAsync();
        }
        [HttpGet("vwcsc/{id}")]
        public async Task<ActionResult<IEnumerable<VwCourseStudentCount>>> GetvwcscCourse(int id)
        //public async Task<ActionResult<List<VwCourseStudentCount>>> GetvwcsCourse(int id)
        {
            var course = await _context.VwCourseStudentCount.Where(a => a.CourseId == id).ToListAsync();

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }


        //請用 Raw SQL Query 的方式查詢 vwDepartmentCourseCount 檢視表的內容
        [HttpGet("vwdcc")]
        public async Task<ActionResult<IEnumerable<VwDepartmentCourseCount>>> GetvwdccCourse()
        //public async Task<ActionResult<List<VwCourseStudentCount>>> GetvwcsCourse(int id)
        {
            var vwdcc = await _context.VwDepartmentCourseCount.FromSqlRaw(" SELECT * FROM vwDepartmentCourseCount ").ToListAsync();

            if (vwdcc == null)
            {
                return NotFound();
            }

            return vwdcc;
        }

        [HttpGet("vwdcc/{id}")]
        public async Task<ActionResult<IEnumerable<VwDepartmentCourseCount>>> GetvwdccCourse(int id)
        //public async Task<ActionResult<List<VwCourseStudentCount>>> GetvwcsCourse(int id)
        {
            var vwdcc = await _context.VwDepartmentCourseCount.FromSqlInterpolated($"SELECT * FROM vwDepartmentCourseCount WHERE DepartmentID = {id}").ToListAsync();

            if (vwdcc == null)
            {
                return NotFound();
            }

            return vwdcc;
        }


        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourse(int id)
        {
            //var course = await _context.Course.FindAsync(id);
            var course = await _context.Course.Where(a => a.IsDeleted == false && a.CourseId == id).ToListAsync();
            if (course == null)
            {
                return NotFound();
            }

            return course;
        }


        // PUT: api/Courses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.CourseId)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;
            course.DateModified = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Courses
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            _context.Course.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Course>> DeleteCourse(int id)
        {
            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            //_context.Course.Remove(course);
            _context.Entry(course).State = EntityState.Modified;
            course.IsDeleted = true;
            await _context.SaveChangesAsync();


            return course;
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }
    }
}
