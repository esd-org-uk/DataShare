using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.Domain.Interface
{
    public interface ISqlTableUtility
    {
        void DropTable(string tablename);

        /// <summary>
        /// clean up tool for ver 1.4 as tables are not being dropped when the tables are deleted from datashare
        /// can be used to delete tables which exist in the current datasharecontext
        /// </summary>
        void DropTables(List<string> tableNames);

        List<string> GetUnusedDSTables();


    }
}
