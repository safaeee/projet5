using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
namespace ConsoleConcurency
{
    class Program
    {
        static void Main(string[] args)
        {
            //L'utilisateur 1 et l'utilisateur 2 tentent tous les deux de mettre à jour la même entité(update)
            Employe e1 = null, e2 = null;
            using (var ctx = new CONCURENSYEntities()) { e1 = getEmploye(ctx, 1); }
            using (var ctx = new CONCURENSYEntities()) { e2 = getEmploye(ctx, 1); }

            e1.salaire = 20000;
            e2.salaire = 30000;

            try { using (var ctx = new CONCURENSYEntities()) { saveEmploye(ctx, e1); } }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Ereeur concurence user 1" + ex.Message);
            }
            try { using (var ctx = new CONCURENSYEntities()) { saveEmploye(ctx, e2); } }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Ereeur concurence user 2" + ex.Message);

            }
            //L'utilisateur 1 et l'utilisateur 2 tentent tous les deux de supprimer la même entité (delete)
            using (var ctx = new CONCURENSYEntities()) { e1 = RemoveEmploye(1); }
            using (var ctx = new CONCURENSYEntities()) { e2 = RemoveEmploye("Aloui"); }

            try { using (var ctx = new CONCURENSYEntities()) { saveEmploye(ctx, e1); } }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Ereeur concurence user 1" + ex.Message);
            }
            try { using (var ctx = new CONCURENSYEntities()) { saveEmploye(ctx, e2); } }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Ereeur concurence user 2" + ex.Message);

            }
            //L'utilisateur 1 essaie de mettre à jour la même entité (update) où l'utilisateur 2 l'a supprimée (delete)
            using (var ctx = new CONCURENSYEntities()) { e1 = getEmploye(ctx, 1); }
            using (var ctx = new CONCURENSYEntities()) { e2 = RemoveEmploye("Aloui"); }
            e1.nom = "saidi";
            try { using (var ctx = new CONCURENSYEntities()) { saveEmploye(ctx, e1); } }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Ereeur concurence user 1" + ex.Message);
            }
            try { using (var ctx = new CONCURENSYEntities()) { saveEmploye(ctx, e2); } }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Ereeur concurence user 2" + ex.Message);
            }
           //L'utilisateur 1 a demandé à lire (select) une entité et l'utilisateur 2 l'a mise à jour après l'avoir lu
            using (var ctx = new CONCURENSYEntities()) { e1 = getEmploye(ctx, 1); }
            using (var ctx = new CONCURENSYEntities()) { e2 = getEmploye(ctx, 1); }
            using (var ctx = new CONCURENSYEntities()) {
                e1.nom = "saidi";
                try { using (var ct = new CONCURENSYEntities()) { saveEmploye(ctx, e1); } }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("Ereeur concurence user 1" + ex.Message);
                }
                try { using (var ct = new CONCURENSYEntities()) { saveEmploye(ct, e2); } }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("Ereeur concurence user 2" + ex.Message);

                }
                Console.WriteLine(getEmploye(ctx, 1).nom);
                Console.ReadKey();

            }
              

           
        }
        public static Employe getEmploye(CONCURENSYEntities ctx, int id)
        {
            Employe emp = ctx.Employes.Where(e => e.id == id).Single();
            return emp;
        }
        public static Employe RemoveEmploye(int id)
        {
            using (CONCURENSYEntities Context = new CONCURENSYEntities())
            {
                Employe emp = Context.Employes.Find(id);
                Context.Employes.Remove(emp);
                Context.SaveChanges();
                return emp;
            }
        }
        public static Employe RemoveEmploye(string nom)
        {
            using (CONCURENSYEntities Context = new CONCURENSYEntities())
            {
                Employe emp = Context.Employes.Find(nom);
                Context.Employes.Remove(emp);
                Context.SaveChanges();
                return emp;
            }
        }
           public static void saveEmploye(CONCURENSYEntities ctx, Employe emp)
           {
               ctx.Entry(emp).State = EntityState.Modified;
               ctx.SaveChanges();

           }
       
        }
    }

