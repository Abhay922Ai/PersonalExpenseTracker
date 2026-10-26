using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Models;
using PersonalExpenseTracker.ViewModels;

namespace PersonalExpenseTracker.Controllers
{
    public class IncomeController : Controller
    {
        private readonly DbConnection _dbConnection;

        public IncomeController(DbConnection dbConnection)
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

            List<Income> incomes = new List<Income>();

            using SqlConnection connection = _dbConnection.GetConnection();
            connection.Open();

            string query = @"
                SELECT IncomeId, UserId, Source, Amount, IncomeDate, Description
                FROM Income
                WHERE UserId = @UserId
                ORDER BY IncomeDate DESC, IncomeId DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId.Value);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                incomes.Add(new Income
                {
                    IncomeId = Convert.ToInt32(reader["IncomeId"]),
                    UserId = Convert.ToInt32(reader["UserId"]),
                    Source = reader["Source"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    IncomeDate = Convert.ToDateTime(reader["IncomeDate"]),
                    Description = reader["Description"] == DBNull.Value
                        ? null
                        : reader["Description"].ToString()
                });
            }

            return View(incomes);
        }

        [HttpGet]
        public IActionResult Add()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new IncomeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(IncomeViewModel model)
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
                INSERT INTO Income
                (UserId, Source, Amount, IncomeDate, Description)
                VALUES
                (@UserId, @Source, @Amount, @IncomeDate, @Description)";

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", userId.Value);
            command.Parameters.AddWithValue("@Source", model.Source);
            command.Parameters.AddWithValue("@Amount", model.Amount);
            command.Parameters.AddWithValue("@IncomeDate", model.IncomeDate);
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

            string query = "DELETE FROM Income WHERE IncomeId = @IncomeId AND UserId = @UserId";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IncomeId", id);
            command.Parameters.AddWithValue("@UserId", userId.Value);

            command.ExecuteNonQuery();

            return RedirectToAction("Index");
        }
    }
}
