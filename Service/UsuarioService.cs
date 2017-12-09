using System.Linq;
using System.Threading.Tasks;
using TODO_API.Models;
using TodoApi.Models;

namespace TODO_API.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly TodoContext _context;
        public UsuarioService(TodoContext context)
        {
            _context = context;
        }

        public Task<Usuario> FindUserAsync(string id, string token)
        {
            Usuario usuario = new Usuario();
            if (id == null || token == null)
            {
                return Task.FromResult(usuario);
            }

            usuario = _context.Usuario.SingleOrDefault(u => u.Id == id && u.Token == token);
            
            return Task.FromResult(usuario);
        }
    }
}