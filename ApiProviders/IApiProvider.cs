namespace bot.ApiProviders
{
    public interface IApiProvider
    {
        Task Reply(string text);
        Task<string> GetMention(decimal id);
        Task<string> GetNick(decimal id);
        Task<IEnumerable<string>> GetPictures();
    }
}