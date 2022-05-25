using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MySql.Data.MySqlClient;
using QuizApi.Models;

namespace QuizApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    public EmployeeController(IConfiguration configuration,IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    [HttpGet]
    public JsonResult Get()
    {
        string query = @"
                        select EmployeeId,EmployeeName,Department,
                        DATE_FORMAT(DateOfJoining,'%Y-%m-%d') as DateOfJoining,
                        PhotoFileName
                        from 
                        Employee
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

    [HttpPost]
    public JsonResult Post()
    {
        var req = Request.Form;
        string imageName;
        try
        {
            var postedImage = req.Files[0];
            string filename = postedImage.FileName;
            var physicalPath = _env.ContentRootPath + "/Images/" + filename;

            using(var stream=new FileStream(physicalPath, FileMode.Create))
            {
                postedImage.CopyTo(stream);
            }

            imageName = filename;
        } 
        catch (Exception)
        {
            imageName = "anonymous.png";
        }

        string query = @"
                        insert into Employee 
                        (EmployeeName,Department,DateOfJoining,PhotoFileName) 
                        values
                         (@EmployeeName,@Department,@DateOfJoining,@PhotoFileName) ;
            ";
        
        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@EmployeeName", req["EmployeeName"]);
                myCommand.Parameters.AddWithValue("@Department", req["Department"]);
                myCommand.Parameters.AddWithValue("@PhotoFileName", imageName);
                myCommand.Parameters.AddWithValue("@DateOfJoining", req["StartDate"]);
                
                var myReader = myCommand.ExecuteReader();
                table.Load(myReader);
        
                myReader.Close();
                mycon.Close();
            }
        }
        
        return new JsonResult("file Uploaded");
    }

    [HttpPut]
    public JsonResult Put(Employee emp)
    {
        string query = @"
                        update Employee set 
                        EmployeeName =@EmployeeName,
                        Department =@Department,
                        DateOfJoining =@DateOfJoining,
                        PhotoFileName =@PhotoFileName
                        where EmployeeId=@EmployeeId;
            ";

        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        MySqlDataReader myReader;
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@EmployeeId", emp.EmployeeId);
                myCommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                myCommand.Parameters.AddWithValue("@Department", emp.Department);
                myCommand.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                myCommand.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);

                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }

        return new JsonResult("Updated Successfully");
    }
    
    [HttpDelete("{id}")]
    public JsonResult Delete(int id)
    {
        string query = @"
                        delete from Employee 
                        where EmployeeId=@EmployeeId;
            ";

        DataTable table = new DataTable();
        string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@EmployeeId", id);

                var myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }

        return new JsonResult("Deleted Successfully");
    }
    
    [Route("SaveFile")]
    [HttpPost]
    public JsonResult SaveFile()
    {
        try
        {
            var httpRequest = Request.Form;
            var postedFile = httpRequest.Files[0];
            string filename = postedFile.FileName;
            var physicalPath = _env.ContentRootPath + "/Images/" + filename;

            using(var stream=new FileStream(physicalPath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
            }

            return new JsonResult(filename);
        }
        catch (Exception)
        {
            return new JsonResult("anonymous.png");
        }
    }
}