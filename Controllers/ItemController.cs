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
public class ItemController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    
    public ItemController(IConfiguration configuration,IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    // Get api/item
    [HttpGet]
    public JsonResult Get()
    {
        string query = @"
                            SELECT itemId,name,`desc`,quantity,price,pic
                            FROM items
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

    // Get api/item/5
    [HttpGet("{id}")]
    public JsonResult GetById(int id)
    {
        string query = @"
                            SELECT itemId,name,`desc`,quantity,price,pic
                            FROM items WHERE itemId = @itemId
               ";

        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
        {
            myCon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
            {
                myCommand.Parameters.AddWithValue("@itemId", id);
                var reader = myCommand.ExecuteReader();
                table.Load(reader);
                    
                reader.Close();
                myCon.Close();
            }    
        }
            
        return new JsonResult(table);
    }
    
    // Post api/item
    [HttpPost]
    public JsonResult Post()
    {
        var req = Request.Form;

        string image;
        try
        {
            var postedFile = req.Files[0];
            string fileName = postedFile.FileName;
            string filePath = _env.ContentRootPath + "/images/" + fileName;

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
            }
            image = fileName;
                
        }
        catch (Exception)
        {
            image = "m.jpg";
        }
            
        string query = @"
                            INSERT INTO items(name,`desc`,quantity,price,pic)
                            VALUES(@name,@desc,@quantity,@price,@pic)
            ";
            
        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
        {
            myCon.Open();
            using (MySqlCommand command = new MySqlCommand(query, myCon))
            {
                command.Parameters.AddWithValue("@name", req["name"]);
                command.Parameters.AddWithValue("@desc", req["desc"]);
                command.Parameters.AddWithValue("@quantity", req["quantity"]);
                command.Parameters.AddWithValue("@price", req["price"]);
                command.Parameters.AddWithValue("@pic", image);
                    
                var myReader = command.ExecuteReader();
                myReader.Close();
            }
            query = @"
                            SELECT itemId,name,`desc`,quantity,price,pic
                            FROM items order by itemId desc limit 1";

            using (MySqlCommand command = new MySqlCommand(query, myCon))
            {
                var myReader = command.ExecuteReader();
                table.Load(myReader);
                    
                myReader.Close();
            }
                
            myCon.Close();
        }

        return new JsonResult(table);
    }
    
    // Put api/item/5
    [HttpPut("{id}")]
    public JsonResult Put(int id)
    {
        var req = Request.Form;
        
        string image;
        try
        {
            var postedFile = req.Files[0];
            string fileName = postedFile.FileName;
            string filePath = _env.ContentRootPath + "/images/" + fileName;

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
            }
            image = fileName;
                
        }
        catch (Exception)
        {
            image = "m.jpg";
        }
        
        string query = @"
                            UPDATE items
                            SET name = @name,`desc` = @desc,quantity = @quantity,price = @price,pic = @pic
                            WHERE itemId = @itemId
            ";
        
        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
        {
            myCon.Open();
            using (MySqlCommand command = new MySqlCommand(query, myCon))
            {
                command.Parameters.AddWithValue("@name", req["name"]);
                command.Parameters.AddWithValue("@desc", req["desc"]);
                command.Parameters.AddWithValue("@quantity", req["quantity"]);
                command.Parameters.AddWithValue("@price", req["price"]);
                command.Parameters.AddWithValue("@pic", image);
                command.Parameters.AddWithValue("@itemId", id);
                    
                var myReader = command.ExecuteReader();
                table.Load(myReader);
                myReader.Close();
            }
            myCon.Close();     
        }
        
        return new JsonResult("Success");
    }
}