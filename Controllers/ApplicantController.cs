using Lanina.Public.Web.Api.DTOs;
using Lanina.Public.Web.Api.Models;
using Lanina.Public.Web.Api.ThirdPartyClients.Google;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lanina.Public.Web.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApplicantController : ControllerBase
    {
        private const int FreeMeetingCount = 10;

        private readonly GoogleReCaptchaClient _reCaptchaClient;
        private readonly IHostingEnvironment _env;
        private readonly LaNinaInterviewManager _laninaInterviewManager;
        private readonly int _interviewDateHoursOffset = 3;
        private readonly LaNinaApplicantDbContext _dbContext;
        private readonly IOptions<AdminSettings> _adminSettings;
        private readonly List<ApplicantStatus> _getSetInterviewDateAllowedStatuses = new List<ApplicantStatus> {
            ApplicantStatus.ShouldSelectFirstInterviewDate, ApplicantStatus.ShouldSelectSecondInterviewDate };

        public ApplicantController(
            LaNinaInterviewManager laNinaInterviewManager,
            LaNinaApplicantDbContext dbContext,
            IHostingEnvironment env,
            IOptions<AdminSettings> adminSettings,
            GoogleReCaptchaClient reCaptchaClient)
        {
            _dbContext = dbContext;
            _adminSettings = adminSettings;
            _laninaInterviewManager = laNinaInterviewManager;
            _env = env;
            _reCaptchaClient = reCaptchaClient;
        }


        [HttpPost]
        public IActionResult Apply([FromHeader(Name = "apply-token")]string applyTokenValue, [FromForm]ApplyInput input)
        {
            if (string.IsNullOrWhiteSpace(applyTokenValue))
            {
                ModelState.AddModelError("apply-token", "apply-token header is missing. You can get an apply token from http://laninayazilim.com/getApplyToken.html");
            }
            else
            {
                var applyToken = _dbContext.ApplyTokens.FirstOrDefault(at => at.Value == applyTokenValue);
                if (applyToken == null)
                {
                    ModelState.AddModelError("apply-token", "apply-token header is invalid. You can get an apply token from http://laninayazilim.com/getApplyToken.html");
                }
                else
                {
                    _dbContext.Remove(applyToken);
                    _dbContext.SaveChanges();
                }
            }

            if (input.Resume.Length <= 0 || !string.Equals(input.Resume.ContentType, "application/pdf"))
            {
                ModelState.AddModelError("Resume", "Resume contentType should be application/pdf");
            }

            if (!string.IsNullOrWhiteSpace(input.Flag))
            {
                var flagExists = _dbContext.Flags.Any(f => f.Value == input.Flag.Trim());

                if (!flagExists)
                {
                    ModelState.AddModelError("Flag", "Invalid flag value");
                }
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var emailOrMobilePhoneExists = _dbContext.Applicants.Any(a =>
                a.MobilePhone == input.MobilePhone
                || a.Email == input.Email);

            if (emailOrMobilePhoneExists)
            {
                ModelState.AddModelError("Email || MobilePhone", "An application with the same email or mobilePhone already exists");

                return Conflict(ModelState);
            }

            var newApplicant = new Applicant
            {
                Key = Guid.NewGuid().ToString(),
                Name = input.Name.Trim(),
                Surname = input.Surname.Trim(),
                Email = input.Email.Trim().ToLower(),
                EmailConfirmationKey = Guid.NewGuid().ToString(),
                MobilePhone = input.MobilePhone,
                MobilePhoneConfirmationKey = Guid.NewGuid().ToString(),
                Github = input.Github,
                Linkedin = input.Linkedin,
                CoverLetter = input.CoverLetter,
                Status = ApplicantStatus.New,
                Flag = input.Flag
            };

            _dbContext.Applicants.Add(newApplicant);
            _dbContext.SaveChanges();

            string filePath = null;
            if (input.Resume.Length > 0)
            {
                var ext = Path.GetExtension(input.Resume.FileName);
                var fileName = newApplicant.Id.ToString() + ext;
                filePath = Path.Combine(_env.ContentRootPath, "Resumes", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    input.Resume.CopyTo(fileStream);
                }
            }

            _laninaInterviewManager.SendApplicationReceivedMail(
                newApplicant.Email,
                newApplicant.Name,
                newApplicant.EmailConfirmationKey,
                _adminSettings.Value.AdminEmail,
                filePath);            

            return Ok();
        }

        [HttpPut]
        public IActionResult ConfirmEmail([FromBody]ConfirmEmailInput input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var applicant = _dbContext.Applicants.FirstOrDefault(a => a.EmailConfirmationKey == input.EmailConfirmationKey);

            if (applicant == null) return NotFound(input);

            applicant.IsEmailConfirmed = true;
            applicant.EmailConfirmationKey = null;
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpPut]
        public IActionResult ConfirmMobilePhone([FromBody]ConfirmMobilePhoneInput input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var applicant = _dbContext.Applicants.FirstOrDefault(a => a.MobilePhoneConfirmationKey == input.MobilePhoneConfirmationKey);

            if (applicant == null) return NotFound(input);

            applicant.IsMobilePhoneConfirmed = true;
            applicant.MobilePhoneConfirmationKey = null;
            _dbContext.SaveChanges();

            return Ok();
        }
        [HttpGet]
        public IActionResult GetInterviewDates([FromHeader]string applicantKey)
        {
            if (string.IsNullOrWhiteSpace(applicantKey))
            {
                ModelState.AddModelError("ApplicantKey", "ApplicantKey header value is missing");
                return BadRequest(ModelState);
            }

            var applicant = _dbContext.Applicants.AsNoTracking().FirstOrDefault(a => a.Key == applicantKey);

            if (applicant == null)
            {
                return NotFound(applicantKey);
            }

            var canSelectInterviewDate = _getSetInterviewDateAllowedStatuses.Contains(applicant.Status);

            if (!canSelectInterviewDate)
            {
                return Unauthorized(applicantKey);
            }

            var interviewDateDTOList = _dbContext.InterviewDates
                .AsNoTracking()
                .Where(id =>
                    id.InterviewId == null
                    && id.Date >= DateTime.Now.AddHours(_interviewDateHoursOffset)
                    && id.Date < DateTime.Now.AddDays(8))
                .Select(id => new InterviewDateDTO { Date = id.Date, Key = id.Key })
                .OrderBy(id => id.Date)
                .ToList();

            var output = new GetInterviewDateOutput { InterviewDates = interviewDateDTOList };

            return Ok(output);
        }

        [HttpPost]
        public IActionResult SetInterviewDate([FromBody]SetInterviewDateInput input)
        {
            if ((input.InterviewDate - DateTime.Now).Hours < _interviewDateHoursOffset)
            {
                ModelState.AddModelError("InterviewDate", $"InterviewDate should be atleast {_interviewDateHoursOffset} hours ahead");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applicant = _dbContext.Applicants
                .FirstOrDefault(a => a.Key == input.ApplicantKey);

            if (applicant == null)
            {
                return NotFound(input);
            }

            var canSelectInterviewDate = _getSetInterviewDateAllowedStatuses.Contains(applicant.Status);

            if (!canSelectInterviewDate)
            {
                return Unauthorized(input);
            }

            var interviewDate = _dbContext.InterviewDates.FirstOrDefault(id =>
              id.Key == input.InterviewDateKey &&
              id.Date == input.InterviewDate &&
              id.InterviewId == null);

            if (interviewDate == null)
            {
                return NotFound("InterviewDate is not available");
            }

            var reserveMeetingRoom = _dbContext.Interviews.Count(i =>
                i.InterviewDate.Date.Month == DateTime.Now.Month
                && i.InterviewDate.Date.Year == DateTime.Now.Year) < FreeMeetingCount;

            var processResult = _laninaInterviewManager.ProcessInterview(
                interviewDate.Date,
                applicant.Name,
                applicant.Surname,
                applicant.MobilePhone,
                applicant.Email,
                reserveMeetingRoom,
                _adminSettings.Value.AdminEmail);

            var newInterview = new Interview
            {
                Applicant = applicant,
                InterviewDate = interviewDate,
                KoplanetReservationId = processResult.ReservationId,
                MeetingRoomName = processResult.MeetingRoom != null ? processResult.MeetingRoom.Name : null
            };

            switch (applicant.Status)
            {
                case ApplicantStatus.ShouldSelectFirstInterviewDate:
                    applicant.Status = ApplicantStatus.FirstInterviewDateSet;
                    break;

                case ApplicantStatus.ShouldSelectSecondInterviewDate:
                    applicant.Status = ApplicantStatus.SecondInterviewDateSet;
                    break;
            }

            _dbContext.Interviews.Add(newInterview);
            _dbContext.SaveChanges();

            _laninaInterviewManager.SendInterviewDateSetMail(
                applicant.Email,
                applicant.Name,
                interviewDate.Date,
                _adminSettings.Value.AdminEmail);

            return Ok();
        }

        [HttpGet]
        public IActionResult HealthCheck()
        {
             return Redirect("https://youtu.be/qM0zINtulhM");
        }

        [HttpPut]
        public IActionResult InviteApplicant([FromBody]InviteApplicantInput input, [FromHeader]string secret)
        {
            if (string.IsNullOrWhiteSpace(secret)
                || !_adminSettings.Value.Secret.Equals(secret)) return BadRequest("Secret header missing");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var applicant = _dbContext.Applicants.FirstOrDefault(a => a.Key == input.ApplicantKey);

            if (applicant == null) return NotFound(input);

            switch (input.InvitationType)
            {
                case InvitationType.FirstInterview:
                    applicant.Status = ApplicantStatus.ShouldSelectFirstInterviewDate;
                    break;
                case InvitationType.SecondInterview:
                    applicant.Status = ApplicantStatus.ShouldSelectSecondInterviewDate;
                    break;
            }

            applicant.IsReminderSent = false;
            applicant.Key = Guid.NewGuid().ToString();
            _dbContext.SaveChanges();

            switch (applicant.Status)
            {
                case ApplicantStatus.ShouldSelectFirstInterviewDate:
                    _laninaInterviewManager.SendInvitationForFirstInterviewMail(
                    applicant.Email,
                    applicant.Name,
                    applicant.Key,
                    _adminSettings.Value.AdminEmail);
                    break;
                case ApplicantStatus.ShouldSelectSecondInterviewDate:
                    _laninaInterviewManager.SendInvitationForSecondInterviewMail(
                    applicant.Email,
                    applicant.Name,
                    applicant.Key,
                    _adminSettings.Value.AdminEmail);
                    break;
            }

            return Ok();
        }

        [HttpPut]
        public IActionResult RejectApplicant([FromBody]RejectApplicantInput input, [FromHeader]string secret)
        {
            if (string.IsNullOrWhiteSpace(secret) || !_adminSettings.Value.Secret.Equals(secret)) return BadRequest("Secret header missing");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var applicant = _dbContext.Applicants.FirstOrDefault(a => a.Key == input.ApplicantKey);

            if (applicant == null) return NotFound(input);

            applicant.Status = ApplicantStatus.Rejected;
            applicant.Key = Guid.NewGuid().ToString();
            applicant.RejectReason = input.Reason;

            _dbContext.SaveChanges();

            _laninaInterviewManager.SendRejectApplicantMail(applicant.Email, applicant.Name, applicant.RejectReason);

            return Ok();
        }

        [HttpGet]
        public IActionResult GetInterviews([FromHeader]string secret)
        {
            if (string.IsNullOrWhiteSpace(secret) || !_adminSettings.Value.Secret.Equals(secret))
                return BadRequest("Secret header missing");

            var interviews = _dbContext.Interviews
                .Include(i => i.Applicant)
                .Include(i => i.InterviewDate)
                .Select(i => new InterviewDto
                {
                    ApplicantId = i.ApplicantId.ToString(),
                    Email = i.Applicant.Email,
                    FullName = i.Applicant.Name + " " + i.Applicant.Surname,
                    Github = i.Applicant.Github,
                    InterviewDate = i.InterviewDate.Date.ToString("dd/MM/yyyy HH:mm"),
                    KoplanetReservationId = i.KoplanetReservationId,
                    Linkedin = i.Applicant.Linkedin,
                    MeetingRoomName = i.MeetingRoomName,
                    MobilePhone = i.Applicant.MobilePhone
                })
                .ToList();

            return Ok(new GetInterviewsOutput { Interviews = interviews });
        }

        [HttpPost]
        public IActionResult AddInterviewDates([FromBody]AddInterviewDatesInput input, [FromHeader]string secret)
        {
            if (string.IsNullOrWhiteSpace(secret) || !_adminSettings.Value.Secret.Equals(secret)) return BadRequest("Secret header missing");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (input.Dates == null || !input.Dates.Any())
            {
                return BadRequest();
            }

            foreach (var date in input.Dates)
            {
                var newInterviewDate = new InterviewDate
                {
                    Date = date,
                    Key = Guid.NewGuid().ToString()
                };
                _dbContext.InterviewDates.Add(newInterviewDate);
            }

            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IActionResult GetAllFlags([FromHeader]string flag)
        {
            List<Flag> result = new List<Flag>();
            
            if (Regex.Match(flag, "c.*o.*m.*m.*i.*t", RegexOptions.IgnoreCase).Success)
            {
                return BadRequest("Nope!");
            }

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                result = _dbContext.Flags.AsNoTracking().FromSql("SELECT * FROM Flags WHERE Value = '" + flag + "'").ToList();
            }

            var flags = _dbContext.Flags.ToList();

            if (result.Any(r=> !flags.Select(f=> f.Id).ToList().Contains(r.Id)) 
                || result.Any(r=> !flags.Select(f=> f.Value).ToList().Contains(r.Value)))
            {
                return BadRequest("I can't let you go that far!");
            }

            return Ok(result);
        }

        [HttpPost]
        public IActionResult GetApplyToken([FromForm(Name = "g-recaptcha-response")]string gReCaptchaResponse)
        {
            if (string.IsNullOrWhiteSpace(gReCaptchaResponse))
            {
                return BadRequest();
            }

            var isHuman = _reCaptchaClient.IsHuman(gReCaptchaResponse);

            if (!isHuman)
            {
                return BadRequest();
            }

            var newApplyToken = new ApplyToken
            {
                Value = Guid.NewGuid().ToString()
            };

            _dbContext.ApplyTokens.Add(newApplyToken);
            _dbContext.SaveChanges();

            return Ok(newApplyToken.Value);
        }

        [HttpGet]
        public IActionResult CheckInterviews([FromQuery]string secret)
        {
            if (string.IsNullOrWhiteSpace(secret) || !_adminSettings.Value.Secret.Equals(secret))
                return BadRequest("Secret qsp missing");

            var statusFilter = new List<ApplicantStatus> {
                ApplicantStatus.FirstInterviewDateSet,
                ApplicantStatus.SecondInterviewDateSet};

            var interviewsToRemindAbout = _dbContext.Interviews
                .Include(i => i.Applicant)
                .Include(i => i.InterviewDate)
                .Where(i =>
                    i.InterviewDate.Date.Date == DateTime.Now.Date &&
                    !i.Applicant.IsReminderSent &&
                    statusFilter.Contains(i.Applicant.Status))
                .ToList();

            foreach (var interview in interviewsToRemindAbout)
            {
                _laninaInterviewManager.SendInterviewReminderMail(interview.Applicant.Email, interview.Applicant.Name, interview.InterviewDate.Date, _adminSettings.Value.AdminEmail);
                interview.Applicant.IsReminderSent = true;
            }

            _dbContext.SaveChanges();

            //Delete KoPlanet reservation if not approved
            //upperDateLimit = DateTime.Now.AddHours(2);

            //var interviewsToDeleteMettingRoomReservation = _dbContext.Interviews
            //    .Where(i =>
            //        i.Date.Date >= lowerDateLimit &&
            //        i.Date.Date <= upperDateLimit &&
            //        i.KoplanetReservationId.HasValue &&
            //        !i.IsApprovedForToday)
            //    .ToList();

            //foreach (var interview in interviewsToDeleteMettingRoomReservation)
            //{
            //    _laninaInterviewManager.DeleteMeetingRoomReservation(interview.KoplanetReservationId.Value);
            //    interview.KoplanetReservationId = null;
            //    interview.MeetingRoomName = null;
            //}

            //_dbContext.SaveChanges();

            return Ok(interviewsToRemindAbout);
        }
    }
}
