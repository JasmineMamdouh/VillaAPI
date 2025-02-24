using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using VillaAPI.Models;
using VillaAPI.Models.Dto;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers
{
    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IMapper mapper;

        //get the dbContext by dependency injection
        public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper _mapper)
        {
            //_db = db;
            _dbVillaNumber = dbVillaNumber;
            mapper = _mapper;
            this._response = new();
        }


        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync();
                _response.Result = mapper.Map<List<VillaNumberDTO>>(villaNumberList);
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


        [HttpGet( "{id:int}"  , Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {
                VillaNumber villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
                if (villaNumber == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberDTO villaNumberDTO)
        {
            try
            {
                if (villaNumberDTO == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                VillaNumber villaNo = mapper.Map<VillaNumber>(villaNumberDTO);
                await _dbVillaNumber.CreateAsync(villaNo);

                _response.Result = mapper.Map<VillaNumberDTO>(villaNo);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVillaNumber", new { id = villaNo.VillaNo }, _response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete ("{id:int}")]
        public async Task<ActionResult<APIResponse>> DeleteVilla (int id)
        {
            try
            {
                //first get the villa to be deleted, ensure it exists
                VillaNumber villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
                if (villaNumber == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response); //you must return in this case to avoid executing nxt instructions
                }
                await _dbVillaNumber.RemoveAsync(villaNumber);

                _response.StatusCode = HttpStatusCode.NoContent;
   
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [HttpPut ("{id:int}")]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaNumberDTO villaNumberDTO)
        {
            try
            {
                if(villaNumberDTO == null || id!= villaNumberDTO.VillaNo)
                {
                    return BadRequest();
                }
                VillaNumber villaNo = mapper.Map<VillaNumber>(villaNumberDTO);
                await _dbVillaNumber.UpdateAsync(villaNo);

                _response.StatusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

     }

    
            
    
}
