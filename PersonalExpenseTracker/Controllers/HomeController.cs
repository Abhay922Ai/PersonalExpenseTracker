using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PersonalExpenseTracker.Data;

namespace PersonalExpenseTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbConnection _dbConnection;

        public HomeController(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return RedirectToAction("Login", "Account");
        }

        public IActionResult TestDb()
        {
            try
            {
                using (SqlConnection connection = _dbConnection.GetConnection())
                {
                    connection.Open();

                    return Content("Database connection successful!");
                }
            }
            catch (Exception ex)
            {
                return Content("Database connection failed: " + ex.Message);
            }
        }
    }
}