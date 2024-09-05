using ApiServer.Models;

namespace ApiServer.Services;

public interface IQuestionService
{
    Task<IEnumerable<Question>> GetAll();
    Task<Question> GetQuestion(int id, HttpRequest request);
    Task<bool> Create(Question question);
    Task<bool> Update(Question question);
}
