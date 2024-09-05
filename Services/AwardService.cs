using Microsoft.AspNetCore;
using System.Data;
using Npgsql;
using ApiServer.Models;
using ApiServer.Utils;
using Microsoft.AspNetCore.Http.Extensions;

namespace ApiServer.Services;

public class AwardService(NpgsqlConnection connection) : IAwardService, IDisposable
{
    public async Task<IEnumerable<Award>> GetAll() {
        var awards = new List<Award>();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM premios";
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                var award = new Award
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Description = Convert.ToString(reader["descricao"])!,
                    Category = Convert.ToString(reader["categoria"])!,
                    Status = Convert.ToString(reader["status"])!,
                    CreateDate = Convert.ToString(reader["criado_em"])!
                };
                awards.Add(award);
            }
        }
        await connection.CloseAsync();
        return [.. awards];
    }

    public async Task<Award> GetAward(int id, HttpRequest request) {
        // Console.WriteLine(request.Scheme + "://" + request.Host);
        var award = new Award();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM premios WHERE id = @Id";
        cmd.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                award.Id = Convert.ToInt32(reader["id"]);
                award.Description = Convert.ToString(reader["descricao"])!;
                award.Code = Convert.ToString(reader["codigo"])!;
                award.Category = Convert.ToString(reader["categoria"])!;
                award.Status = Convert.ToString(reader["status"])!;
                award.Active = Convert.ToBoolean(reader["active"]);
                award.BannerMimeType = Convert.ToString(reader["banner_mime_type"])!;
                award.BannerURL = ImageLib.getImageDataUrl(Convert.ToString(reader["banner_url"])!, award.BannerMimeType);
                award.CreateDate = Convert.ToString(reader["criado_em"])!;
            }
        }
        await connection.CloseAsync();
        return award;
    }

    public async Task<bool> Create(Award award) {
        const string insertQuery =
            "INSERT INTO premios (descricao, codigo, categoria, banner_url, banner_mime_type, status, active, criado_em) VALUES (@Description, @Code, @Category, @BannerURL, @BannerMimeType, @Status, @Active, @CreateDate)";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = insertQuery;
        award.BannerURL = ImageLib.storeFile("Banner"+DateTime.Now.ToString("ddMMyyyyHHmmss")+".png", award.BannerURL);
        AddParameters(cmd, award);
        await connection.OpenAsync();
        var rowAffected = await cmd.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }
    
    public async Task<bool> Update(Award award) {
        const string imagePathQuery = "SELECT banner_url FROM premios WHERE id = @Id";
        const string updateQuery =
                "UPDATE premios SET descricao = @Description, codigo = @Code, categoria = @Category, banner_url = @BannerURL, banner_mime_type = @BannerMimeType, status = @Status, active = @Active, criado_em = @CreateDate WHERE id = @Id";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = imagePathQuery;
        cmd.Parameters.AddWithValue("@Id", award.Id);
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
        AddParameters(cmd1, award);
        await connection.OpenAsync();
        var rowAffected = await cmd1.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }

    private static void AddParameters(NpgsqlCommand command, Award award)
    {
        var parameters = command.Parameters;

        parameters.AddWithValue("@Id", award.Id);
        parameters.AddWithValue("@Description", award.Description ?? string.Empty);
        parameters.AddWithValue("@Code", award.Code ?? string.Empty);
        parameters.AddWithValue("@Category", award.Category);
        parameters.AddWithValue("@Status", award.Status);
        parameters.AddWithValue("@Active", award.Active);
        parameters.AddWithValue("@BannerURL", award.BannerURL);
        parameters.AddWithValue("@BannerMimeType", award.BannerMimeType);
        parameters.AddWithValue("@CreateDate", award.CreateDate);
    }
    public void Dispose() {
        if (connection.State != ConnectionState.Closed) {
            connection.Close();
        }
        GC.SuppressFinalize(this);
    }
}

