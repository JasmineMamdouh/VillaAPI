using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VillaAPI.Models.Dto;

using VillaAPI.Data;
using Microsoft.AspNetCore.JsonPatch;
using System.Runtime.Serialization;
using VillaAPI.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using VillaAPI.Repository.IRepository;
using System.Net;
using Azure;
using Microsoft.AspNetCore.Authorization;

namespace VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    //so that if contoller name is modified, you won't have to notify all consumers
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {/*
        public ILogger<VillaAPIController> logger; 

        //logger dependency injection
        //note: for production you need to log into a file, download package like serilog aspnetcore, serilog,sinks.file
        public VillaAPIController(ILogger<VillaAPIController> _logger)
        {
            logger = _logger;
        }
        */

        protected APIResponse _response;
        //private ApplicationDbContext _db;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper mapper;

        //get the dbContext by dependency injection
        public VillaAPIController(IVillaRepository dbVilla, IMapper _mapper)
        {
            //_db = db;
            _dbVilla = dbVilla;
            mapper = _mapper;
            this._response = new();
        }

        /*let the return type be
         IActionResult -> flexible, std to return diff http status codes
         async Task<IActionResult> -> when returning varied HTTP status codes asynchronously.  --> Put, Patch, Delete
         ActionResult<T> -> strongly typed, enforce specific return type, either an object<T> or http respone  --> Get 'to know what model to return in success'
         async Task<ActionResult<T>> -> for async methods when using EF or external services  --> Post
        */

        /*Let any dealing with the EF dbContext be async
         * In ASP.NET Core API, async methods are commonly used to improve performance, scalability, and responsiveness.
         * 1. Improved Scalability
            Async methods free up threads while waiting for I/O operations (like database queries, HTTP requests, or file access).
            This allows the server to handle more concurrent requests with fewer resources.
           2. Non-Blocking I/O Operations
              Traditional synchronous calls block the thread until the operation is completed.
              Async methods let the thread perform other tasks while waiting for I/O operations (e.g., database calls, external API calls).
           3. Database Operations
               Use Case: When interacting with databases using Entity Framework Core, async methods like ToListAsync(), FirstOrDefaultAsync(), and SaveChangesAsync() prevent blocking the request thread.
         * 
         *  If you don’t use await, the method completes instantly and returns a Task<int>, without waiting for the delay to finish.
         */
        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "filterOccupancy")]int?occupancy
            , [FromQuery] string? search, [FromQuery] int pageSize = 2, [FromQuery] int pageNumber = 1)
        {
            try
            {
                IEnumerable<Villa> villaList;
                if (occupancy > 0)
                {
                    villaList = await _dbVilla.GetAllAsync(u => u.Occupancy == occupancy, pageSize:pageSize, pageNumber:pageNumber);
                }
                else
                {
                    villaList = await _dbVilla.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);
                }
                //add search by name or amenity, done in coding not on db
                if (!string.IsNullOrEmpty(search))
                {
                    villaList = villaList.Where(u => u.Name.ToLower().Contains(search)
                    || u.Amenity.ToLower().Contains(search));
                }
               
                //logger.LogInformation("Getting all Villas");    //using logging
                _response.Result = mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }



        [HttpGet("{id:int}", Name = "GetVilla")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //define the multiple responsed that can be produced
        [ProducesResponseType(200, Type = typeof(VillaDTO))] //Ok + can specify the success model
        [ProducesResponseType(404)] //NotFound
        [ProducesResponseType(StatusCodes.Status400BadRequest)] //BadRequest, if you don't remeber the codes
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);  //'.' didn't find any villa with that id
                }
                _response.Result = mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;

        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]   
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }
                /*note when creating a villa, id must be 0
                if (villaDTO.Id > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                /*that's no longer needed, EF automatically set it*/
                //villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;


                //map villaDTO to villa, can use automapper instead
                /* Villa model = new()
                 {
                     Amenity = createDTO.Amenity,
                     //Id = villaDTO.Id, //will be populated automatically
                     Name = createDTO.Name,
                     Details = createDTO.Details,
                     Occupancy = createDTO.Occupancy,
                     Rate = createDTO.Rate,
                     Sqft = createDTO.Sqft,
                     ImageUrl = createDTO.ImageUrl
                 };
                 */
                //use mapper instead
                Villa villa = mapper.Map<Villa>(createDTO);  // <outputtype> (inputtype)

                await _dbVilla.CreateAsync(villa);  //this create and save together
                                                    //Now you have to save changes
                                                    //_db.SaveChanges();

                /*instead of just Ok, return the url with the actual resource created
                you need to use the Get method which takes the id, you can't pass its endpoint 3ltool except if you give it an explicit name

                 routeName (string, required) :The name of the route to generate the URL for.Usually matches the Name property of a route in a controller.

                routeValues (object, optional) The route parameters needed to construct the URL for the created resource. Typically includes the id of the newly created entity.

                value (object, optional) The response body containing the newly created resource. Usually the DTO or entity representation.
                 */
                _response.Result = mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;

        }


        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //since we added authorization, role-based access
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]   //if user not logged in
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // if user is logged in but not the role who can access
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(u => u.Id == id);

                if (villa == null)
                {
                    return NotFound();
                }
                await _dbVilla.RemoveAsync(villa);   //not we don't have a remove async
                                                     //await _db.SaveChangesAsync();
                                                     //done successfully but when we delete, we don't return anything

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;

        }


        [HttpPut("{id:int}", Name = "UpdateVilla")] //update the whole record, common
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                //first check for possible errors
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }
                /* you don't need all of this, EF smart to figure out what record you need to update
                //then retrieve the obj
                var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
                if (villa == null)
                {
                    return BadRequest();
                }
                villa.Name = villaDTO.Name;
                villa.Sqft = villaDTO.Sqft;
                villa.Occupancy = villaDTO.Occupancy;
                */

                //map villaDTO to villa
                /*Villa model = new()
                {
                    Amenity = villaDTO.Amenity,
                    Id = villaDTO.Id,
                    Name = villaDTO.Name,
                    Details = villaDTO.Details,
                    Occupancy = villaDTO.Occupancy,
                    Rate = villaDTO.Rate,
                    Sqft = villaDTO.Sqft,
                    ImageUrl = villaDTO.ImageUrl
                };*/

                //use mapper instead
                Villa model = mapper.Map<Villa>(updateDTO);  // <output> input

                await _dbVilla.UpdateAsync(model);
                //await _db.SaveChangesAsync();
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]  //update one of the fields only
        [Consumes("application/json-patch+json")]
        public async Task<IActionResult> UpdatePartialVilla(int id, [FromBody] JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            //you must add the AsNoTracking part to avoid the exception of tracking
            var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);
            if (villa == null)
            {
                return BadRequest();
            }

            // Convert Villa to VillaDTO 
            /*VillaUpdateDTO villaDTO = new ()
            {
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Amenity = villa.Amenity,
                Id = villa.Id,
                Details = villa.Details,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
                ImageUrl = villa.ImageUrl
            };*/
            //use mapper instead
            VillaUpdateDTO villaDTO = mapper.Map<VillaUpdateDTO>(villa);  // <output> input

            // the jsonPatchDocument obj has what is to be updated, so just apply it on the villa obj

            // Apply the patch with error handling to be stored in ModelState'new 9'
            patchDTO.ApplyTo(villaDTO, ModelState);

            //to update, convert back to villa
            /*
            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                Details = villaDTO.Details,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
                ImageUrl = villaDTO.ImageUrl
            };*/
            //use mapper instead
            Villa model = mapper.Map<Villa>(villaDTO);  // <output> input

            await _dbVilla.UpdateAsync(model);
            //await _db.SaveChangesAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return NoContent();

        }








    }
}