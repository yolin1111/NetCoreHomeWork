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
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosoUniversityContext _context;

        public DepartmentsController(ContosoUniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            return await _context.Department.ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }
            _context.Entry(department).State = EntityState.Modified;
            department.DateModified = DateTime.Now;
            try
            {

                await _context.Department.FromSqlInterpolated($"EXEC dbo.Department_Update {department.DepartmentId}, {department.Name},{department.Budget},{department.StartDate},{department.InstructorId},{department.RowVersion}").Select(a => new Department { RowVersion = a.RowVersion }).ToListAsync();
                //department.RowVersion = listDep[0].RowVersion;
                //return CreatedAtAction("GetDepartment", new { RowVersion = listDep[0].RowVersion }, department);

                //Console.WriteLine("OK");
                //await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //return CreatedAtAction("GetDepartment", new { RowVersion = listDep[0].RowVersion }, department);
            return NoContent();


            // 原始程式碼
            // if (id != department.DepartmentId)
            // {
            //     return BadRequest();
            // }

            // _context.Entry(department).State = EntityState.Modified;

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!DepartmentExists(id))
            //     {
            //         return NotFound();
            //     }
            //     else
            //     {
            //         throw;
            //     }
            // }

            // return NoContent();
        }

        // POST: api/Departments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            var listDep = await _context.Department.FromSqlInterpolated($"EXEC dbo.Department_Insert {department.Name},{department.Budget},{department.StartDate},{department.InstructorId}").Select(a => new Department { DepartmentId = a.DepartmentId, RowVersion = a.RowVersion }).ToListAsync();
            department.DepartmentId = listDep[0].DepartmentId;
            department.RowVersion = listDep[0].RowVersion;
            return CreatedAtAction("GetDepartment", new { id = listDep[0].DepartmentId }, department);

            // DepartmentId 會出現 0
            // await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.Department_Insert {department.Name},{department.Budget},{department.StartDate},{department.InstructorId}");
            // return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);



            // 原始程式碼
            // _context.Department.Add(department);
            // await _context.SaveChangesAsync();

            // return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.Department_Delete {department.DepartmentId},{department.RowVersion}");


            //_context.Database.ExecuteSqlRaw("DELETE dbo.Department WHERE DepartmentID = '17' ");
            // await _context.SaveChangesAsync();
            // Console.WriteLine("DEL");
            // Console.WriteLine(aa);
            return department;

            // 原始程式碼
            // var department = await _context.Department.FindAsync(id);
            // if (department == null)
            // {
            //     return NotFound();
            // }

            // _context.Department.Remove(department);
            // await _context.SaveChangesAsync();

            // return department;
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentId == id);
        }
    }
}
