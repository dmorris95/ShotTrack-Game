using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShotTrackService.Controllers;

namespace ShotTrackService.Controllers
{
    //api/user/"method name" 

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SQLAccess _sQLAccess;

        public UserController()
        {
            _sQLAccess = new SQLAccess();
        }
        // [Route("~/api/User/Login")]
        [HttpPost]
        public async Task<ActionResult<int>> Login([FromBody]UserProp user)
        {
            var userScore = await _sQLAccess.UserLoginCommand(user.UsernameProp, user.PasswordProp);
            return userScore;
        }

        //[Route("~/api/User/Create")]
        [HttpPost]
        public async Task<ActionResult<string>> Create([FromBody] UserProp user)
        {
            var createSucc = await _sQLAccess.NewUserCommand(user.UsernameProp, user.PasswordProp);
            return createSucc;
        }
              
        //[Route("~/api/Verify")]
        [HttpPost]
        public async Task<ActionResult<string>> Verify([FromBody] UserProp user)
        {
            var userIdCheck = await _sQLAccess.VerifyUsernameCommand(user.UsernameProp);
            return userIdCheck;

        }

        //[Route("~/api/ScoreUpdate")]
        [HttpPost]
        public async Task<string> ScoreUpdate([FromBody] UserProp user)
        {
            var scoreUpdateCheck = await _sQLAccess.ScoreUpdateCommand(user.UsernameProp, user.HighscoreProp);
            return scoreUpdateCheck;
        }

        
        [HttpPost]
        public async Task<List<UserProp>> LeaderScores()
        {
            List<UserProp> LeadersNScores = new List<UserProp>();
            LeadersNScores = await _sQLAccess.LeaderboardCommand();
            return LeadersNScores;
        }
    }
}