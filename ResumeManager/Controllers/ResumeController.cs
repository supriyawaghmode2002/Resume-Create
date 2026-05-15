using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeManager.Data;
using ResumeManager.Models;
using X.PagedList;
using X.PagedList.EF;
using X.PagedList.Extensions;

namespace ResumeManager.Controllers
{
    public class ResumeController : Controller
    {
        private readonly ResumeDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ResumeController(ResumeDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;
            var data = await _context.Applicants
                                     .OrderBy(a => a.Id)
                                     .ToPagedListAsync(page, pageSize);

           return View(data);
        }


        
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Applicant
            {
                Experiences = new List<Experience> { new Experience() }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Applicant applicant, IFormFile PhotoFile)
        {
            if (!ModelState.IsValid)
            {
                if (applicant.Experiences == null || applicant.Experiences.Count == 0)
                    applicant.Experiences = new List<Experience> { new Experience() };

                return View(applicant);
            }

            
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadPath);
                string fileName = Guid.NewGuid() + "_" + PhotoFile.FileName;

                using var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create);
                PhotoFile.CopyTo(stream);

                applicant.PhotoUrl = "/uploads/" + fileName;
            }
            else
            {
                applicant.PhotoUrl = "/images/default.jpg";
            }

           
            applicant.Experiences = applicant.Experiences?
                .Where(e => !string.IsNullOrWhiteSpace(e.CompanyName) ||
                            !string.IsNullOrWhiteSpace(e.Designation) ||
                            e.YearsWorkes > 0)
                .ToList() ?? new List<Experience>();

                 foreach (var exp in applicant.Experiences)
                 exp.Applicant = applicant;

                 _context.Applicants.Add(applicant);
                 _context.SaveChanges();

                 return RedirectToAction(nameof(Index));
        }

       
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var applicant = _context.Applicants
                .Include(a => a.Experiences)
                .FirstOrDefault(a => a.Id == id);

            if (applicant == null) return NotFound();

            if (!applicant.Experiences.Any())
                applicant.Experiences = new List<Experience> { new Experience() };

            return View(applicant);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Applicant applicant, IFormFile? PhotoFile)
        {
            var existingApplicant = _context.Applicants
                .Include(a => a.Experiences)
                .FirstOrDefault(a => a.Id == id);

            if (existingApplicant == null) return NotFound();
            if (!ModelState.IsValid) return View(applicant);

           
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadPath);
                string fileName = Guid.NewGuid() + "_" + PhotoFile.FileName;

                using var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create);
                PhotoFile.CopyTo(stream);

                existingApplicant.PhotoUrl = "/uploads/" + fileName;
            }

            
                existingApplicant.Name = applicant.Name;
                existingApplicant.Gender = applicant.Gender;
                existingApplicant.Age = applicant.Age;
                existingApplicant.Qulification = applicant.Qulification;

           
                existingApplicant.Experiences = applicant.Experiences?
                   .Where(e => !string.IsNullOrWhiteSpace(e.CompanyName) ||
                               !string.IsNullOrWhiteSpace(e.Designation) ||
                               e.YearsWorkes > 0)
                .Select(e => new Experience
                {
                    Id = e.Id,
                    CompanyName = e.CompanyName,
                    Designation = e.Designation,
                    YearsWorkes = e.YearsWorkes,
                    ApplicantId = id
                }).ToList() ?? new List<Experience>();

                    _context.SaveChanges();
                     return RedirectToAction(nameof(Index));
        }

       
        [HttpGet]
        public IActionResult Details(int id)
        {
            var applicant = _context.Applicants
                .Include(a => a.Experiences)
                .FirstOrDefault(a => a.Id == id);

            if (applicant == null) return NotFound();
            return View(applicant);
        }

        
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var applicant = _context.Applicants.Find(id);
            if (applicant == null) return NotFound();
            return View(applicant);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var applicant = _context.Applicants
                .Include(a => a.Experiences)
                .FirstOrDefault(a => a.Id == id);

            if (applicant == null) return NotFound();

            if (applicant.Experiences.Any())
                _context.Experiences.RemoveRange(applicant.Experiences);

         
            if (!string.IsNullOrEmpty(applicant.PhotoUrl) && applicant.PhotoUrl.Contains("uploads"))
            {
                var filePath = Path.Combine(_env.WebRootPath, applicant.PhotoUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Applicants.Remove(applicant);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
