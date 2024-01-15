//using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model.db
{
    public class User
    {
        //[PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int Group { get; set; }

        public string Name { get; set; }
    }
}
