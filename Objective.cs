using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessments
{
    class Objective
    {
        #region Class Fields and Properties
        private string description;
        private string identifier;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }
        #endregion

        #region Objective Constructors
        public Objective (string id, string desc = null)
        {
            identifier = id.Trim();
            description = desc.Trim();
        }

        public Objective(string id)
        {
            identifier = id.Trim();
        }

        /// <summary>
        /// Will generate string representing objective object's class identifier value;
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Identifier;
            //return identifier;  this was changed on 11/23/16 to line above.
        }

        public string ToStringForFile()
        {
            string full = Identifier + "*" + Description + "*";
            return full;
        }

        #endregion
    }
}
