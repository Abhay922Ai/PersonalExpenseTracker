using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Models;
using PersonalExpenseTracker.ViewModels;

namespace PersonalExpenseTracker.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly DbConnection _dbConnection;

        public ExpenseController(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<Expense> expenses = new List<Expense>();

            using SqlConnection connection = _dbConnection.GetConnection();
            connection.Open();

            string query = @"
                SELECT ExpenseId, UserId, Category, Amount, ExpenseDate, Description
                FROM Expenses
                WHERE UserId = @UserId
                ORDER BY ExpenseDate DESC, ExpenseId DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId.Value);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                expenses.Add(new Expense
                {
                    ExpenseId = Convert.ToInt32(reader["ExpenseId"]),
                    UserId = Convert.ToInt32(reader["UserId"]),
                    Category = reader["Category"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    ExpenseDate = Convert.ToDateTime(reader["ExpenseDate"]),
                    Description = reader["Description"] == DBNull.Value
                        ? null
                        : reader["Description"].ToString()
                });
            }

            return View(expenses);
        }

        [HttpGet]
        public IActionResult Add()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new ExpenseViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(ExpenseViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using SqlConnection connection = _dbConnection.GetConnection();
            connection.Open();

            string query = @"
                INSERT INTO Expenses
                (UserId, Category, Amount, ExpenseDate, Description)
                VALUES
                (@UserId, @Category, @Amount, @ExpenseDate, @Description)";

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", userId.Value);
            command.Parameters.AddWithValue("@Category", model.Category);
            command.Parameters.AddWithValue("@Amount", model.Amount);
            command.Parameters.AddWithValue("@ExpenseDate", model.ExpenseDate);
            command.Parameters.AddWithValue("@Description",
                (object?)model.Description ?? DBNull.Value);

            command.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            using SqlConnection connection = _dbConnection.GetConnection();
            connection.Open();

            string query = "DELETE FROM Expenses WHERE ExpenseId = @ExpenseId AND UserId = @UserId";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ExpenseId", id);
            command.Parameters.AddWithValue("@UserId", userId.Value);

            command.ExecuteNonQuery();

            return RedirectToAction("Index");
        }
    }
}
