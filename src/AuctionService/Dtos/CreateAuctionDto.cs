using System.ComponentModel.DataAnnotations;

namespace AuctionService.Dtos;

public class CreateAuctionDto
{
    [Required]
    public string Make { get; set; } = default!;

    [Required]
    public string Model { get; set; } = default!;

    [Required]
    public int Year { get; set; }

    [Required]
    public string Color { get; set; } = default!;

    [Required]
    public int Mileage { get; set; }

    [Required]
    public string ImageUrl { get; set; } = default!;

    [Required]
    public int ReservePrice { get; set; }

    [Required]
    public DateTime AuctionEnd { get; set; }
}
