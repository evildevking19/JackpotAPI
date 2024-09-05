using ApiServer.Models;

namespace ApiServer.Services;

public interface IPotService
{
    Task<IEnumerable<Pot>> GetAll();
    Task<Pot> GetPot(int id, HttpRequest request);
    Task<bool> Create(Pot pot);
    Task<bool> Update(Pot pot);
    Task<bool> Delete(int id);
}
