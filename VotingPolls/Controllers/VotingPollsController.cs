using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VotingPolls.Data;
using AutoMapper;
using VotingPolls.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using VotingPolls.Contracts;
using VotingPolls.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace VotingPolls.Controllers
{
    [Authorize]
    public class VotingPollsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IVotingPollRepository _votingPollRepository;
        private readonly UserManager<User> _userManager;

        public VotingPollsController(ApplicationDbContext context,
                                     IMapper mapper,
                                     IVotingPollRepository votingPollRepository,
                                     UserManager<User> userManager)
        {
            _context = context;
            this._mapper = mapper;
            this._votingPollRepository = votingPollRepository;
            this._userManager = userManager;
        }

        // GET: VotingPolls
        public async Task<IActionResult> Index()
        {
            var model = _mapper.Map<List<VotingPollListVM>>(await _context.VotingPolls.ToListAsync()); //.Include(q=>q.User)
            return View(model);
        }

        // GET: VotingPolls/Details/5
        public async Task<IActionResult> Vote(int? id)
        {
            if (id == null || _context.VotingPolls == null)
            {
                return NotFound();
            }
            
            
            var votingPoll = await _context.VotingPolls.FirstOrDefaultAsync(q => q.Id == id);

            if (votingPoll == null)
            {
                return NotFound();
            }

            var model = new VoteVM() { VotingPoll = votingPoll};
             
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            model.UserId = currentUser.Id;
            model.VotingPoll.Answers = await _context.Answers.Where(q => q.VotingPollId == votingPoll.Id).ToListAsync(); // bez sensu ;/

            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(VoteVM voteVM) //async Task<IActionResult>
        {
            if (ModelState.IsValid)
            {

            }

            if (voteVM.VotingPoll.MultipleChoice)
            {
                var votes = new List<Vote>();
                foreach (var answer in voteVM.UserAnswers)
                {
                    votes.Add(new Vote
                    {
                        UserId = voteVM.UserId,
                        VotingPollId = voteVM.VotingPoll.Id,
                        AnswerId = answer,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now
                    });
                }
                await _context.Votes.AddRangeAsync(votes);
                _context.SaveChanges();
            }
            else
            {
                var vote = new Vote()
                {
                    UserId = voteVM.UserId,
                    VotingPollId = voteVM.VotingPoll.Id,
                    AnswerId = voteVM.UserAnswer,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                };
                await _context.Votes.AddAsync(vote);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: VotingPolls/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (TempData.IsNullOrEmpty())
            {
                var model = new VotingPollCreateVM();
                model.UserId = currentUser.Id;
                model.Answers = new List<Answer>();
                model.Answers.AddRange(new List<Answer>()

                {
                    new Answer {Text=""},
                    new Answer {Text=""}
                });
                return View(model);
            }
            else
            {
                var model = _mapper.Map<VotingPollCreateVM>(TempData.Get<VotingPoll>(nameof(VotingPoll)));
                //model.UserId = currentUser.Id;
                return View(model);
            } // model.UserId przypisać do odpowiedzi, spr. gdzie 
        }


        [HttpPost]
        public IActionResult AddRemoveAnswerAsync(VotingPollCreateVM votingPollCreateVM, int? answerNo)
        {
            var votingPoll = _mapper.Map<VotingPoll>(votingPollCreateVM);
            
            if (answerNo.HasValue)
            {
                if (votingPoll.Answers.Count > 2)
                {
                    var ans = votingPoll.Answers.Find(q => q.Number == answerNo.Value);
                    votingPoll.Answers.Remove(ans);
                }
                //else warning can't have < 2 answers
            }
            else
            {
                votingPoll.Answers.Add(new Answer { Text = "" });
            }

            TempData.Clear();
            TempData.Put(nameof(VotingPoll), votingPoll);
            
            return RedirectToAction(nameof(Create));
        }


        // POST: VotingPolls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VotingPollCreateVM votingPollCreateVM)
        {
            if (ModelState.IsValid)
            {
                var votingPoll = _mapper.Map<VotingPoll>(votingPollCreateVM);
                votingPoll.DateCreated = DateTime.Now;
                votingPoll.DateModified = DateTime.Now;
                _context.Add(votingPoll);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(votingPollCreateVM);
        }




        // GET: VotingPolls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.VotingPolls == null)
            {
                return NotFound();
            }

            var votingPoll = await _context.VotingPolls.FindAsync(id);
            if (votingPoll == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", votingPoll.UserId);
            return View(votingPoll);
        }

        

        // POST: VotingPolls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Question,MultipleChoice,AdditionalAnswers,OwnerId,Id,DateCreated,DateModified")] VotingPoll votingPoll)
        {
            if (id != votingPoll.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(votingPoll);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VotingPollExists(votingPoll.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", votingPoll.UserId);
            return View(votingPoll);
        }

        // GET: VotingPolls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.VotingPolls == null)
            {
                return NotFound();
            }

            var votingPoll = await _context.VotingPolls
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (votingPoll == null)
            {
                return NotFound();
            }

            return View(votingPoll);
        }

        // POST: VotingPolls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.VotingPolls == null)
            {
                return Problem("Entity set 'ApplicationDbContext.VotingPolls'  is null.");
            }
            var votingPoll = await _context.VotingPolls.FindAsync(id);
            if (votingPoll != null)
            {
                _context.VotingPolls.Remove(votingPoll);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VotingPollExists(int id)
        {
          return (_context.VotingPolls?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}






//var dictModel = new RouteValueDictionary(votingPollCreateVM);
//var dictAnswers = new RouteValueDictionary();

//for (int i = 0; i < votingPollCreateVM.Answers.Count; i++)
//{
//    dictAnswers.Add($"Answers{i}.Text", votingPollCreateVM.Answers[i].Text);
//}

//return RedirectToAction(nameof(Create), new { votingPollCreateVM = dictModel/*, answers = dictAnswers*/});