﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartHomeApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

public class AddDeviceModel : PageModel
{
    
    private readonly IConfiguration _configuration;

    public AddDeviceModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    [BindProperty]
    public Device Device { get; set; }


    public string userName { get; set; }



    public IActionResult OnPost()
    {

         userName = HttpContext.Session.GetString("SelectedUserId");
  

        if (string.IsNullOrWhiteSpace(Device?.Name )||( string.IsNullOrWhiteSpace(Device?.Type)))
        {
        
            return Page();
        }
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "INSERT INTO Devices (Name, Type, Status, LoginName) VALUES (@Name, @Type, @Status, @LoginName)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                
                cmd.Parameters.AddWithValue("@Name", Device.Name);
                cmd.Parameters.AddWithValue("@Type", Device.Type);
                cmd.Parameters.AddWithValue("@Status", "uit");
                cmd.Parameters.AddWithValue("@LoginName", userName ?? "onbekend");




                cmd.ExecuteNonQuery();
            }
        }

        return RedirectToPage("Overview");
    }
}
