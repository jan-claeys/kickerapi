namespace kickerapi.Services
{
    public interface IServiceContext
    {
        Task<int> SaveChangesAsync();
    }
}
