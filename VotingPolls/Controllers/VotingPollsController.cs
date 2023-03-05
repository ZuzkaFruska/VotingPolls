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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace VotingPolls.Controllers
{
    [Authorize]
    public class VotingPollsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IVotingPollRepository _votingPollRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IVoteRepository _voteRepository;
        private readonly UserManager<User> _userManager;

        public VotingPollsController(ApplicationDbContext context,
                                     IMapper mapper,
                                     IVotingPollRepository votingPollRepository,
                                     IAnswerRepository answerRepository,
                                     IVoteRepository voteRepository,
                                     UserManager<User> userManager)
        {
            _context = context;
            this._mapper = mapper;
            this._votingPollRepository = votingPollRepository;
            this._answerRepository = answerRepository;
            this._voteRepository = voteRepository;
            this._userManager = userManager;
        }

        // GET: VotingPolls
        public async Task<IActionResult> Index()
        {
            TempData.Clear();
            var model = _mapper.Map<List<VotingPollListVM>>(await _votingPollRepository.GetAllAsync());
            _context.ChangeTracker.Clear();
            return View(model);
        }

        public async Task<IActionResult> MyPolls()
        {
            TempData.Clear();
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var model = _mapper.Map<List<VotingPollListVM>>(await _votingPollRepository.GetUserPolls(currentUser.Id));
            _context.ChangeTracker.Clear();
            return View(model);
        }

        // GET: VotingPolls/Details/5
        public async Task<IActionResult> Vote(int? votingPollId, string referer)
        {
            if (votingPollId == null || _context.VotingPolls == null)
            {
                return NotFound();
            }

            var votingPoll = await _votingPollRepository.GetWithAnswersVotesAndUserAsync(votingPollId);

            if (votingPoll == null)
            {
                return NotFound();
            }

            var model = new VoteVM() { VotingPoll = votingPoll };
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            model.VoterId = currentUser.Id;


            model.UserAlreadyVoted = votingPoll.Votes.Any(v => v.VoterId == currentUser.Id) ? true : false;

            model.Referer = (referer == null) ? Request.Headers.Referer.ToString() : referer;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(VoteVM voteVM) 
        {
            voteVM.VotingPoll = await _votingPollRepository.GetWithAnswersVotesAndUserAsync(voteVM.VotingPoll.Id);
            foreach (var key in ModelState.Keys.ToList().Where(key => key.Contains("VotingPoll")))
            {
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }
            //ModelState[nameof(voteVM.NewAnswerValue)].ValidationState = ModelValidationState.Valid;
            

            if (ModelState.IsValid)
            {
                if (voteVM.UserAlreadyVoted)
                {
                    var userOldVotes = await _voteRepository.GetUserPollVotesAsync(voteVM.VoterId, voteVM.VotingPoll.Id);

                    if (voteVM.VotingPoll.MultipleChoice) // user changed vote, the poll is multiple-choice
                    {
                        foreach (var vote in voteVM.UserAnswers)
                        {
                            if (!userOldVotes.Any(v => v.AnswerId == vote)) // if the vote is new, then add
                            {
                                var newVote = new Vote
                                {
                                    VoterId = voteVM.VoterId,
                                    VotingPollId = voteVM.VotingPoll.Id,
                                    AnswerId = vote,
                                    DateCreated = DateTime.Now,
                                    DateModified = DateTime.Now,
                                };
                                await _voteRepository.AddAsync(newVote);
                            }
                        }

                        foreach (var vote in userOldVotes)
                        {
                            if (!voteVM.UserAnswers.Any(v => v.Equals(vote.AnswerId))) // if the old vote was unticked, then delete it
                            {
                                await _voteRepository.DeleteAsync(vote.Id);
                            }
                        }

                    }
                    else // user changed vote, the poll is single-choice
                    {
                        if (userOldVotes[0].AnswerId != voteVM.UserAnswers[0])
                        {
                            userOldVotes[0].DateModified = DateTime.Now;
                            userOldVotes[0].AnswerId = voteVM.UserAnswers[0];
                            await _voteRepository.UpdateAsync(userOldVotes[0]);
                        }
                    }

                    return RedirectToAction(nameof(Results), new { votingPollId = voteVM.VotingPoll.Id, referer = voteVM.Referer});
                }
                else // this is the first time user is voting
                {
                    var votes = new List<Vote>();
                    foreach (var vote in voteVM.UserAnswers)
                    {
                        votes.Add(new Vote
                        {
                            VoterId = voteVM.VoterId,
                            VotingPollId = voteVM.VotingPoll.Id,
                            AnswerId = vote,
                            DateCreated = DateTime.Now,
                        });
                    }
                    await _voteRepository.AddRangeAsync(votes);

                    return RedirectToAction(nameof(Results), new { votingPollId = voteVM.VotingPoll.Id, referer = voteVM.Referer });
                }
            }

            return View(voteVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrRemoveAnswer_Vote(VoteVM voteVM, int? answerNo)
        {
            foreach (var key in ModelState.Keys.ToList().Where(key => key.Contains("VotingPoll")))
            {
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }
            ModelState[nameof(voteVM.UserAnswers)].ValidationState = ModelValidationState.Valid;

            if (String.IsNullOrWhiteSpace(voteVM.NewAnswerValue) || voteVM.NewAnswerValue.Length >= 500)
            {
                ModelState.AddModelError(nameof(voteVM.NewAnswerValue), "The answer must contain a minimum of 1 and a maximum of 500 characters.");
            }

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                var votingPoll = await _votingPollRepository.GetWithAnswersVotesAndUserAsync(voteVM.VotingPoll.Id);

                if (answerNo.HasValue)
                {

                }
                else
                {
                    votingPoll.Answers.Add(new Answer
                    {
                        Text = voteVM.NewAnswerValue,
                        Number = votingPoll.Answers.Count + 1,
                        VotingPollId = votingPoll.Id,
                        AuthorId = currentUser.Id,
                        DateCreated = DateTime.Now
                    });
                    await _votingPollRepository.UpdateAsync(votingPoll);
                }

                return RedirectToAction(nameof(Vote), new { votingPollId = voteVM.VotingPoll.Id, referer = voteVM.Referer });
            }

            return RedirectToAction(nameof(Vote), new { votingPollId = voteVM.VotingPoll.Id, referer = voteVM.Referer });
        }


        public async Task<IActionResult> DeleteUserPollVotes(int votingPollId, string referer, string actionName)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var userVotes = await _voteRepository.GetUserPollVotesAsync(currentUser.Id, votingPollId);
            foreach (var vote in userVotes)
            {
                await _voteRepository.DeleteAsync(vote.Id);
            }

            var routeData = RouteData.Values;

            return RedirectToAction(actionName, new { referer = referer, votingPollId = votingPollId });
        }


        // GET: VotingPolls/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (TempData.IsNullOrEmpty())
            {
                var model = new VotingPollCreateVM();
                model.OwnerId = currentUser.Id;
                //model.NotEnoughAnswers = false;

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
                var model = TempData.Get<VotingPollCreateVM>(nameof(VotingPollCreateVM)); //_mapper.Map<VotingPollCreateVM>(TempData.Get<VotingPoll>(nameof(VotingPoll)));
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
        public async Task<IActionResult> AddOrRemoveAnswer_CreateEdit(VotingPollCreateVM votingPollCreateVM, int? answerNo, string actionName, int? votingPollId)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            votingPollCreateVM.NotEnoughAnswers = false;

            if (answerNo.HasValue)
            {
                if (votingPollCreateVM.Answers.Count > 2)
                {
                    var ans = votingPollCreateVM.Answers.Find(q => q.Number == answerNo.Value);
                    votingPollCreateVM.Answers.Remove(ans);
                }
                else
                {
                    votingPollCreateVM.NotEnoughAnswers = true;
                }
            }
            else
            {
                votingPollCreateVM.Answers.Add(new Answer { Text = "" , AuthorId = currentUser.Id});
            }

            TempData.Clear();
            TempData.Put(nameof(VotingPollCreateVM), votingPollCreateVM);
            
            return RedirectToAction(actionName, new { votingPollId = votingPollId });
        }


        public async Task<IActionResult> Results(int votingPollId, string referer)
        {
            var model = new ResultsVM();
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            model.VotingPoll = await _votingPollRepository.GetWithAnswersVotesAndUserAsync(votingPollId);

            model.VotingPoll.Answers = model.VotingPoll.Answers.OrderByDescending(a => a.Votes.Count).ToList();
            model.Referer = (referer == null) ? Request.Headers.Referer.ToString() : referer;
            model.UserAlreadyVoted = model.VotingPoll.Votes.Any(v => v.VoterId == currentUser.Id) ? true : false;
            return View(model);
            
        }


        // GET: VotingPolls/Edit/5
        public async Task<IActionResult> Edit(int votingPollId)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (TempData.IsNullOrEmpty())
            {
                var votingPoll = await _votingPollRepository.GetWithAnswersVotesAndUserAsync(votingPollId);
                if (votingPoll == null)
                {
                    return NotFound();
                }

                var model = _mapper.Map<VotingPollEditVM>(votingPoll);
                model.CurrentUserId = currentUser.Id;
                return View(model);
            }
            else
            {
                var model = TempData.Get<VotingPollEditVM>(nameof(VotingPollCreateVM));  //_mapper.Map<VotingPollEditVM>(TempData.Get<VotingPoll>(nameof(VotingPoll)));
                model.CurrentUserId = currentUser.Id;
                model.Id = votingPollId;
                return View(model);
            }
        }

        // POST: VotingPolls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VotingPollEditVM votingPollEditVM)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var oldAnswers = await _answerRepository.GetVotingPollAnswers(votingPollEditVM.Id);

                    foreach (var oldAnswer in oldAnswers)
                    {
                        if (votingPollEditVM.Answers.Any(a => a.Id == oldAnswer.Id)) //does this old answer still exist?
                        {
                            foreach (var answer in votingPollEditVM.Answers)
                            {
                                if (answer.Id == oldAnswer.Id && answer.Text == oldAnswer.Text) //answer unchanged
                                {
                                    answer.DateCreated = oldAnswer.DateCreated;
                                    answer.DateModified = oldAnswer.DateModified;
                                }
                                else if (answer.Id == oldAnswer.Id && answer.Text != oldAnswer.Text) //answer was changed
                                {
                                    answer.DateCreated = oldAnswer.DateCreated;
                                    answer.DateModified = DateTime.Now;
                                }
                            }
                        }
                        else //this old answer was removed
                        {
                            await _answerRepository.DeleteAsync(oldAnswer.Id);
                        }
                        
                    }

                    var votingPoll = _mapper.Map<VotingPoll>(votingPollEditVM);
                    //votingPoll.Answers = null;
                    //votingPoll.Votes = null;
                    await _votingPollRepository.UpdateAsync(votingPoll);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _votingPollRepository.Exists(votingPollEditVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyPolls));
            }
            return RedirectToAction(nameof(MyPolls));
        }


        // GET: VotingPolls/Delete/5
        public async Task<IActionResult> Delete(int? votingPollId)
        {
            if (votingPollId == null || _context.VotingPolls == null)
            {
                return NotFound();
            }
            var model = new ResultsVM();
            model.VotingPoll = await _votingPollRepository.GetWithAnswersVotesAndUserAsync(votingPollId);
            model.VotingPoll.Answers = model.VotingPoll.Answers.OrderByDescending(a => a.Votes.Count).ToList();
            return View(model);
        }

        // POST: VotingPolls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int votingPollId)
        {
            await _votingPollRepository.DeleteAsync(votingPollId);
            return RedirectToAction(nameof(MyPolls));
        }

      
    }
}