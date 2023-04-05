using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPhotoGallery.Data;
using WebPhotoGallery.Helper;
using WebPhotoGallery.Models;

namespace WebPhotoGallery.Controllers
{
    public class FAQsController : Controller
    {
        private readonly AppDbContext _dbContext;
        public FAQsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [AllowAnonymous]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet("PostFeedback")]
        public IActionResult PostFeedback()
        {
            return View();
        }
        [Authorize]
        [HttpPost("PostFeedback")]
        public async Task<IActionResult> PostFeedback(FAQsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var question = new FAQS()
            {
                Question = model.Question,
            };
            await _dbContext.FAQs.AddAsync(question);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "FAQs");
        }


        [Authorize]
        [HttpGet("PostAnswerFeedback")]
        public IActionResult PostAnswerFeedback(int id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "Question id is Requied";
                return View();
            }
            var question = _dbContext.FAQs.FirstOrDefault(x => x.Id == id);
            if (question == null)
            {
                ViewBag.ErrorMessage = "Question is not found";
                return View();
            }
            FAQsAnswerViewModel model = new FAQsAnswerViewModel()
            {
                Answer = question.Answer,
                QuestionID = question.Id,
            };
            return View(model);
        }
        [Authorize(Roles = Constant.Admin)]
        [HttpPost("PostAnswerFeedback")]
        public async Task<IActionResult> PostAnswerFeedback(FAQsAnswerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.QuestionID == null)
            {
                ViewBag.ErrorMessage = "Answer id is Requied";
                return View(model);
            }
            var question = _dbContext.FAQs.FirstOrDefault(x => x.Id == model.QuestionID);
            if (model.QuestionID == null)
            {
                ViewBag.ErrorMessage = "Question is not found";
                return View(model);
            }
            question.Answer = model.Answer;
            _dbContext.FAQs.Update(question);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Feedback", "FAQs");
        }

        [Authorize]
        [HttpGet("Feedback")]
        public async Task<IActionResult> Feedback()
        {
            var feedbacks = await _dbContext.FAQs.ToListAsync();
            return View(feedbacks);
        }

        //[Authorize(Roles = Constant.Admin)]
        [HttpPost("DeleteQuestion")]
        public async Task<IActionResult> DeleteQuestion(int? id)
        {
            if (id == null)
            {
                ViewBag.NotFound = "Question Not Found";
                return RedirectToAction("Feedback", "FAQs");
            }
            var feedbacks = await _dbContext.FAQs.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (feedbacks == null)
            {
                ViewBag.NotFound = "Question Not Found";
                return RedirectToAction("Feedback", "FAQs");

            }
            else
            {
                _dbContext.FAQs.Remove(feedbacks);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Feedback", "FAQs");
        }
    }
}
