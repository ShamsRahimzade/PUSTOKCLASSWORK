using PustokSH.Model;

namespace PustokSH.ViewModels
{
    public class HomeVM
    {
       
        
            public List<Slide> Slides { get; set; }
        public List<Feature> Features { get; set; }

        public List<Book> Books { get; set; }

        public List<Book> Discountbooks { get; set; }
        public List<Book> Expensivebooks { get; set; }
        public List<Book> Newbooks { get; set; }
    }
}
