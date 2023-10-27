namespace kickerapi.Services
{
    public class ContextService
    {
        protected readonly KickerContext _context;

        public ContextService(KickerContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
