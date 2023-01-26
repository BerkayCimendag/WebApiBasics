using WebApıBegining.Data;
using WebApıBegining.Interfaces;
using WebApıBegining.Models;

namespace WebApıBegining.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly DataContext _context;

        public OwnerRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateOwner(Owner owner)
        {
            _context.Owners.Add(owner);
            return Save();
        }

        public bool DeleteOwner(Owner Owner)
        {
            _context.Owners.Remove(Owner);
            return Save();
        }

        public Owner GetOwner(int ownerId)
        {
            return _context.Owners.Where(x => x.Id == ownerId).FirstOrDefault();

        }

        public ICollection<Owner> GetOwnerOfPokemon(int pokeId)
        {
            return _context.PokemonOwners.Where(p => p.PokemonId == pokeId).Select(x => x.Owner).ToList();
        }

        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.ToList();

        }

        public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
        {
            return _context.PokemonOwners.Where(p => p.OwnerId == ownerId).Select(x => x.Pokemon).ToList();
        }

        public bool OwnerExists(int ownerId)
        {
            return _context.Owners.Any(x => x.Id == ownerId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateOwner(Owner owner)
        {
            _context.Owners.Update(owner);
            return Save();
        }
    }
}
