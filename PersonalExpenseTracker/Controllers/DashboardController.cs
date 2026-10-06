using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.ViewModels;

namespace PersonalExpenseTracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DbConnection _dbConnection;

        public DashboardController(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? fullName = HttpContext.Session.GetString("FullName");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            DashboardViewModel dashboard = new DashboardViewModel();

            dashboard.FullName = fullName ?? "User";

            using SqlConnection connection = _dbConnection.GetConnection();

            connection.Open();

            string query = @"
                SELECT
                    (SELECT ISNULL(SUM(Amount), 0)
                     FROM Income
                     WHERE UserId = @UserId) AS TotalIncome,

                    (SELECT ISNULL(SUM(Amount), 0)
                     FROM Expenses
                     WHERE UserId = @UserId) AS TotalExpenses,

                    (SELECT ISNULL(SUM(SavedAmount), 0)
                     FROM SavingsGoals
                     WHERE UserId = @UserId) AS TotalSavings,

                    (SELECT ISNULL(SUM(TargetAmount), 0)
                     FROM SavingsGoals
                     WHERE UserId = @UserId) AS SavingsTarget,

                    (SELECT TOP 1 GoalName
                     FROM SavingsGoals
                     WHERE UserId = @UserId
                     ORDER BY GoalId DESC) AS SavingsGoalName;
            ";

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", userId.Value);

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                dashboard.TotalIncome = reader.GetDecimal(0);
                dashboard.TotalExpenses = reader.GetDecimal(1);
                dashboard.TotalSavings = reader.GetDecimal(2);
                dashboard.SavingsTarget = reader.GetDecimal(3);

                if (!reader.IsDBNull(4))
                {
                    dashboard.SavingsGoalName =
                        reader.GetString(4);
                }
            }

            dashboard.Balance =
                dashboard.TotalIncome - dashboard.TotalExpenses;

            return View(dashboard);
        }
    }
}