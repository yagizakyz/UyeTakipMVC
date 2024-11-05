using UyeTakipMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace UyeTakipMVC.Repositories
{
    public class GenericRepository<T> where T : class, new()
    {
        UyeTakipContext c = new UyeTakipContext();

        public List<T> TList()
        {
            return c.Set<T>().ToList();
        }
        
        public List<T> TList(string p)
        {
            return c.Set<T>().Include(p).ToList();
        }

        public void TAdd(T cc)
        {
            c.Set<T>().Add(cc);
            c.SaveChanges();
        }

        public void TDelete(T cc)
        {
            c.Set<T>().Remove(cc);
            c.SaveChanges();
        }

        public void TUpdate(T cc)
        {
            c.Set<T>().Update(cc);
            c.SaveChanges();
        }

        public T TGet(int id)
        {
            return c.Set<T>().Find(id);
        }

        public T TGet(string id)
        {
            return c.Set<T>().Find(id);
        }
    }
}
