using Microsoft.AspNetCore;
using System.Data;
using Npgsql;
using ApiServer.Models;
using ApiServer.Utils;
using Microsoft.AspNetCore.Http.Extensions;

namespace ApiServer.Services;

public class TeamService(NpgsqlConnection connection) : ITeamService, IDisposable
{

    public async Task<IEnumerable<Team>> GetAll() {
        var teams = new List<Team>();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM times";
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                var team = new Team
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = Convert.ToString(reader["nome"])!,
                    BannerMimeType = Convert.ToString(reader["banner_mime_type"])!,
                    BannerURL = Convert.ToString(reader["banner_url"])!,
                    CreateDate = Convert.ToString(reader["criado_em"])!
                };
                team.BannerURL = ImageLib.getImageDataUrl(team.BannerURL, team.BannerMimeType);
                teams.Add(team);
            }
        }
        await connection.CloseAsync();
        return [.. teams];
    }

    public async Task<Team> GetTeam(int id, HttpRequest request) {
        // Console.WriteLine(request.Scheme + "://" + request.Host);
        var team = new Team();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM times WHERE id = @Id";
        cmd.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                team.Id = Convert.ToInt32(reader["id"]);
                team.Name = Convert.ToString(reader["nome"])!;
                team.BannerURL = Convert.ToString(reader["banner_url"])!;
                team.BannerMimeType = Convert.ToString(reader["banner_mime_type"])!;
                team.BannerURL = ImageLib.getImageDataUrl(team.BannerURL, team.BannerMimeType);
                team.CreateDate = Convert.ToString(reader["criado_em"])!;
            }
        }
        await connection.CloseAsync();
        return team;
    }

    public async Task<bool> Create(Team team) {
        const string insertQuery =
            "INSERT INTO times (nome, banner_url, banner_mime_type, criado_em) VALUES (@Name, @BannerURL, @BannerMimeType, @CreateDate)";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = insertQuery;
        team.BannerURL = ImageLib.storeFile("Team"+DateTime.Now.ToString("ddMMyyyyHHmmss")+".png", team.BannerURL);
        AddParameters(cmd, team);
        await connection.OpenAsync();
        var rowAffected = await cmd.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }
    
    public async Task<bool> Update(Team team) {
        const string imagePathQuery = "SELECT banner_url FROM times WHERE id = @Id";
        const string updateQuery = "UPDATE times SET nome = @Name, banner_url = @BannerURL, banner_mime_type = @BannerMimeType, criado_em = @CreateDate WHERE id = @Id";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = imagePathQuery;
        cmd.Parameters.AddWithValue("@Id", team.Id);
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                var imagePath = Convert.ToString(reader["banner_url"])!;
                ImageLib.removeImage(imagePath);
            }
        }
        await connection.CloseAsync();
        using var cmd1 = connection.CreateCommand();
        cmd1.CommandText = updateQuery;
        team.BannerURL = ImageLib.storeFile("Team"+DateTime.Now.ToString("ddMMyyyyHHmmss")+".png", team.BannerURL);
        AddParameters(cmd1, team);
        await connection.OpenAsync();
        var rowAffected = await cmd1.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }
    
    public async Task<bool> Delete(int id) {
        const string deleteQuery = "DELETE FROM times WHERE id = @Id";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = deleteQuery;
        cmd.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        var rowAffected = await cmd.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }

    private static void AddParameters(NpgsqlCommand command, Team team)
    {
        var parameters = command.Parameters;

        parameters.AddWithValue("@Id", team.Id);
        parameters.AddWithValue("@Name", team.Name ?? string.Empty);
        parameters.AddWithValue("@BannerURL", team.BannerURL);
        parameters.AddWithValue("@BannerMimeType", team.BannerMimeType);
        parameters.AddWithValue("@CreateDate", team.CreateDate);
    }
    public void Dispose() {
        if (connection.State != ConnectionState.Closed) {
            connection.Close();
        }
        GC.SuppressFinalize(this);
    }
}

