using BookStoreAPI.Models.DTOs;
using BookStoreAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Genera un JWT para iniciar sesión.
    /// </summary>
    /// <remarks>
    /// Para probar usa el usuario que ya viene cargado:
    ///
    ///     {
    ///        "username": "admin",
    ///        "password": "admin123"
    ///     }
    ///
    /// Cuando te responda con el `token`, cópialo, dale al botón **Authorize** de arriba
    /// y pégalo tal cual en el recuadro (solo el token, sin escribir `Bearer` ni nada más;
    /// Swagger se encarga de eso). El token dura 1 hora.
    /// </remarks>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }
}
