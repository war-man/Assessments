using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessments
{
    class Roster
    {
        //class fields and properties

        private string classId;
        private List<Student> roster;
        private string classDescription;

        public string ClassName
        {
            get { return classId; }
            set { classId = value; }
        }

        public List<Student> Roster
        {
            get { return roster; }
            set { roster = value; }
        }

        public string ClassDescription
        {
            get { return classDescription; }
            set { classDescription = value; }
        }

        //class Constructor
        public Roster(string className)
        {
            classId = className;
        }

        /// <summary>
        /// Adds a student object to a section object's Class Roster Field
        /// </summary>
        /// <param name="toAdd">Student object to add to roster</param>
        public void addStudent(Student toAdd)
        {
            roster.Add(toAdd);
        }

        /// <summary>
        /// Determines which Student objects in the Section contain 'completed', 'partial' or none status in their respective object field lists.
        /// partialPass and notPassed represent out values while the actual return object is string of names of those who have definitively passed.
        /// </summary>
        /// <param name="o1">objective to "search" for</param>
        /// <returns>List of strings containing the names of student objects which contain objective achievement</returns>
        public List<string> HasAchieved(Objective o1, out List<string> partialPass, out List<string> notPassed)
        {
            List<string> passed = new List<string>();
            notPassed = new List<string>();
            partialPass = new List<string>();
            int length = roster.Count;
            for (int i = 0; i < length; ++i)
            {
                if (roster[i].Completed.Contains(o1))
                {
                    passed.Add(roster[i].ToString());
                }
                else if (roster[i].Partial.Contains(o1))
                {
                    partialPass.Add(roster[i].ToString());
                }
                else
                {
                    notPassed.Add(roster[i].ToString());
                }
            }
            return passed;
        }

       

        public void sortRoster()
        {
            List<Student> currentRoster;
            
        //classRoster.Sort();
        }



    }
}

