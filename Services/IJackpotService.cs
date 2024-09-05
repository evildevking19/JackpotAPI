using ApiServer.Models;

namespace ApiServer.Services;

public interface IJackpotService
{
    Task<IEnumerable<Jackpot>> GetAll();
    Task<Jackpot> GetJackpot(int id, HttpRequest request);
    Task<bool> Create(Jackpot jackpot);
    Task<bool> Update(Jackpot jackpot);
}
