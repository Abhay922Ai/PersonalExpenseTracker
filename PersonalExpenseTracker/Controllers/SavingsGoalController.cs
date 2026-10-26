using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Models;
using PersonalExpenseTracker.ViewModels;

namespace PersonalExpenseTracker.Controllers
{
    public class SavingsGoalController : Controller
    {
        private readonly DbConnection _dbConnection;

        public SavingsGoalController(DbConnection dbConnection)
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

            List<SavingsGoal> goals = new List<SavingsGoal>();

            using SqlConnection connection = _dbConnection.GetConnection();
            connection.Open();

            string query = @"
                SELECT GoalId, UserId, GoalName, TargetAmount,
                       SavedAmount, TargetDate, CreatedAt
                FROM SavingsGoals
                WHERE UserId = @UserId
                ORDER BY CreatedAt DESC";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId.Value);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                goals.Add(new SavingsGoal
                {
                    GoalId = Convert.ToInt32(reader["GoalId"]),
                    UserId = Convert.ToInt32(reader["UserId"]),
                    GoalName = reader["GoalName"].ToString() ?? "",
                    TargetAmount = Convert.ToDecimal(reader["TargetAmount"]),
                    SavedAmount = Convert.ToDecimal(reader["SavedAmount"]),
                    TargetDate = reader["TargetDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["TargetDate"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return View(goals);
        }

        [HttpGet]
        public IActionResult Add()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new SavingsGoalViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(SavingsGoalViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (model.SavedAmount > model.TargetAmount)
            {
                ModelState.AddModelError(
                    "SavedAmount",
                    "Saved amount cannot be greater than target amount.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using SqlConnection connection = _dbConnection.GetConnection();
            connection.Open();

            string query = @"
                INSERT INTO SavingsGoals
                (UserId, GoalName, TargetAmount, SavedAmount, TargetDate)
                VALUES
                (@UserId, @GoalName, @TargetAmount,
                 @SavedAmount, @TargetDate)";

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", userId.Value);
            command.Parameters.AddWithValue("@GoalName", model.GoalName);
            command.Parameters.AddWithValue("@TargetAmount", model.TargetAmount);
            command.Parameters.AddWithValue("@SavedAmount", model.SavedAmount);

            command.Parameters.AddWithValue(
                "@TargetDate",
                (object?)model.TargetDate ?? DBNull.Value);

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

            string query = "DELETE FROM SavingsGoals WHERE GoalId = @GoalId AND UserId = @UserId";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@GoalId", id);
            command.Parameters.AddWithValue("@UserId", userId.Value);

            command.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProgress(int id, decimal amount)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (amount <= 0)
            {
                return RedirectToAction("Index");
            }

            using SqlConnection connection = _dbConnection.GetConnection();
            connection.Open();

            string query = @"
                UPDATE SavingsGoals
                SET SavedAmount = CASE 
                    WHEN SavedAmount + @Amount > TargetAmount THEN TargetAmount
                    ELSE SavedAmount + @Amount
                END
                WHERE GoalId = @GoalId AND UserId = @UserId";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@GoalId", id);
            command.Parameters.AddWithValue("@UserId", userId.Value);

            command.ExecuteNonQuery();

            return RedirectToAction("Index");
        }
    }
}
