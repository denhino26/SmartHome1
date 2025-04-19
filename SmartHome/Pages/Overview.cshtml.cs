using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartHomeApp.Models;


public class IndexModel : PageModel
{

    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public List<SmartHomeApp.Models.Device> Devices { get; set; } = new();
    public string UserName { get; set; }


    public IndexModel(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }


    public void OnGet()
    {
        LoadDevices();

        var user = new SmartHomeApp.Models.User();
        user.SelectedUserName = "SelectedUser";

        UserName = HttpContext.Session.GetString("SelectedUserId");

    }



    public IActionResult OnPost()
    {
        int deviceId = int.Parse(Request.Form["deviceId"]);
        string action = Request.Form["action"];



        if (action == "toggle")
        {
            ToggleDeviceStatus(deviceId);
        }

        return RedirectToPage();
    }

    private void LoadDevices()
    {
        Devices.Clear();

        using SqlConnection conn = new SqlConnection(_connectionString);
        conn.Open();

        string query = "SELECT DeviceId, Name, Type, Status FROM Devices";
        using SqlCommand cmd = new SqlCommand(query, conn);
        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Devices.Add(new Device
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Type = reader.GetString(2),
                Status = reader.GetString(3)
            });
        }
    }

    private void ToggleDeviceStatus(int deviceId)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        conn.Open();

        string getStatusQuery = "SELECT Status FROM Devices WHERE DeviceId = @Id";
        string newStatus = "uit";

        using (SqlCommand getCmd = new SqlCommand(getStatusQuery, conn))
        {
            getCmd.Parameters.AddWithValue("@Id", deviceId);
            var currentStatus = (string)getCmd.ExecuteScalar();
            newStatus = currentStatus == "aan" ? "uit" : "aan";
        }

        string updateQuery = "UPDATE Devices SET Status = @Status WHERE DeviceId = @Id";
        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
        {
            updateCmd.Parameters.AddWithValue("@Status", newStatus);
            updateCmd.Parameters.AddWithValue("@Id", deviceId);
            updateCmd.ExecuteNonQuery();
        }
    }




   
    


}
