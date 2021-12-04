using GithubSearchApi.Models;
using GithubSearchApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GithubSearchApi.Middleware;
using System.Text.Json;


namespace GithubSearchApi.Controllers
{
 //   [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GithubSearchService _githubSearchService;
        private readonly AuthenticationService _authenticationService;
        private const string SESSION_KEY_USER = "CurrentUser";
        private const string SESSION_KEY_REPO = "RepoBuokmarked";

        public HomeController(ILogger<HomeController> logger, GithubSearchService githubSearchService, AuthenticationService authenticationService)
        {
            _logger = logger;
            _githubSearchService = githubSearchService;
            _authenticationService = authenticationService;
        }

        public IActionResult Index()
        {
            string res = "Welcome to GitHubSearch";
            return Ok(res);
        }

        [HttpGet]
        public ActionResult GetRepos(string q)
        {
            string res = _githubSearchService.GetRepos(q);
            return Ok(res);
        }

        #region Autentification & Autorisation
        [HttpPost]
        public ActionResult Login([FromBody] User user)
        {
            string res = string.Empty;
            string token = string.Empty;
            int errorCode = -1;
            User currentUser = null;
            try
            {
                currentUser = _authenticationService.GetUserIdentity(user);
                if (currentUser != null)
                {
                    errorCode = 0;
                    token = _authenticationService.GenerateSecurityToken(currentUser);
                    //set empty collection bookmarked repoes
                    List<Repo> repoList = new List<Repo>();
                    //store user and  list to user session

                    HttpContext.Session.Set<User>(SESSION_KEY_USER, currentUser);
                    HttpContext.Session.Set<List<Repo>>(SESSION_KEY_REPO, repoList);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                var objects = new { Login = user, Token = token, ErrorCode = errorCode };
                res = JsonSerializer.Serialize(objects);
            }

            return Ok(res);
        }
        #endregion Autentification & Autorisation

        #region BookMarked

        [HttpPost]
        public ActionResult AddRepoToBookmarked([FromBody] Repo repo)
        {
            string res = string.Empty;
            List<Repo> repoList = null;
            if (HttpContext.Session.Keys.Contains(SESSION_KEY_REPO))
            {
                repoList = HttpContext.Session.Get<List<Repo>>(SESSION_KEY_REPO);
            }
            else
            {
                repoList = new List<Repo>();
            }
            repo.bookmarked = true;
            if (!repoList.Contains(repo))
            {
                repoList.Add(repo);
            }
            HttpContext.Session.Set<List<Repo>>(SESSION_KEY_REPO, repoList);
            return Ok(res);
        }
        [HttpPost]
        public ActionResult RemoveRepoFromBookmarked(Repo repo)
        {
            string res = string.Empty;
            List<Repo> repoList = null;
            if (HttpContext.Session.Keys.Contains(SESSION_KEY_REPO))
            {
                repoList = HttpContext.Session.Get<List<Repo>>(SESSION_KEY_REPO);
            }
            else
            {
                repoList = new List<Repo>();
            }                
            if (repoList.Contains(repo))
            {
                repoList.Remove(repo);
            }
            HttpContext.Session.Set<List<Repo>>(SESSION_KEY_REPO, repoList);
            return Ok(res);
        }

        #endregion BookMarked
    }
}
