using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        
        public OrderController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        
        // GET: api/Order
        [HttpGet]
        public JsonResult Get()
        {
            
            // needs tweek
            string query = @"
                            SELECT orderId,itemId,userId,quantity,orderDate,info
                            From `order`
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
        
        // GET: api/Order/5
        [HttpGet("{id}")]
        public JsonResult GetId(int id)
        {
            string query = @"
                            SELECT orderId,itemId,userId,quantity,orderDate,info
                            From `order`
                            WHERE userId = @userId";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@userId", id);

                    var reader = myCommand.ExecuteReader();
                    table.Load(reader);
                    
                    reader.Close();
                }    
                myCon.Close();
            }
            
            return new JsonResult(table);
        }

        // Post api/Order
        [HttpPost]
        public JsonResult Post()
        {
            var req = Request.Form;
            
            string query = @"
                            INSERT INTO `order`(`itemId`, `userId`, `quantity`, `orderDate`, `address`, `city`, `country`)
                            VALUES (@itemId, @userId, @quantity, @orderDate, @address, @city, @country)
               ";
            
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@itemId", req["itemId"]);
                    myCommand.Parameters.AddWithValue("@userId", req["userId"]);
                    myCommand.Parameters.AddWithValue("@quantity", req["quantity"]);
                    myCommand.Parameters.AddWithValue("@orderDate", req["orderDate"]);
                    myCommand.Parameters.AddWithValue("@address", req["address"]);
                    myCommand.Parameters.AddWithValue("@city", req["city"]);
                    myCommand.Parameters.AddWithValue("@country", req["country"]);
                    
                    var reader = myCommand.ExecuteReader();
                    table.Load(reader);
                    
                    reader.Close();
                }    
                myCon.Close();
            }

            return new JsonResult(table);
        }
        
        // Delete api/Order/5
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                            DELETE FROM `order`
                            WHERE orderId = @orderId
               ";
            
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@orderId", id);
                    
                    var reader = myCommand.ExecuteReader();
                    table.Load(reader);
                    
                    reader.Close();
                }    
                myCon.Close();
            }
            
            return new JsonResult(table);
        }
    }
}
