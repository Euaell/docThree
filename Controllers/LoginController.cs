using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
    
        public LoginController(IConfiguration configuration,IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
    
        private static string HashPassword(string password)
        {
            byte[] salt = Encoding.ASCII.GetBytes("Encrypting Salt for Hashing!");
            // Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));

            return hashed;
        }
        
        
        // api/login
        [HttpPost]
        public JsonResult Get()
        {
            var req = Request.Form;

            string email = req["email"];
            string pass = HashPassword(req["password"]);

            string query = @"
                        SELECT userId,name,email,phone,password,IsAdmin FROM user WHERE @email = user.email
            ";
        
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@email", email);
                    var myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }

            if (table.Rows.Count == 0)
                return new JsonResult("Invalid email or password");
            
            return (string) table.Rows[0]["password"] != pass ? new JsonResult("Incorrect Password") : new JsonResult(table);
        }
    }
}
