using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartHomeApp.Models;

namespace SmartHome.Pages
{
    public class OverviewHistoryModel : PageModel
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public List<SmartHomeApp.Models.Device> Devices { get; set; } = new();

        [BindProperty]
        public int SelectedDeviceId { get; set; }

        public void OnGet()
        {
            LoadDevicesWithTime();

        }
        public IActionResult OnPost()
        {
            if (SelectedDeviceId > 0)
            {
                DeleteDevice(SelectedDeviceId);
            }
            return RedirectToPage();
        }



        public OverviewHistoryModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private void LoadDevicesWithTime()
        {
            Devices.Clear();

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT DeviceId, Name, Type, Status, LoginName, CreatedAt FROM Devices";
            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Devices.Add(new Device
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Type = reader.GetString(2),
                    Status = reader.GetString(3),
                    userName = reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5)

                });
            }
        }


        private void DeleteDevice(int deviceId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string deleteQuery = "DELETE FROM Devices WHERE DeviceId = @Id";

            using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
            {
                cmd.Parameters.AddWithValue("@Id", deviceId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
