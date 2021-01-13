using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NetflixComplete.Portable;

namespace NetflixComplete.WebApi.Controllers
{
    public class CategoriesController : ApiController
    {
        private static List<Category> _categories = null;
        // GET: api/Categories
        public IEnumerable<Category> Get()
        {
            if (_categories == null)
            {
                _categories = new List<Category>();

                var path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Categories.txt";
                using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        string categoryLine = reader.ReadLine();
                        var categoryArray = categoryLine.Split(':');
                        string categoryName = categoryArray[0];
                        long categoryNumber = long.Parse(categoryArray[1]);

                        Category category = new Category { Id = categoryNumber, Name = categoryName };
                        _categories.Add(category);
                    }
                }
            }

            return _categories;
        }

        // GET: api/Categories/5
        public Category Get(long id)
        {
            return Get().Where(c => c.Id == id).FirstOrDefault();
        }
    }

    public class Category
    {
        public string Name { get; set; }
        public long Id { get; set; }
    }
}
