using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.ViewModels;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;

namespace PersonalExpenseTracker.ViewModels
{
   
}


namespace PersonalExpenseTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbConnection _dbConnection;


        public AccountController(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using SqlConnection connection = _dbConnection.GetConnection();
                connection.Open();

                string checkQuery =
                    "SELECT COUNT(*) FROM Users WHERE Email = @Email";

                using SqlCommand checkCommand =
                    new SqlCommand(checkQuery, connection);

                checkCommand.Parameters.AddWithValue("@Email", model.Email);

                int existingUser =
                    (int)checkCommand.ExecuteScalar();

                if (existingUser > 0)
                {
                    ModelState.AddModelError(
                        "Email",
                        "An account with this email already exists.");

                    return View(model);
                }

                string passwordHash = HashPassword(model.Password);

                string insertQuery = @"
                    INSERT INTO Users
                    (FullName, Email, Password)
                    VALUES
                    (@FullName, @Email, @Password)";

                using SqlCommand insertCommand =
                    new SqlCommand(insertQuery, connection);

                insertCommand.Parameters.AddWithValue(
                    "@FullName", model.FullName);

                insertCommand.Parameters.AddWithValue(
                    "@Email", model.Email);

                insertCommand.Parameters.AddWithValue(
                    "@Password", passwordHash);

                insertCommand.ExecuteNonQuery();

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Registration failed: " + ex.Message);

                return View(model);
            }
        }


        private string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32);

            return Convert.ToBase64String(salt)
                   + "."
                   + Convert.ToBase64String(hash);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using SqlConnection connection = _dbConnection.GetConnection();
                connection.Open();

                string query = @"
            SELECT UserId, FullName, Password
            FROM Users
            WHERE Email = @Email";

                using SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Email", model.Email);

                using SqlDataReader reader = command.ExecuteReader();

                if (!reader.Read())
                {
                    ModelState.AddModelError(
                        "",
                        "Invalid email or password.");

                    return View(model);
                }

                int userId = reader.GetInt32(0);
                string fullName = reader.GetString(1);
                string storedPassword = reader.GetString(2);

                if (!VerifyPassword(model.Password, storedPassword))
                {
                    ModelState.AddModelError(
                        "",
                        "Invalid email or password.");

                    return View(model);
                }

                HttpContext.Session.SetInt32("UserId", userId);
                HttpContext.Session.SetString("FullName", fullName);

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "Login failed: " + ex.Message);

                return View(model);
            }
        }
        private bool VerifyPassword(string password, string storedPassword)
        {
            string[] parts = storedPassword.Split('.');

            if (parts.Length != 2)
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHash = Convert.FromBase64String(parts[1]);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32);

            return CryptographicOperations.FixedTimeEquals(
                hash,
                storedHash);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }

    }

}