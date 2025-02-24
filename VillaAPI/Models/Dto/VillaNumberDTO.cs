using System.ComponentModel.DataAnnotations;

namespace VillaAPI.Models.Dto
{
    /*Note we have to make different DTOs for update and create
     * create we don't need the id, update we need the id, to know which record to update
     * Image we can ask for it in case of update, not necsessary for create
     */
    public class VillaNumberDTO
    {
        [Required]
        public int VillaNo { get; set; }
        public string SpecialDetails { get; set; }
    }

}
