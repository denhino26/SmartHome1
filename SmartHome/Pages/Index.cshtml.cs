using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using SmartHomeApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartHomeApp.Pages
{


    public class SelectUserModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public List<User> Users { get; set; } = new List<User>();



        [BindProperty]
        [Required(ErrorMessage = "Selecteer een gebruiker.")]
        public string Name { get; set; }


        [BindProperty]
        public string NewUserName { get; set; }



        public SelectUserModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
             
        }



        public void OnGet()
        {
            LoadUsers();
        }


        public IActionResult OnPost()
        {
            LoadUsers();


            if (!string.IsNullOrWhiteSpace(Name))
            {
                var selectedUser = Users.FirstOrDefault(u => u.Name == Name);
                if (selectedUser != null)
                {
                    // Correct: Sla de gebruikersnaam op in de sessie
                    HttpContext.Session.SetString("SelectedUserId", selectedUser.Name);
                    return RedirectToPage("/Overview");
                }
            }


            ModelState.AddModelError("", "Selecteer eerst een geldige gebruikers.");
            return Page();
        }



        public IActionResult OnPostAddUser()
        {
            if (!string.IsNullOrWhiteSpace(NewUserName))
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("INSERT INTO Users (Name) VALUES (@name)", conn);
                    cmd.Parameters.AddWithValue("@name", NewUserName);
                    cmd.ExecuteNonQuery();
                }
                LoadUsers();
            }

            return RedirectToPage();
        }

        private void LoadUsers()
        {
            Users.Clear();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var command = new SqlCommand("SELECT UserId,Name FROM Users", conn);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Users.Add(new User
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        Name = reader["Name"].ToString()
                    });
                }
            }
        }
    }
}

