using System.ComponentModel.DataAnnotations;

namespace DattingApp.API.Dtos;
public class UserForRegisterDto
{
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Username { get; set; }

    [Required]
    [StringLength(15, MinimumLength = 8, ErrorMessage = "Should be between 8 and 15 charcters.")]
    public string Password { get; set; }
}