using BettingAgency.Application.Models;
using BettingAgency.Application.Models.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BettingAgency.Controllers {
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController: ControllerBase {
        private readonly JwtSettings _jwtSettings;
        public AccountController(JwtSettings jwtSettings) {
            this._jwtSettings = jwtSettings;
        }
        private IEnumerable < Users > logins = new List < Users > () {
            new() {
                    Id = Guid.NewGuid(),
                        EmailId = "adminakp@gmail.com",
                        UserName = "Admin",
                        Password = "Admin",
                },
                new() {
                    Id = Guid.NewGuid(),
                        EmailId = "adminakp@gmail.com",
                        UserName = "User1",
                        Password = "Admin",
                }
        };
        [HttpPost]
        public IActionResult GetToken(UserLogins userLogins) {
            try {
                var Token = new UserTokens();
                var Valid = logins.Any(x => x.UserName.Equals(userLogins.UserName, StringComparison.OrdinalIgnoreCase));
                if (Valid) {
                    var user = logins.FirstOrDefault(x => x.UserName.Equals(userLogins.UserName, StringComparison.OrdinalIgnoreCase));
                    Token = JwtHelpers.GenTokenkey(new UserTokens() {
                        EmailId = user.EmailId,
                            GuidId = Guid.NewGuid(),
                            UserName = user.UserName,
                            Id = user.Id,
                    }, _jwtSettings);
                } else {
                    return BadRequest("wrong password");
                }
                return Ok(Token);
            } catch (Exception ex) {
                throw;
            }
        }
        /// <summary>
        /// Get List of UserAccounts
        /// </summary>
        /// <returns>List Of UserAccounts</returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetList() {
            return Ok(logins);
        }
    }
}