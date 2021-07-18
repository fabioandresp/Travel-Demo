using System.Linq;

namespace Travel.Model.Context
{
    public static class TDBInitializer
    {
        public static void Initialize(TDBContext context)
        {
            context.Database.EnsureCreated();

            if (context.Author.Any())
            {
                return;
            }

            var authors = new Author[]
            {
                new Author{Id=10, Name="Xavi", LastName="San Martín"},
                new Author{Id=11, Name="Pablo", LastName="Venegas"},
            };

            foreach (var author in authors)
            {
                context.Author.Add(author);
            }
            context.SaveChanges();

        }
    }
}
