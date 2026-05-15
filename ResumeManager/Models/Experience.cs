using ResumeManager.Models;
public class Experience
{
    public int Id { get; set; }
    public string? CompanyName { get; set; }
    public string? Designation { get; set; }
    public int? YearsWorkes { get; set; }

    public int ApplicantId { get; set; }
    public Applicant? Applicant { get; set; } = new();
}
