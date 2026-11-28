using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartProcessAI.Datos;
using SmartProcessAI.Models;
namespace SmartProcessAI.Services;

public class UsuarioService
{
    private const int LargoMinimoClave = 8;
    private readonly AppDbContext _db;
    private readonly PasswordHasher<Usuario> _hasher = new();
    public UsuarioService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<List<Usuario>> ListarAsync()
    {
        return await _db.Usuarios
            .Include(u => u.Rol)
            .OrderBy(u => u.Nombre)
            .ToListAsync();
    }
    public async Task<List<Rol>> RolesAsync()
    {
        return await _db.Roles.OrderBy(r => r.Id).ToListAsync();
    }
    public async Task<string?> CrearAsync(string? nombre, string? correo, int
rolId, string? clave, int actualId)
    {
        nombre = nombre?.Trim();
        correo = correo?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(nombre))
            return "El nombre es obligatorio.";
        if (!CorreoValido(correo))
            return "El correo no tiene un formato válido.";
        if (!ClaveValida(clave))
            return "La contraseña debe tener al menos 8 caracteres.";
        if (!await _db.Roles.AnyAsync(r => r.Id == rolId))
            return "El rol seleccionado no existe.";
        if (await _db.Usuarios.AnyAsync(u => u.Correo == correo))
            return "Ya existe un usuario con ese correo.";
        int empresaId = await _db.Usuarios
            .Where(u => u.Id == actualId)
            .Select(u => u.EmpresaId)
            .FirstOrDefaultAsync();
        if (empresaId == 0)
            return "No se pudo determinar la empresa del administrador.";
        var usuario = new Usuario
        {
            Nombre = nombre,
            Correo = correo!,
            RolId = rolId,
            EmpresaId = empresaId,
            Activo = true
        };
        usuario.PasswordHash = _hasher.HashPassword(usuario, clave!);
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();
        return null;
    }
    public async Task<string?> EditarAsync(int id, string? nombre, int rolId, int
actualId)
    {
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario == null)
            return "El usuario no existe.";
        nombre = nombre?.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
            return "El nombre es obligatorio.";
        if (!await _db.Roles.AnyAsync(r => r.Id == rolId))
            return "El rol seleccionado no existe.";
        if (usuario.Id == actualId && usuario.RolId != rolId)
            return "No puedes cambiar tu propio rol.";
        usuario.Nombre = nombre;
        usuario.RolId = rolId;
        await _db.SaveChangesAsync();
        return null;
    }
    public async Task<string?> CambiarEstadoAsync(int id, bool activo, int
actualId)
    {
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario == null)
            return "El usuario no existe.";
        if (usuario.Id == actualId && !activo)
            return "No puedes desactivar tu propia cuenta.";
        usuario.Activo = activo;
        await _db.SaveChangesAsync();
        return null;
    }
    public async Task<string?> RestablecerClaveAsync(int id, string? clave)
    {
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario == null)
            return "El usuario no existe.";
        if (!ClaveValida(clave))
            return "La contraseña debe tener al menos 8 caracteres.";
        usuario.PasswordHash = _hasher.HashPassword(usuario, clave!);
        await _db.SaveChangesAsync();
        return null;
    }
    private static bool CorreoValido(string? correo)
    {
        return !string.IsNullOrWhiteSpace(correo)
        && MailAddress.TryCreate(correo, out var direccion)
        && direccion.Address == correo;
    }
    private static bool ClaveValida(string? clave)
    {
        return !string.IsNullOrEmpty(clave) && clave.Length >= LargoMinimoClave;
    }
}