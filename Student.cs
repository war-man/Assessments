using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessments
{
    class Student
    {

        #region Class Fields and Properties
        private string firstName;
        private string lastName;
        private string sectionId;
        private List<Objective> completed = new List<Objective>();
        private List<Objective> partial = new List<Objective>();

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string SectionID
        {
            get { return sectionId; }
            set { sectionId = value; }
        }

        public List<Objective> Completed
        {
            get { return completed; }
            //set { completed = value; }
        }

        public List<Objective> Partial
        {
            get { return partial; }
            //set { partial = value; }
        }
        #endregion

        #region Student Object Constructor(s)
        public Student(string fName, string lName, string id)
        {
            firstName = fName.Trim();
            lastName = lName.Trim();

            if (id == null || id == "")
            {
                sectionId = " ";
            }
            else
            {
                sectionId = id.Trim();
            }
        }

        public Student(string fName, string lName)
        {
            firstName = fName.Trim();
            lastName = lName.Trim();
            sectionId = " ";
        }
        #endregion

        #region ToString Methods
        /// <summary>
        /// Creates one string containing student firstname, lastname, section id, AND
        /// each objective completed or partially completed. Every item after lastname is 
        /// separated by asterisk symbols to be used to parse data when reading back from file.
        /// </summary>
        /// <returns>single string with all student data in the form 
        /// "fName LName*classid*completeObjIds*partialObjIds*"</returns>
        public string ToStringForFile()
        {
            string uniqueStudentString;
            string fAndL = ToStringFirstLastForFile();
            string section = "*" + this.sectionId + "*";
            string completes = null; string partials = null;
            foreach (Objective o in completed)
            {
                completes += o.ToString() + "*";
            }
            foreach (Objective p in partial)
            {
                partials += p.ToString() + "p*";
            }
            uniqueStudentString = fAndL + section + completes + partials;
            return uniqueStudentString;
        }

        /// <summary>
        /// Generates first and last name to be used in writing data to file.  If no last name exists, 
        /// this method will include the default "/" symbol as a placeholder for last name value as
        /// this is necessary to ensure correct parsing during future loading from .txt file.
        /// </summary>
        /// <returns>student's first and last name</returns>
        public string ToStringFirstLastForFile()
        {
            string fAndLName;
            fAndLName = this.firstName + " " + this.lastName;
            return fAndLName;
        }

        /// <summary>
        /// Generates student name for proper user display.  If no last name exists, 
        /// will generate first name only.
        /// </summary>
        /// <returns>Student name</returns>
        public string ToStringFirstLast()
        {
            string fAndLName;
            if (this.LastName == "/")
            {
                fAndLName = this.firstName;
            }
            else
            {
                fAndLName = this.firstName + " " + this.lastName;
            }
            return fAndLName;
        }
        
        /// <summary>
        /// Generates initial string necessary to add a new student to data file.  
        /// Includes student first and last name and section id (separated by asterisks).
        /// </summary>
        /// <returns>necessary string to write to file for student data 
        /// (prior to acknowledging any objective completions)</returns>
        public string ToStringAddNewStudent()
        {
            string fAndLName;
            fAndLName = this.firstName + " " + this.lastName + "*" + this.SectionID + "*";
            return fAndLName;
        }
        #endregion

        #region Objective Completion Events
        /// <summary>
        /// Adds an objective object to an individual student object's completed objective list.
        /// </summary>
        /// <param name="o">objective student has completed</param>
        public void CompletedObjective(Objective o)
        {
            completed.Add(o);
        }

        /// <summary>
        /// Adds an objective object to an individual student object's partially completed objective list.
        /// </summary>
        /// <param name="o">objective student has partially completed</param>
        public void PartialComplete(Objective o)
        {
            partial.Add(o);
        }
        #endregion
    }
}
