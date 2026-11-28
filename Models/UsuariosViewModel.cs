using SmartProcessAI.Models;

namespace SmartProcessAI.ViewModels;

public class UsuariosViewModel
{
    public List<Usuario> Lista { get; set; } = new();
    public List<Rol> Roles { get; set; } = new();
}