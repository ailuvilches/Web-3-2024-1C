namespace API_Equipos.Controllers;

using API_Equipos.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class EquiposController : ControllerBase
{
    private static List<Equipo> equipos = new List<Equipo>();

    // GET: api/Equipos
    [HttpGet]
    public IEnumerable<Equipo> Get()
    {
        return equipos;
    }

    // POST: api/Equipos
    [HttpPost]
    public void Post([FromBody] Equipo equipo)
    {
        equipos.Add(equipo);
    }

    // DELETE: api/Equipos/nombreDelEquipo
    [HttpDelete("{nombre}")]
    public void Delete(string nombre)
    {
        var equipo = equipos.Find(e => e.Nombre == nombre);
        if (equipo != null)
        {
            equipos.Remove(equipo);
        }
    }
}
