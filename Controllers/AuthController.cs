using Microsoft.AspNetCore.Mvc;
using STOApi.Interfaces;
using Microsoft.AspNetCore.Cors;
using STOApi.Models;

namespace STOApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class AuthController : Controller
    {
        public IAuthRepository authRepository { set; get; }

        public AuthController(IAuthRepository authRepository)
        {
            this.authRepository = authRepository;
        }

        [HttpPost]
        public JsonResult Login(string username, string password)
        {
            return Json(authRepository.Login(username, password));
        }

        [HttpPost]
        public JsonResult AddUser(string username, string password, string repeatPassword, string role, string aboutMe)
        {

            TokenResponse tp = authRepository.AddUser(username, password, repeatPassword, role, aboutMe);
            JsonResult jr = Json(tp);
            if (tp.Email == null)
            {
                jr.StatusCode = 401;
                return jr;
            }
            jr.StatusCode = 200;
            return jr;
        }

    }
}