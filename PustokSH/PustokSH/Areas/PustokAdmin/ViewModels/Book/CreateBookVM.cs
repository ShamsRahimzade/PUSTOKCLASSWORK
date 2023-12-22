using PustokSH.Model;

namespace PustokSH.Areas.PustokAdmin.ViewModels
{
    public class CreateBookVM
    {
        public string Name { get; set; }
        public int Page { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Discount { get; set; }
        public string Desc { get; set; }
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
		public IFormFile MainPhoto { get; set; }
		public IFormFile HoverPhoto { get; set; }
		public List<IFormFile>? Photos { get; set; }
		public List<Author>? authors { get; set; }
		public List<Genre>? genres { get; set; }
        public List<int>? tagIds { get; set; }
        public List<Tag>? tags { get; set; }

	}
}
