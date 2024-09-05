using Microsoft.AspNetCore;
using System.Data;
using Npgsql;
using ApiServer.Models;
using ApiServer.Utils;
using Microsoft.AspNetCore.Http.Extensions;

namespace ApiServer.Services;

public class JackpotService(NpgsqlConnection connection) : IJackpotService, IDisposable
{

    public async Task<IEnumerable<Jackpot>> GetAll() {
        var jackpots = new List<Jackpot>();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM jackpots";
        
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                var jackpot = new Jackpot
                {
                    Id = Convert.ToInt32(reader["id"]),
                    ChampionId = Convert.ToInt32(reader["campeonato_id"]),
                    TeamId = Convert.ToInt32(reader["time_id"]),
                    QuestionId = Convert.ToInt32(reader["pergunta_id"]),
                    AwardType = Convert.ToString(reader["tipo_de_premiacao"])!,
                    PrizeCategory = Convert.ToString(reader["categoria_do_premio"])!,
                    EndDate = Convert.ToString(reader["data_fim"])!,
                    EndTime = Convert.ToString(reader["hora_fim"])!,
                    PotValue = Convert.ToDouble(reader["valor_do_pote"]),
                    PotBalance = Convert.ToDouble(reader["saldo_do_pote"]),
                    BannerURL = Convert.ToString(reader["banner_url"])!,
                    BannerMimeType = Convert.ToString(reader["banner_mime_type"])!,
                    BannerStatus = Convert.ToBoolean(reader["banner_status"]),
                    Status = Convert.ToString(reader["status"])!,
                    CreateDate = Convert.ToString(reader["criado_em"])!,
                    LotteryDate = Convert.ToString(reader["data_loteria_federal"])!,
                    RoundName = Convert.ToString(reader["rodada"])!,
                    HomeTeamNames = Convert.ToString(reader["times_mandante"])!,
                    TicketGames = Convert.ToString(reader["jogo_ingresso"])!,
                    TicketSales = Convert.ToString(reader["integracao_venda_ingresso"])!
                };
                jackpots.Add(jackpot);
            }
        }
        await connection.CloseAsync();
        return [.. jackpots];
    }

    public async Task<Jackpot> GetJackpot(int id, HttpRequest request) {
        // Console.WriteLine(request.Scheme + "://" + request.Host);
        var jackpot = new Jackpot();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM jackpots WHERE id = @Id";
        cmd.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                jackpot.Id = Convert.ToInt32(reader["id"]);
            }
        }
        await connection.CloseAsync();
        return jackpot;
    }

    public async Task<bool> Create(Jackpot jackpot) {
        const string insertQuery =
            "INSERT INTO campeonatos (nome, data_inicio, data_fm, banner_url, banner_mime_type, status, criado_em) VALUES (@Name, @StartDate, @EndDate, @BannerURL, @BannerMimeType, @Status, @CreateDate)";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = insertQuery;
        jackpot.BannerURL = ImageLib.storeFile("Banner"+DateTime.Now.ToString("ddMMyyyyHHmmss")+".png", jackpot.BannerURL);
        AddParameters(cmd, jackpot);
        await connection.OpenAsync();
        var rowAffected = await cmd.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }
    
    public async Task<bool> Update(Jackpot jackpot) {
        const string imagePathQuery = "SELECT banner_url FROM campeonatos WHERE id = @Id";
        const string updateQuery =
                "UPDATE campeonatos SET nome = @Name, data_inicio = @StartDate, " +
                "data_fm = @EndDate, banner_url = @BannerURL, banner_mime_type = @BannerMimeType, status = @Status, criado_em = @CreateDate WHERE id = @Id";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = imagePathQuery;
        cmd.Parameters.AddWithValue("@Id", jackpot.Id);
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
        AddParameters(cmd1, jackpot);
        await connection.OpenAsync();
        var rowAffected = await cmd1.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }

    private static void AddParameters(NpgsqlCommand command, Jackpot jackpot)
    {
        var parameters = command.Parameters;

        parameters.AddWithValue("@Id", jackpot.Id);
    }
    public void Dispose() {
        if (connection.State != ConnectionState.Closed) {
            connection.Close();
        }
        GC.SuppressFinalize(this);
    }
}

