using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApplicationCoreLogin.Models;
using WebApplicationCoreLogin.Models.ViewModel;

namespace WebApplicationCoreLogin.Controllers
{
    public class UserController : Controller
    {
        private DatabaseContext db;
        private IMapper _mapper;

        public UserController(DatabaseContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }
        public IActionResult Index()
        {//Bir listedeki bilgileri başka bir listeye atma :
            //(önce databaseden userliste tüm bilgiler çekilir sonra boş bir users oluşturulup seçilen bilgiler oraya aktarılır.Bunları yamak yerine Automapper.Extensions.Microsoft.DependenInjection indirip Automapper yapabiliriz.
            //List<User> userlist=db.Users.ToList();
            //List<UserViewModel> users = new List<UserViewModel>();
            //foreach (User user in userlist)
            //{
            //    users.Add(new UserViewModel
            //    {
            //        Id = user.Id,
            //        Name = user.Name,
            //        UserName = user.UserName,
            //    });
            //}

            List<User> userlist=db.Users.ToList();
            List<UserViewModel> model= userlist.Select(x=>_mapper.Map<UserViewModel>(x)).ToList();//bu satır sadace foreachın yaptığı iş oluyor
            //Bütün fieldların aynı olması gerekiyor name ediysen değiştirip ad yapamazsın!!!

          
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Create(UserViewModel model)
        {
            return View();
        }
    }
}
