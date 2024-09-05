using Microsoft.AspNetCore;
using System.Data;
using Npgsql;
using ApiServer.Models;
using ApiServer.Utils;
using Microsoft.AspNetCore.Http.Extensions;

namespace ApiServer.Services;

public class ChampionService(NpgsqlConnection connection) : IChampionService, IDisposable
{

    public async Task<IEnumerable<Champion>> GetAll() {
        var champions = new List<Champion>();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM campeonatos";
        
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                var champion = new Champion
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = Convert.ToString(reader["nome"])!,
                    StartDate = Convert.ToString(reader["data_inicio"])!,
                    EndDate = Convert.ToString(reader["data_fm"])!,
                    Status = Convert.ToBoolean(reader["status"])!
                };
                champions.Add(champion);
            }
        }
        await connection.CloseAsync();
        return [.. champions];
    }

    public async Task<Champion> GetChampion(int id, HttpRequest request) {
        // Console.WriteLine(request.Scheme + "://" + request.Host);
        var champion = new Champion();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM campeonatos WHERE id = @Id";
        cmd.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                champion.Id = Convert.ToInt32(reader["id"]);
                champion.Name = Convert.ToString(reader["nome"])!;
                champion.StartDate = Convert.ToString(reader["data_inicio"])!;
                champion.EndDate = Convert.ToString(reader["data_fm"])!;
                champion.BannerURL = Convert.ToString(reader["banner_url"])!;
                champion.BannerMimeType = Convert.ToString(reader["banner_mime_type"])!;
                champion.BannerURL = ImageLib.getImageDataUrl(champion.BannerURL, champion.BannerMimeType);
                champion.Status = Convert.ToBoolean(reader["status"])!;
                champion.CreateDate = Convert.ToString(reader["criado_em"])!;
            }
        }
        await connection.CloseAsync();
        return champion;
    }

    public async Task<bool> Create(Champion champion) {
        const string insertQuery =
            "INSERT INTO campeonatos (nome, data_inicio, data_fm, banner_url, banner_mime_type, status, criado_em) VALUES (@Name, @StartDate, @EndDate, @BannerURL, @BannerMimeType, @Status, @CreateDate)";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = insertQuery;
        champion.BannerURL = ImageLib.storeFile("Banner"+DateTime.Now.ToString("ddMMyyyyHHmmss")+".png", champion.BannerURL);
        AddParameters(cmd, champion);
        await connection.OpenAsync();
        var rowAffected = await cmd.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }
    
    public async Task<bool> Update(Champion champion) {
        const string imagePathQuery = "SELECT banner_url FROM campeonatos WHERE id = @Id";
        const string updateQuery =
                "UPDATE campeonatos SET nome = @Name, data_inicio = @StartDate, " +
                "data_fm = @EndDate, banner_url = @BannerURL, banner_mime_type = @BannerMimeType, status = @Status, criado_em = @CreateDate WHERE id = @Id";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = imagePathQuery;
        cmd.Parameters.AddWithValue("@Id", champion.Id);
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
        AddParameters(cmd1, champion);
        await connection.OpenAsync();
        var rowAffected = await cmd1.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }
    
    public async Task<bool> Delete(int id) {
        const string deleteQuery = "DELETE FROM campeonatos WHERE id = @Id";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = deleteQuery;
        cmd.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        var rowAffected = await cmd.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }

    private static void AddParameters(NpgsqlCommand command, Champion champion)
    {
        var parameters = command.Parameters;

        parameters.AddWithValue("@Id", champion.Id);
        parameters.AddWithValue("@Name", champion.Name ?? string.Empty);
        parameters.AddWithValue("@StartDate", champion.StartDate ?? string.Empty);
        parameters.AddWithValue("@EndDate", champion.EndDate ?? string.Empty);
        parameters.AddWithValue("@Status", champion.Status);
        parameters.AddWithValue("@BannerURL", champion.BannerURL);
        parameters.AddWithValue("@BannerMimeType", champion.BannerMimeType);
        parameters.AddWithValue("@CreateDate", champion.CreateDate);
    }
    public void Dispose() {
        if (connection.State != ConnectionState.Closed) {
            connection.Close();
        }
        GC.SuppressFinalize(this);
    }
}

