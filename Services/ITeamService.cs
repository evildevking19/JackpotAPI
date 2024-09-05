using ApiServer.Models;

namespace ApiServer.Services;

public interface ITeamService
{
    Task<IEnumerable<Team>> GetAll();
    Task<Team> GetTeam(int id, HttpRequest request);
    Task<bool> Create(Team team);
    Task<bool> Update(Team team);
    Task<bool> Delete(int id);
}
