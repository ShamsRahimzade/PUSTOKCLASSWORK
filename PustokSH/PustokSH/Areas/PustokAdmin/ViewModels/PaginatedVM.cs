namespace PustokSH.Areas.PustokAdmin.ViewModels
{
    public class PaginatedVM<T>
    {
        public double TotalPage { get; set; }
        public int CurrentedPage { get; set; }
        public List<T> items { get; set; }
    }
}
