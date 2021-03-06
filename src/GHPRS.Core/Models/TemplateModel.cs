using Microsoft.AspNetCore.Http;

namespace GHPRS.Core.Models
{
    public class TemplateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Frequency { get; set; }
        public decimal Version { get; set; }
        public IFormFile File { get; set; }
    }
}
