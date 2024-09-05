using ApiServer.Models;

namespace ApiServer.Services;

public interface IAwardService
{
    Task<IEnumerable<Award>> GetAll();
    Task<Award> GetAward(int id, HttpRequest request);
    Task<bool> Create(Award award);
    Task<bool> Update(Award award);
}
