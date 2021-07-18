
using Travel.Model.Base;

namespace Travel.Model.RelationShip
{
    [RelationShip(nameof(Model.Author),nameof(Model.Book))]
    [UniqueKey(nameof(Author_ID), nameof(Book_ISBN))]
    class Author_Book_Relation
    {
        [TravelDescription("Autor_ID")]
        public int Author_ID { get; set; }


        [TravelDescription("Libro_ISBN")]
        public int Book_ISBN { get; set; }
    }
}
