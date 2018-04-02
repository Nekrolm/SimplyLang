using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLang.Optimizations
{
    public interface IVertex<T>{
        IEnumerable<IVertex<T>> Next();
        IEnumerable<IVertex<T>> Prev();
        ISet<T> In();
        ISet<T> Out();
        bool UpdateIn(ISet<T> newIn);
        bool UpdateOut(ISet<T> newIn);
    }

    public interface ITransferer{
        ISet<T> Transfer<T>(ISet<T> X, IVertex<T> V);
    }

    public interface ICombiner{
        ISet<T> Combine<T>(IEnumerable<ISet<T>> In);
    }


    public static class Semilattice
    {
        public static void IterAlgoForward <T>( IEnumerable<IVertex<T>> vertexes, ITransferer tr, ICombiner comb)
        {
            bool ok = true;

            while (ok){
                ok = false;
                foreach (var v in vertexes){
                    ok |= v.UpdateIn(tr.Transfer(comb.Combine(v.Prev().Select(vv=>vv.Out())), v));
                }
            }
        }

        public static void IterAlgoBackward<T>(IEnumerable<IVertex<T>> vertexes, ITransferer tr, ICombiner comb)
        {
            bool ok = true;

            while (ok)
            {
                ok = false;
                foreach (var v in vertexes)
                {
                    ok |= v.UpdateOut(tr.Transfer(comb.Combine(v.Next().Select(vv => vv.In())), v));
                }
            }
        }

    }
}
