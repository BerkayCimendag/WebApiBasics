using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApıBegining.Dto;
using WebApıBegining.Interfaces;
using WebApıBegining.Models;
using WebApıBegining.Repository;

namespace WebApıBegining.Controllers
{
    [Route("api/[controller]")]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository,IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(countries);
        }


        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));
            return Ok(country);
        }


        [HttpGet("/owners/{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200,Type = typeof(Country))]
        public IActionResult GetCountryOfAnOwner(int ownerId)
        {
            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwner(ownerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(country);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]

        public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
        {
            if (countryCreate == null)
            {
                return BadRequest();
            }
            var country = _countryRepository.GetCountries().Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.Trim().ToUpper()).FirstOrDefault();


            if (country != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var categoryMap = _mapper.Map<Country>(countryCreate);

            if (!_countryRepository.CreateCountry(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Created");
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto updatedCountry)
        {
            if (updatedCountry == null)
            {
                return BadRequest(ModelState);
            }
            if (countryId != updatedCountry.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_countryRepository.CountryExists(countryId))
            {
                return NotFound();

            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var countryMap = _mapper.Map<Country>(updatedCountry);

            if (!_countryRepository.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong on updating category");
                return StatusCode(500, ModelState);

            }
            return Ok("Successfully Updated");
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public IActionResult DeleteCategory(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }

            var countryToDelete = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong.");
            }

            return Ok("Successfully Deleted");
        }
    }
}
