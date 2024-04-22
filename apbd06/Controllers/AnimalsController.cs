using System.Data.SqlClient;
using apbd06.Models;
using apbd06.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace apbd06.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetAnimals([FromQuery] string orderBy)
    {
        // Open connection
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Create command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        string[] columns = ["idanimal", "name", "description", "category", "area"];
        if (columns.Contains(orderBy.ToLower()))
        {
            command.CommandText = $"SELECT * FROM Animal ORDER BY {orderBy.ToUpper()};";
        }
        else if (orderBy == "")
        {
            command.CommandText = "SELECT * FROM Animal;";
        }
        else
        {
            return BadRequest("Wrong sorting request");
        }

        // Execute command
        var reader = command.ExecuteReader();

        var animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");
        int descriptionOrdinal = reader.GetOrdinal("Description");
        int categoryOrdinal = reader.GetOrdinal("Category");
        int areaOrdinal = reader.GetOrdinal("Area");

        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(nameOrdinal),
                Description = reader.GetString(descriptionOrdinal),
                Category = reader.GetString(categoryOrdinal),
                Area = reader.GetString(areaOrdinal)
            });
        }

        //var animals = _animalRepository.GetAnimals();

        return Ok(animals);
    }


    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {
        // Open connection
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Create command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES (@animalName,@animalDescription,@animalCategory,@animalArea)";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        command.Parameters.AddWithValue("@animalDescription", animal.Description);
        command.Parameters.AddWithValue("@animalCategory", animal.Category);
        command.Parameters.AddWithValue("@animalArea", animal.Area);

        // Execute command
        command.ExecuteNonQuery();

        return Created("", null);
    }

    [HttpPut(("/api/[controller]/{id}"))]
    public IActionResult UpdateAnimal(int id, UpdateAnimal animal)
    {
        // Open connection
        using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText =
            "UPDATE Animals SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @Id";
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Name", animal.Name);
        command.Parameters.AddWithValue("@Description", animal.Description);
        command.Parameters.AddWithValue("@Category", animal.Category);
        command.Parameters.AddWithValue("@Area", animal.Area);

        int rowsAffected = command.ExecuteNonQuery();
        if (rowsAffected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }
    [HttpDelete(("/api/[controller]/{id}"))]
    public IActionResult DeleteAnimal(int id)
    {
        // Open connection
        using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText =
            "DELETE * FROM ANIMAL WHERE IdAnimal = @Id";
        command.Parameters.AddWithValue("@Id", id);

        int rowsAffected = command.ExecuteNonQuery();
        if (rowsAffected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }
}