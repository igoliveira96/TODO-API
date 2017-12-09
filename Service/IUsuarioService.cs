using System.Threading.Tasks;
using TODO_API.Models;

namespace TODO_API.Service
{
    public interface IUsuarioService
    {
        Task<Usuario> FindUserAsync(string id, string token);
    }
}