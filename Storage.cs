using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessments
{
    class Storage
    {
        #region Class Fields and Properties
        private string currentClassId;
        private List<string> tempClassIdList = new List<string>();
        private List<string> tempObjectivesList = new List<string>();
        private List<string> tempObjectiveWDescList = new List<string>();
        private List<Student> tempStudentObjectList = new List<Student>();
        private List<int> activeStudentObjectIndices = new List<int>();
        private List<int> activeObjectiveIndices = new List<int>();
        private int activeRosterLength;

        //for fourth group addition
        private List<string> currentChapters = new List<string>();
        public List<string> CurrentChapters
        {
            get { return currentChapters; }
            set { currentChapters = value; }
        }

        private List<string> curChapObjectives;
        public List<string> CurChapObjectives
        {
            get { return curChapObjectives; }
            set { curChapObjectives = value; }
        }


               
        public string CurrentClassID
        {
            get { return currentClassId; }
            set { currentClassId = value; }
        }

        public List<string> TempClassIDList
        {
            get { return tempClassIdList; }
            set { tempClassIdList = value; }
        }

        public List<string> TempObjectivesList
        {
            get { return tempObjectivesList; }
            set { tempObjectivesList = value; }
        }

        public List<string> TempObjAndDescList
        {
            get { return tempObjectiveWDescList; }
            set { tempObjectiveWDescList = value; }
        }

        public List<Student> TempStudentObjectList
        {
            get { return tempStudentObjectList; }
            set { tempStudentObjectList = value; }
        }

        public List<int> ActiveStudentObjectIndices
        {
            get { return activeStudentObjectIndices; }
            set { activeStudentObjectIndices = value; }
        }

        public List<int> ActiveObjectiveIndices
        {
            get { return activeObjectiveIndices; }
            set { activeObjectiveIndices = value; }
        }

        public int ActiveRosterLength
        {
            get { return activeRosterLength; }
            set { activeRosterLength = value; }
        }
        #endregion

        #region To Update File Data
        /// <summary>
        /// Used when adding a new learning objective through the form window.  
        /// Method will append necessary objective string data to new line at the 
        /// end of current existing objective data file.
        /// </summary>
        /// <param name="o"> New learning Objective object to add to data file.</param>
        public void AddObjectiveToFile(Objective o)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"../../Write-Read/Objectives.txt", true))
            {
                file.WriteLine(o.ToStringForFile());
            }
        }

        /// <summary>
        /// Used when adding new students through the form window.  
        /// Method will append current students data file to include new student's 
        /// necessary information to read and write to when future updates are made.
        /// </summary>
        /// <param name="student">student object to add to existing students data file</param>
        public void AddStudentToFile(Student student)
        {
            TempStudentObjectList.Add(student);
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"../../Write-Read/Students.txt", true))
            {
                file.WriteLine(student.ToStringAddNewStudent());
            }
        }

        /// <summary>
        /// Writes passed string[] array to students.txt file.  Will overwrite any current data. 
        /// Typically method is used when user adds a completed or parital objective to a student or students.
        /// </summary>
        /// <param name="strings">string array to be saved</param>
        public void UpdateStudentsFile(string[] strings)
        {
            System.IO.File.WriteAllLines(@"../../Write-Read/Students.txt", strings);
        }

        /// <summary>
        /// Updates ClassID.txt file to reflect newest data passed in
        /// </summary>
        /// <param name="idString">most recent class id to add to file</param>
        public void AddClassIdToFile(string idString)
        {
                bool okToAdd = false;
                //classIdlistGenerated at beginning of form launch so tempClassIDlist should contain current data.
            
                int count = TempClassIDList.Count;
                    foreach (string s in TempClassIDList)
                    {
                        if (s == idString.Trim())
                        {
                            okToAdd = false;
                            break;
                        }
                        else
                        {
                            okToAdd = true;
                        }
                    //break;
                    }
                if (okToAdd == true)
                {

                    using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(@"../../Write-Read/ClassID.txt", true))
                    {
                        file.WriteLine(idString);
                    }
                    TempClassIDList.Add(idString);
                }
                
            
        }
        #endregion

        #region To Get Data From File
        /// <summary>
        /// Retrieves List of strings containing current learning objectives stored in objectives.txt file.
        /// Always called on initial form load and after a new objective has been added by user through the form.
        /// </summary>
        public void GetObjectivesFromFile()
        {
            List<string> objectiveList = new List<string>();
            List<string> objAndDescList = new List<string>();
            List<int> activeObjectiveList = new List<int>();

            string[] lines = System.IO.File.ReadAllLines(@"../../Write-Read/Objectives.txt");
            int linesCount = lines.Length;
            char[] delimiter = new char[] { '*' };
            for (int i = 0; i < linesCount; ++i)
            {
                string[] splitHolder;
                string tempLine = lines[i];
                splitHolder = tempLine.Split(delimiter, 2);
                objectiveList.Add(splitHolder[0]);
                if (splitHolder[1] != "/*")
                {
                    splitHolder[1] = splitHolder[1].Remove(splitHolder[1].Length - 1);
                    objAndDescList.Add(splitHolder[0] + " : " + splitHolder[1]);
                }
                else
                {
                    objAndDescList.Add(splitHolder[0]);
                }
                activeObjectiveList.Add(i);
            }
            TempObjectivesList = objectiveList;
            TempObjAndDescList = objAndDescList;
            ActiveObjectiveIndices = activeObjectiveList;
        }

        public void DetermineActiveObjectiveIndicies(string[] chapters)
        {
            ActiveObjectiveIndices.Clear();
            int i = 0;
            char[] delimiter = new char[] { '.' };

            foreach (string id in TempObjectivesList)
            {
                bool keep = false;
                string[] temp1a = id.Split(delimiter);
                string temp1s = temp1a[0];

                foreach (string chap in chapters)
                {
                    //string temp = chap + ".";
                    string[] temp2a = chap.Split(delimiter);
                    string temp2s = temp2a[0];

                    //if (id.Contains(temp))

                    if (temp1s == temp2s)
                    {
                        keep = true;
                        break;
                    }
                }
                if (keep == true)
                {
                    ActiveObjectiveIndices.Add(i);
                }
                ++i;
            }
        }

        

        

        /// <summary>
        /// Retrieves List of strings containing current class ids stored in classid.txt file.
        /// Always called on initial form load and after a new class has been added by user through the form.
        /// </summary>
        public void GetClassIdsFromFile()
        {
            string[] lines = System.IO.File.ReadAllLines(@"../../Write-Read/ClassID.txt");
            List<string> classIdList = new List<string>();
            classIdList = lines.ToList();
            TempClassIDList = classIdList;
        }
        #endregion

        /// <summary>
        /// Creates unique instances of student objects using current data from students.txt file.
        /// One student object instance for each student line in file, regardless of section.
        /// </summary>
        public void CreateStudentObjects()
        {
            List<Student> students = new List<Student>();
            string[] lines = System.IO.File.ReadAllLines(@"../../Write-Read/Students.txt");
            List<string> studentsTxtFileStrings = lines.ToList<string>();
            int count = studentsTxtFileStrings.Count;
            char[] delimiter = new char[] { '*' };
            for (int i = 0; i < count; ++i)
            {
                string[] splitHolder;
                string tempLine = studentsTxtFileStrings[i];
                splitHolder = tempLine.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                int length = splitHolder.Length;
                string fullName = splitHolder[0];
                string section = splitHolder[1];

                //separate name string into first and last
                char[] delimiter2 = new char[] { ' ' };
                string[] nameHolder = fullName.Split(delimiter2, 2, StringSplitOptions.RemoveEmptyEntries);
                string fName = nameHolder[0]; string lName = nameHolder[1];
                //create student object using first name, last name, and section
                Student current = new Student(fName, lName, section);
                for (int j = 2; j < length; ++j)
                {
                    string tempString = splitHolder[j];
                    Objective tempObj = new Objective(tempString);
                    if (tempString.Contains("p"))
                    {
                        char[] delimiter3 = new char[] { 'p' };
                        string[] partialHolder = tempString.Split(delimiter3, 1, StringSplitOptions.RemoveEmptyEntries);
                        partialHolder[0] = partialHolder[0].Remove(partialHolder[0].Length - 1);
                        tempObj = new Objective(partialHolder[0]);
                        current.PartialComplete(tempObj);
                    }
                    else
                    {
                        current.CompletedObjective(tempObj);
                    }
                }
                students.Add(current);
            }
            TempStudentObjectList = students;
        }

        /// <summary>
        /// Will populate a list of integers containing the indicies of specific class section student object's position in TempStudentObjectList.
        /// Will be used to reference specific objects when populating listboxes and writing data to file.
        /// Goal is to "skip over" the student objects who are NOT in the class section but still maintaining their integrity to write all students to file during updates.
        /// </summary>
        /// <param name="classSection"></param>
        public void DetermineActiveStudents(string classSection)
        {
            List<int> tempIndicies = new List<int>();
            int count = TempStudentObjectList.Count();
            for (int i = 0; i < count; ++i)
            {
                if (TempStudentObjectList[i].SectionID == CurrentClassID)
                {
                    tempIndicies.Add(i);
                }
            }
            ActiveRosterLength = tempIndicies.Count();//stores number of students who belong to currentclassID.
            ActiveStudentObjectIndices = tempIndicies;

       
        }

        #region UNUSED CODE
        ////METHOD BELOW IS NOT BEING USED!
        ///// <summary>
        ///// Appends objectives.txt file to add newest data passed in
        ///// </summary>
        ///// <param name="strings">most recent objective data 'list'</param>
        //public void AddObjectiveToFile(string objString)
        //{
        //    using (System.IO.StreamWriter file =
        //    new System.IO.StreamWriter(@"../../_Write-Read/Objectives.txt", true))
        //    {
        //        file.WriteLine(objString);
        //    }
        //    TempObjectivesList.Add(objString);
        //}
        ////METHOD BELOW IS NOT BEING USED RIGHT NOW!
        //public void UpdateStudents(List<Student> passedStudentObjList)
        //{
        //    string[] lines = System.IO.File.ReadAllLines(@"../../Write-Read/Students.txt");
        //    List<string> retrievedStudentList = lines.ToList<string>();
        //    List<string> newList = new List<string>();
        //    int retrievedListLength = retrievedStudentList.Count;
        //    int passedListLength = passedStudentObjList.Count;
        //    foreach (string s in retrievedStudentList)
        //    {
        //        foreach (Student sObj in passedStudentObjList)
        //        {
        //            string temp = null;
        //            if (s.Contains(sObj.ToStringFirstLast()))
        //            {
        //                temp = sObj.ToStringForFile();
        //            }
        //            else
        //            {
        //                temp = s;
        //            }
        //            newList.Add(temp);
        //        }
        //    }
        //    string[] newListArray = newList.ToArray();
        //    UpdateStudentsFile(newListArray);
        //}
        ////METHOD BELOW NOT BEING USED
        ///// <summary>
        ///// Retrieves list of first and last name of all students in students.txt file
        ///// </summary>
        ///// <returns>list of string elements, each item containing first and last name of student</returns>
        //public void GetNameRoster()
        //{
        //    List<string> specificRoster = new List<string>();
        //    string[] lines = System.IO.File.ReadAllLines(@"../../_Write-Read/Students.txt");
        //    List<string> retrievedRoster = lines.ToList<string>();

        //    int linesCount = retrievedRoster.Count;
        //    char[] delimiter = new char[] { '*' };
        //    for (int i = 0; i < linesCount; ++i)
        //    {
        //        string[] splitHolder;
        //        string tempLine = retrievedRoster[i];
        //        splitHolder = tempLine.Split(delimiter, 1);
        //        specificRoster.Add(splitHolder[0]);
        //    }
        //    TempRosterList = specificRoster;
        //}
        ////METHOD BELOW IS NOT BEING USED!

        ////public List<string> ToFirstAndLastName(List<string> listItems)
        ////{
        ////    List<string> fAndLNameList = new List<string>();
        ////    //fAndLName = firstName.Trim() + " " + this.lastName.Trim() + "*" + sectionId.Trim();
        ////    //return fAndLName;
        ////    //System.IO.File.Red
        ////    int limit = listItems.Count;
        ////    char[] delimiter = new char[] { '*' };
        ////    for (int i = 0; i < limit; ++i)
        ////    {
        ////        string[] splitHolder;
        ////        string tempLine = listItems[i];
        ////        splitHolder = tempLine.Split(delimiter, 1);
        ////        string specificFAndLName = splitHolder[0];
        ////        fAndLNameList.Add(specificFAndLName);
        ////    }
        ////    return fAndLNameList;
        ////}

        //public void GenerateActiveStudentList()
        //{
        //    List<Student> active = new List<Student>();
        //    foreach (Student s in TempStudentObjectList)
        //    {
        //        if (s.SectionID == CurrentClassID)
        //        {
        //            active.Add(s);

        //        }
        //    }
        //}

        ///// <summary>
        ///// Generates string List of student names (first and last) who belong to a given class id.
        ///// Updates the storage object's temporary roster list to include generated list.
        ///// </summary>
        ///// <param name="classIdent">class identifier to retrieve roster for</param>
        //public void GetNameRoster(string classIdent)
        //{
        //    List<string> specificRoster = new List<string>();
        //    string[] lines = System.IO.File.ReadAllLines(@"../../Write-Read/Students.txt");
        //    List<string> retrievedRoster = lines.ToList<string>();

        //    int linesCount = retrievedRoster.Count;
        //    char[] delimiter = new char[] { '*' };
        //    for (int i = 0; i < linesCount; ++i)
        //    {
        //        string[] splitHolder;
        //        string tempLine = retrievedRoster[i];
        //        splitHolder = tempLine.Split(delimiter, 3);
        //        //need try catch here???
        //        string classIdSpot = splitHolder[1];
        //        if (classIdSpot == classIdent && classIdSpot != null)
        //        {
        //            specificRoster.Add(splitHolder[0]);
        //        }
        //    }
        //    TempRosterList = specificRoster;
        //}

        #endregion
    }
}
