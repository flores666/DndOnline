using DndOnline.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DndOnline.Controllers;

[Auth]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}