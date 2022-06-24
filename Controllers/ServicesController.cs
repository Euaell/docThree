using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace QuizApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServicesController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    
    public ServicesController(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }
    
    // GET: api/Services
    [HttpGet]
    public JsonResult Get()
    {
        string query = @"
                            SELECT idservices, u.phone, u.email, u.name,`desc`,fileName,quantity
                            FROM services
                            INNER JOIN `user` u on `services`.iduser = u.userId
               ";

        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
        {
            myCon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
            {
                var reader = myCommand.ExecuteReader();
                table.Load(reader);
                    
                reader.Close();
                myCon.Close();
            }    
        }
            
        return new JsonResult(table);
    }

    // GET: api/Services/5
    [HttpGet("{id}")]
    public JsonResult GetId(int id)
    {
        return new JsonResult("value");
    }

    // POST: api/Services
    [HttpPost]
    public JsonResult Post()
    {
        var req = Request.Form;
        string file;
        try
        {
            var postedFile = req.Files[0];
            string fileName = postedFile.FileName;
            string filePath = _env.ContentRootPath + "/Images/" + fileName;

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
            }

            file = fileName;
        }
        catch (Exception)
        {
            file = "";
        }

        string query = @"
                            INSERT INTO services (`fileName`,`desc`,quantity, iduser) 
                            VALUES (@fileName,@desc,@quantity,@userId);
                            ";
        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
        {
            myCon.Open();
            using (MySqlCommand command = new MySqlCommand(query, myCon))
            {
                command.Parameters.AddWithValue("@fileName", file);
                command.Parameters.AddWithValue("@desc", req["desc"]);
                command.Parameters.AddWithValue("@quantity", req["quantity"]);
                command.Parameters.AddWithValue("@userId", req["iduser"]);
                    
                var myReader = command.ExecuteReader();
                myReader.Close();
            }

            myCon.Close();
        }

        return new JsonResult(table);
    }

    // PUT: api/Services/5
    [HttpPut("{id}")]
    public void Put(int id)
    {
    }

    // DELETE: api/Services/5
    [HttpDelete("{id}")]
    public JsonResult Delete(int id)
    {
        string query = @"
                            DELETE FROM `services`
                            WHERE idservices = @serviceId
               ";
        Console.WriteLine(id);
        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
        {
            myCon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
            {
                myCommand.Parameters.AddWithValue("@serviceId", id);
                    
                var reader = myCommand.ExecuteReader();
                table.Load(reader);
                    
                reader.Close();
            }    
            myCon.Close();
        }
            
        return new JsonResult(table);
    }
}