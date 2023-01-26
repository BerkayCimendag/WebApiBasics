using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApıBegining.Data;
using WebApıBegining.Dto;
using WebApıBegining.Interfaces;
using WebApıBegining.Models;
using WebApıBegining.Repository;

namespace WebApıBegining.Controllers
{
    [Route("api/[controller]")]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IReviewRepository _reviewRepository;

        public PokemonController(IPokemonRepository pokemonRepository,IMapper mapper,IOwnerRepository ownerRepository, IReviewerRepository reviewerRepository,IReviewRepository reviewRepository)
        {
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
            _ownerRepository = ownerRepository;
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type =typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200,Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));
            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var rating = _pokemonRepository.GetPokemonRating(pokeId);
            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto pokemonCreate)
        {
            if (pokemonCreate == null)
            {
                return BadRequest();
            }
            var pokemons = _pokemonRepository.GetPokemons().Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.Trim().ToUpper()).FirstOrDefault();


            if (pokemons != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

            

            if (!_pokemonRepository.CreatePokemon(ownerId,catId,pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Created");
        }

        [HttpPut("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon( int pokeId, [FromQuery] int ownerId, [FromQuery] int catId,[FromBody] PokemonDto updatedPokemon)
        {
            if (updatedPokemon == null)
            {
                return BadRequest(ModelState);
            }
            if (pokeId != updatedPokemon.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();

            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var pokemonMap = _mapper.Map<Pokemon>(updatedPokemon);

            if (!_pokemonRepository.UpdatePokemon(ownerId,catId,pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong on updating category");
                return StatusCode(500, ModelState);

            }
            return Ok("Successfully Updated");
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }


            var reviewsToDelete = _reviewRepository.GetReviewsOfPokemon(pokeId);
            var pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", "Something went wrong ");
            }
            if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
            {
                ModelState.AddModelError("", "Something went wrong.");
            }

            return Ok("Successfully Deleted");
        }
    }
}
