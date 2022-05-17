using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    internal static class GridViewExtensions
    {
        public static List<T> GetSelectedItems<T>(this GridView view) where T : class
        {
            List<T> toreturn = new List<T>();
            foreach (var row in view.GetSelectedRows())
            {
                if (view.IsGroupRow(row))
                {
                    continue;
                }

                if (view.GetRow(row) is T obj)
                {
                    toreturn.Add(obj);
                }
            }

            return toreturn;
        }
    }
}
