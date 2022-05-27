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
using QuizApi.Models;

namespace QuizApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    
    public UserController(IConfiguration configuration,IWebHostEnvironment env)
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
    
    // GET: api/User
    [HttpGet]
    public JsonResult Get()
    {
        string query = @"
                        SELECT userId,name,email,phone,password,IsAdmin FROM user
            ";
        
        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                var myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }

        return new JsonResult(table);
    }

    // GET: api/User/5
    [HttpGet("{id}", Name = "Get")]
    public JsonResult Get(int id)
    {
        Console.WriteLine(id);
        string query = @"
                        SELECT userId,name,email,phone,password,IsAdmin
                        FROM user WHERE @id = userId;
            ";
        
        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@id", id);
                
                var myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }

        return new JsonResult(table);
    }

    // POST: api/User
    [HttpPost]
    public JsonResult Post()
    {
        var req = Request.Form;
        
        string query = @"
                        INSERT INTO user 
                        (name,email,phone,password) 
                        values
                         (@name,@email,@phone,@password);
            ";
        
        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@name", req["name"]);
                myCommand.Parameters.AddWithValue("@email", req["email"]);
                myCommand.Parameters.AddWithValue("@phone", req["phone"]);
                myCommand.Parameters.AddWithValue("@password", HashPassword(req["password"]));
                
                var myReader = myCommand.ExecuteReader();

                myReader.Close();
            }
            
            query = @"SELECT * FROM user ORDER BY userId DESC LIMIT 1;";
            
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                var myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
            }
            mycon.Close();
        }
        
        return new JsonResult(table);
    }

    // PUT: api/User/5
    [HttpPut("{id}")]
    public JsonResult Put(int id)
    {
        var req = Request.Form;
        
        string query = @"
                        UPDATE user SET 
                        name =@name,
                        email =@Department,
                        phone =@phone,
                        password =@password
                        WHERE userId=@id;
            ";

        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@name", req["name"]);
                myCommand.Parameters.AddWithValue("@email", req["email"]);
                myCommand.Parameters.AddWithValue("@phone", req["phone"]);
                myCommand.Parameters.AddWithValue("@password", req["password"]);
                myCommand.Parameters.AddWithValue("@IsAdmin", req["IsAdmin"]);

                var myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }

        return new JsonResult("Updated Successfully");
    }

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    public JsonResult Delete(int id)
    {
        string query = @"
                        DELETE FROM user 
                        WHERE userId=@id;
            ";

        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@id", id);

                var myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }

        return new JsonResult("Deleted Successfully");
    }
}