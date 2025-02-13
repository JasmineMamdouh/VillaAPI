using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VillaAPI.Models.Dto;
using VillaAPI.Data;

namespace VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    //so that if contoller name is modified, you won't have to notify all consumers
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {

        /*let the return type be
         IActionResult -> flexible, std to return diff http status codes
         async Task<IActionResult> -> when returning varied HTTP status codes asynchronously.  --> Put, Patch, Delete
         ActionResult<T> -> strongly typed, enforce specific return type, either an object<T> or http respone  --> Get 'to know what model to return in success'
         async Task<ActionResult<T>> -> for async methods when using EF or external services  --> Post
        */

        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.villaList);  
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        //define the multiple responsed that can be produced
        [ProducesResponseType(200 , Type= typeof(VillaDTO))] //Ok + can specify the success model
        [ProducesResponseType(404)] //NotFound
        [ProducesResponseType(StatusCodes.Status400BadRequest)] //BadRequest, if you don't remeber the codes
        public ActionResult<VillaDTO> GetVilla(int id) 
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();  //'.' didn't find any villa with that id
            }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            //note when creating a villa, id must be 0
            if (villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);

            //instead of just Ok, return the url with the actual resource created
            //you need to use the Get method which takes the id, you can't pass its endpoint 3ltool except if you give it an explicit name
            /*
             routeName (string, required) :The name of the route to generate the URL for.Usually matches the Name property of a route in a controller.
            
            routeValues (object, optional) The route parameters needed to construct the URL for the created resource. Typically includes the id of the newly created entity.
            
            value (object, optional) The response body containing the newly created resource. Usually the DTO or entity representation.
             */
            return CreatedAtRoute("GetVilla",new { id = villaDTO.Id }, villaDTO);

        }
    }
}
