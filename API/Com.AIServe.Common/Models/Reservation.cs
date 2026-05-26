using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.AIServe.Common.Models;

public class Reservation
{
    [Key]
    public int Id { get; set; }

    [MaxLength(32)]
    public string? Uid { get; set; }

    [ForeignKey(nameof(Uid))]
    public User? User { get; set; }

    [Required]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    public DateTime ReservationTime { get; set; }

    public int ServiceType { get; set; }

    [MaxLength(500)]
    public string? Remark { get; set; }

    public int Status { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }
}
