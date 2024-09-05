using Microsoft.AspNetCore;
using System.Data;
using Npgsql;
using ApiServer.Models;
using ApiServer.Utils;
using Microsoft.AspNetCore.Http.Extensions;

namespace ApiServer.Services;

public class PotService(NpgsqlConnection connection) : IPotService, IDisposable
{
    public async Task<IEnumerable<Pot>> GetAll() {
        var pots = new List<Pot>();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT potes.id AS id, potes.campeonato_id AS champion_id, campeonatos.nome AS champion_name, potes.time_id AS team_id, " +
            "times.nome AS team_name, potes.valor_inicial AS init_value, potes.saldo_atual AS cur_value, potes.criado_em AS create_date, " +
            "potes.status AS status " +
            "FROM potes " +
            "LEFT JOIN campeonatos ON campeonatos.id = potes.campeonato_id " +
            "LEFT JOIN times ON times.id = potes.time_id";
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                var pot = new Pot
                {
                    Id = Convert.ToInt32(reader["id"]),
                    ChampionId = Convert.ToInt32(reader["champion_id"]),
                    TeamId = Convert.ToInt32(reader["team_id"]),
                    ChampionName = Convert.ToString(reader["champion_name"])!,
                    TeamName = Convert.ToString(reader["team_name"])!,
                    InitValue = Convert.ToDouble(reader["init_value"]),
                    CurValue = Convert.ToDouble(reader["cur_value"]),
                    Status = Convert.ToBoolean(reader["status"]),
                    CreateDate = Convert.ToString(reader["create_date"])!
                };
                pots.Add(pot);
            }
        }
        await connection.CloseAsync();
        return [.. pots];
    }

    public async Task<Pot> GetPot(int id, HttpRequest request) {
        // Console.WriteLine(request.Scheme + "://" + request.Host);
        var pot = new Pot();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM potes WHERE id = @Id";
        cmd.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                pot.Id = Convert.ToInt32(reader["id"]);
                pot.ChampionId = Convert.ToInt32(reader["campeonato_id"]);
                pot.TeamId = Convert.ToInt32(reader["time_id"]);
                pot.InitValue = Convert.ToDouble(reader["valor_inicial"]);
                pot.CurValue = Convert.ToDouble(reader["saldo_atual"]);
                pot.Status = Convert.ToBoolean(reader["status"]);
                pot.CreateDate = Convert.ToString(reader["criado_em"])!;
            }
        }
        await connection.CloseAsync();
        return pot;
    }

    public async Task<bool> Create(Pot pot) {
        const string insertQuery =
            "INSERT INTO potes (campeonato_id, time_id, valor_inicial, saldo_atual, status, criado_em) VALUES (@ChampionId, @TeamId, @InitValue, @CurValue, @Status, @CreateDate)";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = insertQuery;
        AddParameters(cmd, pot);
        await connection.OpenAsync();
        var rowAffected = await cmd.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }
    
    public async Task<bool> Update(Pot pot) {
        const string updateQuery =
                "UPDATE campeonatos SET nome = @Name, data_inicio = @StartDate, " +
                "data_fm = @EndDate, banner_url = @BannerURL, banner_mime_type = @BannerMimeType, status = @Status, criado_em = @CreateDate WHERE id = @Id";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = updateQuery;
        AddParameters(cmd, pot);
        await connection.OpenAsync();
        var rowAffected = await cmd.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }
    
    public async Task<bool> Delete(int id) {
        const string deleteQuery = "DELETE FROM potes WHERE id = @Id";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = deleteQuery;
        cmd.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        var rowAffected = await cmd.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        return rowAffected > 0;
    }

    private static void AddParameters(NpgsqlCommand command, Pot pot)
    {
        var parameters = command.Parameters;

        parameters.AddWithValue("@Id", pot.Id);
        parameters.AddWithValue("@ChampionId", pot.ChampionId);
        parameters.AddWithValue("@TeamId", pot.TeamId);
        parameters.AddWithValue("@InitValue", pot.InitValue);
        parameters.AddWithValue("@CurValue", pot.CurValue);
        parameters.AddWithValue("@Status", pot.Status);
        parameters.AddWithValue("@CreateDate", pot.CreateDate);
    }
    public void Dispose() {
        if (connection.State != ConnectionState.Closed) {
            connection.Close();
        }
        GC.SuppressFinalize(this);
    }
}

