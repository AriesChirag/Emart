namespace Emart_DotNet.Services
{
    public interface IEPointsService
    {
        Task<int> CreditPointsAsync(int userId, int points);
        Task<int> RedeemPointsAsync(int userId, int points);
        Task<int> GetBalanceAsync(int userId);
    }
}