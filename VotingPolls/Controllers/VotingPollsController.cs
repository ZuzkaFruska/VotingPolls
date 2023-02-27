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
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VotingPolls.Controllers
{
    [Authorize]
    public class VotingPollsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IVotingPollRepository _votingPollRepository;
        private readonly IVoteRepository _voteRepository;
        private readonly UserManager<User> _userManager;

        public VotingPollsController(ApplicationDbContext context,
                                     IMapper mapper,
                                     IVotingPollRepository votingPollRepository,
                                     IVoteRepository voteRepository,
                                     UserManager<User> userManager)
        {
            _context = context;
            this._mapper = mapper;
            this._votingPollRepository = votingPollRepository;
            this._voteRepository = voteRepository;
            this._userManager = userManager;
        }

        // GET: VotingPolls
        public async Task<IActionResult> Index()
        {
            var model = _mapper.Map<List<VotingPollListVM>>(await _votingPollRepository.GetAllAsync());
            return View(model);
        }

        public async Task<IActionResult> MyPolls()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var model = _mapper.Map<List<VotingPollListVM>>(await _votingPollRepository.GetUserPolls(currentUser.Id));
            return View(model);
        }

        // GET: VotingPolls/Details/5
        public async Task<IActionResult> Vote(int? id)
        {
            if (id == null || _context.VotingPolls == null)
            {
                return NotFound();
            }

            var votingPoll = await _votingPollRepository.GetWithAnswersAndVotesAsync(id);

            if (votingPoll == null)
            {
                return NotFound();
            }

            var model = new VoteVM() { VotingPoll = votingPoll };
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            model.UserId = currentUser.Id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(VoteVM voteVM) 
        {
            voteVM.VotingPoll = await _votingPollRepository.GetWithAnswersAndVotesAsync(voteVM.VotingPoll.Id);
            foreach (var key in ModelState.Keys.ToList().Where(key => key.Contains("VotingPoll")))
            {
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }

            if (ModelState.IsValid)
            {
                    var votes = new List<Vote>();
                    foreach (var answer in voteVM.UserAnswers)
                    {
                        votes.Add(new Vote
                        {
                            VoterId = voteVM.UserId,
                            VotingPollId = voteVM.VotingPoll.Id,
                            AnswerId = answer,
                            DateCreated = DateTime.Now,
                            DateModified = DateTime.Now
                        });
                    }
                    await _voteRepository.AddRangeAsync(votes);

                return RedirectToAction(nameof(Results), new { votingPollId = voteVM.VotingPoll.Id });
            }

            return View(voteVM);
        }

        // GET: VotingPolls/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (TempData.IsNullOrEmpty())
            {
                var model = new VotingPollCreateVM();
                model.OwnerId = currentUser.Id;
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
                return View(model);
            } 
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
                await _votingPollRepository.AddAsync(votingPoll);
                return RedirectToAction(nameof(Index));
            }

            return View(votingPollCreateVM);
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

        public async Task<IActionResult> Results(int votingPollId)
        {
            var model = new ResultsVM();
            model.VotingPoll = await _votingPollRepository.GetWithAnswersAndVotesAsync(votingPollId);

            model.VotingPoll.Answers = model.VotingPoll.Answers.OrderByDescending(a => a.Votes.Count).ToList();

            return View(model);
            
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
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", votingPoll.OwnerId);
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
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", votingPoll.OwnerId);
            return View(votingPoll);
        }




        // GET: VotingPolls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.VotingPolls == null)
            {
                return NotFound();
            }
            var model = new ResultsVM();
            model.VotingPoll = await _votingPollRepository.GetWithAnswersAndVotesAsync(id);
            model.VotingPoll.Answers = model.VotingPoll.Answers.OrderByDescending(a => a.Votes.Count).ToList();
            return View(model);
        }

        // POST: VotingPolls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var votingPoll = _votingPollRepository.GetWithAnswersAndVotesAsync(id); // entity must be loaded to avoid delete cascade exception
            await _votingPollRepository.DeleteAsync(id);

            return RedirectToAction(nameof(MyPolls));
        }

        private bool VotingPollExists(int id)
        {
          return (_context.VotingPolls?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}



















//foreach (var key in ModelState.Keys.ToList().Where(key => key.Contains("VotingPollId")))
//{
//    ModelState[key].ValidationState = ModelValidationState.Valid;
//} // Answer can't have VotingPollId before creating Voting Poll 