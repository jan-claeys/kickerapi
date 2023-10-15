using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace kickerapi.Services
{
    public class PlayerService
    {
        private readonly KickerContext _context;

        public PlayerService(KickerContext _context)
        {
            this._context = _context;
        }
    }
}
