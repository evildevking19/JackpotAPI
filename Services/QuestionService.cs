using Microsoft.AspNetCore;
using System.Data;
using Npgsql;
using ApiServer.Models;
using ApiServer.Utils;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text.Json;

namespace ApiServer.Services;

public class QuestionService(NpgsqlConnection connection) : IQuestionService, IDisposable
{
    public async Task<IEnumerable<Question>> GetAll() {
        var questions = new List<Question>();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM perguntas";
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                var question = new Question
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Title = Convert.ToString(reader["titulo"])!,
                    Category = Convert.ToString(reader["categoria"])!,
                    PotLevel = Convert.ToInt32(reader["pote_nivel"]),
                    Status = Convert.ToBoolean(reader["status"]),
                    CreateDate = Convert.ToString(reader["criado_em"])!
                };
                questions.Add(question);
            }
        }
        await connection.CloseAsync();
        return [.. questions];
    }

    public async Task<Question> GetQuestion(int id, HttpRequest request) {
        // Console.WriteLine(request.Scheme + "://" + request.Host);
        var question = new Question();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM perguntas WHERE id = @Id";
        cmd.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader is not null) {
            while (await reader.ReadAsync()) {
                question.Id = Convert.ToInt32(reader["id"]);
                question.Title = Convert.ToString(reader["titulo"])!;
                question.Category = Convert.ToString(reader["categoria"])!;
                question.PotLevel = Convert.ToInt32(reader["pote_nivel"]);
            }
        }
        await connection.CloseAsync();
        var questionItems = new List<QuestionItem>();
        using var cmd1 = connection.CreateCommand();
        cmd1.CommandText = "SELECT * FROM pergunta_itens WHERE pergunta_id = @Id";
        cmd1.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        using var reader1 = await cmd1.ExecuteReaderAsync();
        if (reader1 is not null) {
            while (await reader1.ReadAsync()) {
                var item = new QuestionItem
                {
                    Id = Convert.ToInt32(reader1["id"]),
                    QId = Convert.ToInt32(reader1["pergunta_id"]),
                    QString = Convert.ToString(reader1["pergunta"])!,
                    QType = Convert.ToString(reader1["tipo"])!,
                    Quantity = Convert.ToString(reader1["quantidade_campos"])!,
                    ObjOptions = Convert.ToString(reader1["obj_options"])!,
                    SubjSingleValue = Convert.ToString(reader1["subj_unico_val"])!,
                    SubjDoubleValue = Convert.ToString(reader1["subj_duplo_val"])!,
                };
                questionItems.Add(item);
            }
        }
        await connection.CloseAsync();
        question.Items = questionItems;
        return question;
    }

    public async Task<bool> Create(Question question) {
        const string insertQuery = "INSERT INTO perguntas (titulo, categoria, pote_nivel, status, criado_em) VALUES (@Title, @Category, @PotLevel, @Status, @CreateDate) RETURNING id";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = insertQuery;
        AddParameters(cmd, question);
        await connection.OpenAsync();
        var newQuestionId = await cmd.ExecuteScalarAsync();
        await connection.CloseAsync();
        var insertItemQuery = "INSERT INTO pergunta_itens (pergunta_id, pergunta, tipo, quantidade_campos, obj_options, subj_unico_val, subj_duplo_val) VALUES ";
        foreach (QuestionItem item in question.Items) {
            insertItemQuery += $"({newQuestionId}, '{item.QString}', '{item.QType}', '{item.Quantity}', '{item.ObjOptions}', '{item.SubjSingleValue}', '{item.SubjDoubleValue}'), ";
        }
        insertItemQuery = insertItemQuery.Remove(insertItemQuery.Length - 2);
        using var cmd1 = connection.CreateCommand();
        cmd1.CommandText = insertItemQuery;
        await connection.OpenAsync();
        var rowAffected = await cmd1.ExecuteNonQueryAsync();
        return rowAffected > 0;
    }
    
    public async Task<bool> Update(Question question) {
        // const string updateQuery =
        //         "UPDATE campeonatos SET nome = @Name, data_inicio = @StartDate, " +
        //         "data_fm = @EndDate, banner_url = @BannerURL, banner_mime_type = @BannerMimeType, status = @Status, criado_em = @CreateDate WHERE id = @Id";
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) as cnt FROM perguntas";
        // AddParameters(cmd, pot);
        await connection.OpenAsync();
        var rowAffected = await cmd.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        // return rowAffected > 0;
        return true;
    }

    private static void AddParameters(NpgsqlCommand command, Question question)
    {
        var parameters = command.Parameters;

        parameters.AddWithValue("@Id", question.Id);
        parameters.AddWithValue("@Title", question.Title);
        parameters.AddWithValue("@Category", question.Category);
        parameters.AddWithValue("@PotLevel", question.PotLevel);
        parameters.AddWithValue("@Status", question.Status);
        parameters.AddWithValue("@CreateDate", question.CreateDate);
    }
    public void Dispose() {
        if (connection.State != ConnectionState.Closed) {
            connection.Close();
        }
        GC.SuppressFinalize(this);
    }
}

