using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesMovie.ViewModels
{
    public class ImageUploadViewModel
    {
        public int Id { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
