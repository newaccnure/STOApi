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
        public JsonResult Login(string email, string password)
        {
            TokenResponse tp = authRepository.Login(email, password);
            JsonResult jr = Json(tp);
            if (tp.Email == null)
            {
                jr.StatusCode = 400;
            }
            else
            {
                jr.StatusCode = 200;
            }
            return jr;
        }

        [HttpPost]
        public JsonResult AddUser(string email, string password, string repeatPassword, string role, string aboutMe)
        {

            TokenResponse tp = authRepository.AddUser(email, password, repeatPassword, role, aboutMe);
            JsonResult jr = Json(tp);
            if (tp.Email == null)
            {
                jr.StatusCode = 400;
            }
            else
            {
                jr.StatusCode = 200;
            }
            return jr;
        }
    }
}