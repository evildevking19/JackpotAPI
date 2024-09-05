using ApiServer.Models;

namespace ApiServer.Services;

public interface IChampionService
{
    Task<IEnumerable<Champion>> GetAll();
    Task<Champion> GetChampion(int id, HttpRequest request);
    Task<bool> Create(Champion champion);
    Task<bool> Update(Champion champion);
    Task<bool> Delete(int id);
}
