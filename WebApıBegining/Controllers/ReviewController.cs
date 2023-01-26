using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApıBegining.Dto;
using WebApıBegining.Interfaces;
using WebApıBegining.Models;
using WebApıBegining.Repository;

namespace WebApıBegining.Controllers
{
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IPokemonRepository _pokemonRepository;

        public ReviewController(IReviewRepository reviewRepository,IMapper mapper,IReviewerRepository reviewerRepository,IPokemonRepository pokemonRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _reviewerRepository = reviewerRepository;
            _pokemonRepository = pokemonRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));
            return Ok(review);
        }

        [HttpGet("pokemon/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
           
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfPokemon(pokeId));
            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId,[FromBody] ReviewDto reviewCreate)
        {
            if (reviewCreate == null)
            {
                return BadRequest();
            }
            var reviews = _reviewRepository.GetReviews().Where(c => c.Title.Trim().ToUpper() == reviewCreate.Title.Trim().ToUpper()).FirstOrDefault();


            if (reviews != null)
            {
                ModelState.AddModelError("", "Review already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var reviewMap = _mapper.Map<Review>(reviewCreate);

            reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId);
            reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);


            if (!_reviewRepository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Created");
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updatedReview)
        {
            if (updatedReview == null)
            {
                return BadRequest(ModelState);
            }
            if (reviewId != updatedReview.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewerRepository.ReviewerExists(reviewId))
            {
                return NotFound();

            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var reviewMap = _mapper.Map<Review>(updatedReview);

            if (!_reviewRepository.UpdateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong on updating review");
                return StatusCode(500, ModelState);

            }
            return Ok("Successfully Updated");
        }


        [HttpDelete("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var reviewToDelete = _reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewRepository.DeleteReview(reviewToDelete))
            {
                ModelState.AddModelError("", "Something went wrong.");
            }

            return Ok("Successfully Deleted");
        }
    }
}
