using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Application_Scheduler.Models
{
    public class UserModel
    {

        [Key]
        [Required]
        public int UserId { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }

        public string? Name { get; set; }

      
        public string? Email { get; set; }
    
        public string? MobileNumber { get; set; }

        [Required]
        public bool RememberMe { get; set; } 


        public bool VerifyUser(SqlConnection sqlConnection)
        {
            

            try
            {
                using (SqlConnection connection = sqlConnection)
                {

                    SqlCommand command= new SqlCommand($"SELECT * FROM USERS WHERE UserName = '{this.UserName}' and Password= '{this.Password}'", connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        this.UserId = (int)reader["UserId"];
                        this.UserName = (string)reader["UserName"];
                        this.Email = (string)reader["Email"];
                        this.Name = (string)reader["Name"];
                        this.MobileNumber = (string)reader["MobileNumber"];

                        Console.WriteLine("Inside Verify User "+this.UserName);

                        // Erasing the password.
                        this.Password = (string)reader["Password"];
                        return true;
                    }
                }
            }

            catch (SqlException ex)
            {
                Console.WriteLine("Verify User Model "+ex.Message);
            }
            return false;
        }

    }
}
