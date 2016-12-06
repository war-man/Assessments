using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assessments
{
    public partial class LearningTrackerWindow : Form

    {
        /*establishes instance of storage object at application inception.
        storage object is used to load currently existing data from file
        as well as hold, manipulate, and save any updates or changes that
        are made during user session (until form is closed).
        Storage object ensures all changes are always reflected in
        actual data files for saving during future form instantiations.
        */
        private Storage sto = new Storage();

        //used to determine if add objective or add student buttons have been clicked previously
        byte addObjectiveCounter = 0, addStudentCounter = 0;

        public LearningTrackerWindow()
        {
            InitializeComponent();
            ToDoOnLoad();
        }

        /// <summary>
        /// Contains sequence of methods to execute immediately upon generation of form
        /// </summary>
        private void ToDoOnLoad()
        {
            //load current class id list from file to storage object
            sto.GetClassIdsFromFile();
            //load current objective list from file to storage object
            sto.GetObjectivesFromFile();
            //create student objects based on current data in .txt file upon load
            //any new students created will be appended to end of tempstudentobjectlist in 
            //storage object
            sto.CreateStudentObjects();
            DisplayExistingClasses();
        }

        #region Load Class Roster & Objectives

        /// <summary>
        /// Executes when user clicks on "Class, LoadClass" submenu option.  Will display the 
        /// features to select an existing class from tempclassIdlist in storageobject.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiLoadClass_Click(object sender, EventArgs e)
        {
            DisplayExistingClasses();
            /*why didn't you just put everything that's in DisplayExistingClasses() in here?
             * Because I might need to have those things execute when the tsmiLoadClass button
             * is NOT clicked.  So I separated it to be called later if needed.
             */
        }

        /// <summary>
        /// Adds existing list of different class identifiers to appropriate list box
        /// and displays the select existing class groupbox (sets visibility to true).
        /// Will clear listbox at moment of method call, then populate new list of classes
        /// based on strings in storage object's tempClassIdList.
        /// </summary>
        private void DisplayExistingClasses()
        {
            lbClasses.Items.Clear();//to prevent classes from being duplicated in listbox.
            foreach (string s in sto.TempClassIDList) //classes are taken from tempClassIDList of storage object at this moment.
            {
                lbClasses.Items.Add(s);
            }
            if (grpSelectClass.Visible == false)
            {
                grpSelectClass.Visible = true;
            }
            grpAddClass.Visible = false;
        }

        /// <summary>
        /// When user clicks on "Go" button!  If button is clicked the selected class will become the "active" class!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectClass_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            if (lbClasses.SelectedIndex != -1) //if it is == -1 that means nothing in the box is selected and it won't do anything!
            {
                ClearAllGroupBoxes();
                string userSelect = lbClasses.SelectedItem.ToString();
                sto.CurrentClassID = userSelect;//establishes current selected item in class list box as currentclassID for storage object, which will drive everything with displaying students and updating data now!
                //storageObject.GetNameRoster(storageObject.CurrentClassID);//will generate list of student names who are only in selected class and store them in tempstudentlist of storage object.
                DisplayClassRosterPanel();
                scRosterDisplay.Visible = true;//set this line ahead of DisplayLearningObjectives due to bug in displaying in complete column.
                DisplayLearningObjectivesPanel();
                pnlGenerateGroups.Visible = true;
                //grpAddStudent.Visible = true;
            }
            else
            {
                lblFeedbackMessage.Text = "Please select an existing class to display.\nIf you would like to create a new class, please select the \"Create New\" option in the Class menu item.";
                lblFeedbackMessage.Visible = true;
            }
        }

        /// <summary>
        /// Populates listview box with learning objectives contained in storage object's tempobjanddesclist field.
        /// Clears contents of box before generating collection to display.
        /// </summary>
        private void DisplayLearningObjectivesPanel()
        {
            lvObjectives.Clear();
            lblFeedbackMessage.Visible = false;
            foreach (string o in sto.TempObjAndDescList)
            {
                lvObjectives.Items.Add(o);
            }
            //lvObjectives.Visible = true;
        }

        private void btnShowChaptObj_Click(object sender, EventArgs e)
        {
            int count = lvChapters.SelectedItems.Count;

        }

        /// <summary>
        /// Method will generate list of students who are in current selected class using the temporary student object
        /// list in sto object.  Each time this method is called, the indicies of the students in the active selected
        /// class are re-generated.
        /// </summary>
        private void DisplayClassRosterPanel()
        {
            lblFeedbackMessage.Visible = false;
            RePopulateActiveRoster();
            lvRoster.Clear();
            foreach (int index in sto.ActiveStudentObjectIndicies)
            {
                string tempFirst = sto.TempStudentObjectList[index].FirstName;
                string tempLast = sto.TempStudentObjectList[index].LastName;
                if (tempLast == "/")
                {
                    lvRoster.Items.Add(tempFirst);
                }
                else
                {
                    lvRoster.Items.Add(tempFirst + " " + tempLast);
                }
            }
            lblRosterClassHead.Text = sto.CurrentClassID + " Students";
           
        }

        /// <summary>
        /// Will set check mark value to "true" for each item in Roster listview box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAllStudents_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            if (scRosterDisplay.Visible == true)
            {
                int count = lvRoster.Items.Count;
                for (int i = 0; i < count; ++i)
                {
                    lvRoster.Items[i].Focused = true;
                    lvRoster.Items[i].Checked = true;
                }
            }
        }

        /// <summary>
        /// Will set check mark value to "false" for each item in Roster listview box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUnselectStudents_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            if (scRosterDisplay.Visible == true)
            {
                int count = lvRoster.Items.Count;
                for (int i = 0; i < count; ++i)
                {
                    lvRoster.Items[i].Focused = true;
                    lvRoster.Items[i].Checked = false;
                }
            }
        }

        /// <summary>
        /// Will re-generate activeStudentIndicies list in storage object for most current class Id.
        /// </summary>
        private void RePopulateActiveRoster()
        {
            sto.DetermineActiveStudents(sto.CurrentClassID);
        }

        /// <summary>
        /// Executes when the menuItem to add new class is clicked.  Group 'panel' with new class
        /// fields will display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiNewClass_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            grpAddClass.Visible = true;
            txtAddClass.Focus();
        }

        /// <summary>
        /// Executes when create class button is clicked or simulated by enter press.  Will execute only if 
        /// text exists in class name textbox.  Method updates data file to save new class name as well as updates
        /// 'select class' list box to reflect newly created class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddClass_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            string userInput = txtAddClass.Text.Trim();
            if (userInput != "" && userInput != null)
            {
                sto.AddClassIdToFile(userInput);
                DisplayExistingClasses();
            }
            else
            {
                lblFeedbackMessage.Text = "Please enter in a class name or identifier in the text box.";
                lblFeedbackMessage.Visible = true;
            }
            //txtAddClass.Clear();
            //txtAddClass.Focus();
        }
        #endregion

        #region Add / Update / Group Students

        #region Objective Completed Buttons
        
        /// <summary>
        /// Determines which students are selected and adds the currently selected objective(s) to their
        /// individual student object's CompletedObjective List element.  If student object already has
        /// objective completed it will not add it again.  After adding the completed objective to the 
        /// student object's list, data will be updated in .txt storage file. 
        /// If either student or objective lists have no items
        /// checked, error message will display and nothing will be executed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCompleted_Click(object sender, EventArgs e)
        {
            ClearAllGroupBoxes();
            lblFeedbackMessage.Visible = false;
            int studentCount = lvRoster.CheckedItems.Count;
            int objCount = lvObjectives.CheckedItems.Count;
            if (studentCount > 0 && objCount > 0)//makes sure something is checked in each listview box (at least one in each)
            {
                int[] sIndicies = new int[studentCount];//holds listViewbox index value of selected students
                ListView.CheckedListViewItemCollection roster = lvRoster.CheckedItems;
                int r = 0;
                List<string> updatedNames = new List<string>();//will hold names of currently checked students
                foreach (ListViewItem item in roster)//stores index values of the currently selected students
                {
                    int temp = item.Index;
                    sIndicies[r] = temp;
                    ++r;
                }
                int[] oIndicies = new int[objCount];//holds listViewbox index value of selected objectives
                ListView.CheckedListViewItemCollection objList = lvObjectives.CheckedItems;
                int o = 0;
                foreach (ListViewItem item in objList)
                {
                    int temp = item.Index;
                    oIndicies[o] = temp;
                    ++o;
                }
                foreach (int sInd in sIndicies)
                {
                    int largeScopeIndex = sto.ActiveStudentObjectIndicies[sInd];
                    Student tempStu = sto.TempStudentObjectList[largeScopeIndex];//should create a 'holder' for the student object in the large TempStudentObjectList that refers to the selected name of the checked item in listbox
                    updatedNames.Add(tempStu.ToStringFirstLast());//adds proper display name for student
                    foreach (int oInd in oIndicies)
                    {
                        string objective = sto.TempObjectivesList[oInd];
                        bool goAhead = true;
                        Objective tempObj = new Objective(objective);
                        string tempObjString = tempObj.ToString();
                        foreach (Objective establishedTarget in tempStu.Completed)
                        {
                            string estabTarString = establishedTarget.ToString();
                            if (estabTarString == tempObjString)
                            {
                                goAhead = false;
                                break;
                            }
                        }
                        if (goAhead == true)
                        {
                            tempStu.CompletedObjective(tempObj);
                            sto.TempStudentObjectList[largeScopeIndex] = tempStu;
                        }
                    }
                }
                int size = sto.TempStudentObjectList.Count;
                string[] updatedRoster = new string[size];
                for (int k = 0; k < size; ++k)
                {
                    string tempString = sto.TempStudentObjectList[k].ToStringForFile();
                    updatedRoster[k] = tempString;
                }
                sto.UpdateStudentsFile(updatedRoster);
                lblUpdateComplete.Text = "The following students\nhave been updated\nto reflect objective(s)\ncompletion:\n";

                int updatedCount = updatedNames.Count();
                if (updatedCount > 3)//display up to three names plus however many more there were.
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        lblUpdateComplete.Text += updatedNames[i] + "\n";
                    }
                    int diff = updatedCount - 3;
                    lblUpdateComplete.Text += "And " + diff + " others";
                }
                else//less than four students were updated so we'll display all names.
                {
                    foreach (string s in updatedNames)
                    {
                        lblUpdateComplete.Text += s + "\n";
                    }
                }
                lblUpdateComplete.Visible = true;

                //unchecks all items in student roster box and in objective box.
                foreach (ListViewItem l in roster)
                {
                    l.Checked = false;
                }
                foreach (ListViewItem l in objList)
                {
                    l.Checked = false;
                }
            }
            else
            {
                lblFeedbackMessage.Text = "Please make sure you have selected at least one student and one learning objective.";
                lblFeedbackMessage.Visible = true;
            }
        }

        /// <summary>
        /// Determines which students are selected and adds the currently selected objective(s) to their
        /// individual student object's PartialComplete List element.  If student object already has
        /// objective partially completed it will not add it again.  After adding the partially completed
        /// objective to the student object's list, data will be updated in .txt storage file. 
        /// If either student or objective lists have no items
        /// checked, error message will display and nothing will be executed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPartial_Click(object sender, EventArgs e)
        {
            ClearAllGroupBoxes();
            lblFeedbackMessage.Visible = false;
            int studentCount = lvRoster.CheckedItems.Count;
            int objCount = lvObjectives.CheckedItems.Count;
            if (studentCount > 0 && objCount > 0)//makes sure something is checked in each listview box (at least one in each)
            {
                int[] sIndicies = new int[studentCount];//holds listViewbox index value of selected students
                ListView.CheckedListViewItemCollection roster = lvRoster.CheckedItems;
                int r = 0;
                List<string> updatedNames = new List<string>();//will hold names of currently checked students
                foreach (ListViewItem item in roster)//stores index values of the currently selected students
                {
                    int temp = item.Index;
                    sIndicies[r] = temp;
                    ++r;
                }
                int[] oIndicies = new int[objCount];//holds listViewbox index value of selected objectives
                ListView.CheckedListViewItemCollection objList = lvObjectives.CheckedItems;
                int o = 0;
                foreach (ListViewItem item in objList)
                {
                    int temp = item.Index;
                    oIndicies[o] = temp;
                    ++o;
                }
                foreach (int sInd in sIndicies)
                {
                    int largeScopeIndex = sto.ActiveStudentObjectIndicies[sInd];
                    Student tempStu = sto.TempStudentObjectList[largeScopeIndex];//should create a 'holder' for the student object in the large TempStudentObjectList that refers to the selected name of the checked item in listbox
                    updatedNames.Add(tempStu.ToStringFirstLast());//adds proper display name for student
                    foreach (int oInd in oIndicies)
                    {
                        string objective = sto.TempObjectivesList[oInd];
                        bool goAhead = true;
                        Objective tempObj = new Objective(objective);
                        string tempObjString = tempObj.ToString();
                        foreach (Objective establishedTarget in tempStu.Completed)
                        {
                            string estabTarString = establishedTarget.ToString();
                            if (estabTarString == tempObjString)
                            {
                                goAhead = false;//objective is already in the student's completion list
                                break;
                            }
                        }
                        if (goAhead == true)//student has not partially completed this objective so we will add it to their list!
                        {
                            tempStu.PartialComplete(tempObj);
                            sto.TempStudentObjectList[largeScopeIndex] = tempStu;
                        }
                    }
                }
                int size = sto.TempStudentObjectList.Count;
                string[] updatedRoster = new string[size];
                for (int k = 0; k < size; ++k)
                {
                    string tempString = sto.TempStudentObjectList[k].ToStringForFile();
                    updatedRoster[k] = tempString;
                }
                sto.UpdateStudentsFile(updatedRoster);
                lblPartialUpdate.Text = "The following students\nhave been updated\nto reflect objective(s)\ncompletion:\n";

                int updatedCount = updatedNames.Count();
                if (updatedCount > 3)//display up to three names plus however many more there were.
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        lblPartialUpdate.Text += updatedNames[i] + "\n";
                    }
                    int diff = updatedCount - 3;
                    lblPartialUpdate.Text += "And " + diff + " others";
                }
                else//less than four students were updated so we'll display all names.
                {
                    foreach (string s in updatedNames)
                    {
                        lblPartialUpdate.Text += s + "\n";
                    }
                }
                lblPartialUpdate.Visible = true;

                //unchecks all items in student roster box and in objective box.
                foreach (ListViewItem l in roster)
                {
                    l.Checked = false;
                }
                foreach (ListViewItem l in objList)
                {
                    l.Checked = false;
                }
            }
            else
            {
                lblFeedbackMessage.Text = "Please make sure you have selected at least one student and one learning objective.";
                lblFeedbackMessage.Visible = true;
            }
        }
        #endregion

        /// <summary>
        /// Executes when the menuItem to add new student is clicked.  Group 'panel' with student
        /// fields will display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiAddStudent_Click(object sender, EventArgs e)
        {
            DisplayExistingClasses(); //essentially does the same as when load existing class is clicked.  User must select class first then the option of adding a student will show!
            grpAddStudent.Visible = true;
            txtFName.Focus();//sets cursor in add class field after user selects class id.
        }

        /// <summary>
        /// Executes when create student button is clicked or simulated by enter press.  New student object will be created
        /// using the first name, last name, and currently selected class section id.  If no last name is given, a "/" character
        /// is used during object creation but will not be displayed in roster box.  This method does update the data file to append
        /// new student information as well as adds new student object to temporary storage object's 'temp student object list'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddStudent_Click_1(object sender, EventArgs e)
        {
            string f = txtFName.Text;
            string l = txtLName.Text;
            string userSelect;
            if (l == "" || l == " " || l == null)
            {
                l = "/";
            }
            if (lbClasses.SelectedIndex == -1)
            {
                userSelect = sto.CurrentClassID;
            }
            else
            {
                userSelect = lbClasses.SelectedItem.ToString();
                sto.CurrentClassID = userSelect;
            }
            Student toAdd = new Student(f, l, userSelect);
            sto.AddStudentToFile(toAdd);
            txtFName.Clear();
            txtLName.Clear();
            if (l == "/")
            {
                lblAddStudentConfirm.Text = f + " has been added\nto " + userSelect + "!";
            }
            else
            {
                lblAddStudentConfirm.Text = f + " " + l + " has been added\nto " + userSelect + "!";
            }
            lblObjAddConfirm.Visible = true;
            txtFName.Focus();
            DisplayClassRosterPanel();
            ++addStudentCounter;
        }

        /// <summary>
        /// Uses currently selected student to generate list of completed and partially completed
        /// objectives.  Displays these in student achievement group box.  Does not display objectives
        /// that have not been completed.  Will display error if more than one or zero students have been
        /// checked in the roster listview box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStuAchieve_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            ClearGroupBoxes();
            if (scRosterDisplay.Visible == true)
            {
                if (lvRoster.CheckedItems.Count == 1)
                {
                    lbStuProgCom.Items.Clear();
                    lbStuProgPart.Items.Clear();
                    int index = lvRoster.CheckedIndices[0];
                    int actualIndex = sto.ActiveStudentObjectIndicies[index];
                    Student selected = sto.TempStudentObjectList[actualIndex];
                    grpStuProgress.Text = selected.ToStringFirstLast();
                    foreach (Objective comp in selected.Completed)
                    {
                        string id = comp.ToString();
                        lbStuProgCom.Items.Add(id);
                    }
                    foreach (Objective part in selected.Partial)
                    {
                        string id = part.ToString();
                        lbStuProgPart.Items.Add(id);
                    }
                    grpStuProgress.Visible = true;
                }
                else
                {
                    //roster is displayed but either no or more than one item is selected.
                    lblFeedbackMessage.Text = "Please select only one student to generate details for.";
                    lblFeedbackMessage.Visible = true;
                }
            }
            else
            {
                //roster is not displayed
                lblFeedbackMessage.Text = "You must select a class first in order to generate individual student progress details";
                lblFeedbackMessage.Visible = true;
            }
            
        }

        /// <summary>
        /// Generates groups of students based upon currently selected objectives.  Students are separated
        /// into groups depending on if they have the selected objective(s) in their Complete or PartialComplete
        /// Lists.  If multiple objectives are selected, students must have completed ALL (not all of some and partials of others) in order
        /// to be in the "Complete" list.  If no objective(s) are selected then an error message will display and
        /// no code is actually executed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGoGroups_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            ClearGroupBoxes();
            ClearStuProgBoxes();
            if (scRosterDisplay.Visible == true)
            {
                if (lvObjectives.CheckedIndices.Count > 0)
                {
                    int objecCount = lvObjectives.CheckedItems.Count;
                    int[] oIndicies = new int[objecCount];//holds the index values of all checked objectives
                    List<string> currentSelectedObjectives = new List<string>();
                    ListView.CheckedListViewItemCollection objList = lvObjectives.CheckedItems;
                    int o = 0;
                    foreach (ListViewItem item in objList)
                    {
                        int temp = item.Index;
                        oIndicies[o] = temp;
                        currentSelectedObjectives.Add(sto.TempObjectivesList[temp]);//adds to list to display as confirmation message
                        ++o;
                    }
                    string currentClass = sto.CurrentClassID;
                    //List<string> passed = new List<string>();
                    //List<string> partial = new List<string>();
                    //List<string> failed = new List<string>();
                    foreach (Student stud in sto.TempStudentObjectList)
                    {
                        if (stud.SectionID == sto.CurrentClassID)
                        {
                            bool passAll = true; bool partAll = true;
                            for (int i = 0; i < o; ++i)
                            {
                                bool pass = false; bool part = false;
                                string current = sto.TempObjectivesList[oIndicies[i]];
                                foreach (Objective compObj in stud.Completed)
                                {
                                    if (pass == false)//once pass turns to true stop looking for the objective because it has already been "found"
                                    {
                                        if (current == compObj.ToString())
                                        {
                                            pass = true;
                                        }
                                    }
                                }
                                foreach (Objective partObj in stud.Partial)
                                {
                                    if (pass == false)//if student has completely passed this objective we're not going to see if they also partially passed it at one point.
                                    {
                                        if (part == false)//if part turned to true it means the current objective was found in student's partial list so we should move on.
                                        {
                                            if (current == partObj.ToString())
                                            {
                                                part = true;
                                            }
                                        }
                                    }
                                }
                                //the sequence below should happen once for each objective selected to generate groups from.
                                if ( (part == true && partAll == true) /* student has partial pass of current objective AND partial pass of all the previous ones compared to (if any)*/ 
                                    || (part == true && passAll == true) /*student has partial pass of current objective AND complete pass of all previous ones*/ 
                                    || (pass == true && partAll == true) ) /*student has partial pass of previous objective(s)  and complete pass of current objective*/
                                {
                                    partAll = true; //set Partial pass of all to true (Student has at LEAST PARTIALLY passed all objectives (even if some of those include complete passes)
                                }
                                else
                                {
                                    partAll = false;
                                }

                                if (passAll == true && pass == true)
                                {
                                    passAll = true;
                                }
                                else
                                {
                                    passAll = false;
                                }
                                //end sequence

                                //if (pass == false)
                                //{
                                //    passAll = false;
                                //}
                                //if (part == false)
                                //{
                                //    partAll = false;
                                //}
                            }
                            if (passAll == true)
                            {
                                lvCompleted.Items.Add(stud.ToStringFirstLast());
                            }
                            else if (partAll == true)
                            {
                                lvPartial.Items.Add(stud.ToStringFirstLast());
                            }
                            else
                            {
                                lvNot.Items.Add(stud.ToStringFirstLast());
                            }
                        }
                    }
                    lblSortGroupList.Text = "The objectives used\nas criteria for this\nsort were:\n";
                    int objCount = currentSelectedObjectives.Count();
                    if (objCount > 3)//just display three objectives and tell how many more there were (instead of having a huge list).
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            lblSortGroupList.Text += currentSelectedObjectives[i] + "\n";
                        }
                        int diff = objCount - 3;
                        lblSortGroupList.Text += "And " + diff + " others.";
                    }
                    else
                    {
                        foreach (string s in currentSelectedObjectives)
                        {
                            lblSortGroupList.Text += s + "\n";
                        }
                    }
                    lblSortGroupList.Visible = true;
                    pnlGroups.Visible = true;
                }
                else
                {
                    lblFeedbackMessage.Text = "You must select at least one objective to generate achievement groups for.";
                    lblFeedbackMessage.Visible = true;
                }
            }
            else
            {
                lblFeedbackMessage.Text = "Please first select a class so you can select which\nobjective(s) you would like to generate groups from.";
                lblFeedbackMessage.Visible = true;
                tsmiLoadClass_Click(sender, e);
            }
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            StringBuilder fullSb = new StringBuilder();
            string temp;

            CopyListViewToClipboard(lvCompleted, out temp);
            fullSb.AppendLine("**COMPLETE**");
            if (temp == null)
            {
                fullSb.AppendLine("//NONE//");
                fullSb.AppendLine();
            }
            else
            {
                fullSb.AppendLine(temp);
            }

            CopyListViewToClipboard(lvPartial, out temp);
            fullSb.AppendLine("**PARTIALLY**");
            if (temp == null)
            {
                fullSb.AppendLine("//NONE//");
                fullSb.AppendLine();
            }
            else
            {
                fullSb.AppendLine(temp);
            }

            CopyListViewToClipboard(lvNot, out temp);
            fullSb.AppendLine("**INCOMPLETE**");
            if (temp == null)
            {
                fullSb.AppendLine("//NONE//");
                //fullSb.AppendLine();
            }
            else
            {
                fullSb.AppendLine(temp);
            }

            Clipboard.SetData(System.Windows.Forms.DataFormats.Text, fullSb);

            lblFeedbackMessage.Text = "Group data has been copied to the clipboard.\nYou should be able to paste into a document.";
            lblFeedbackMessage.Visible = true;
        }

        /// <summary>
        /// Copies all existing items in ListViewBox to clipboard.  Over-writes any existing clipboard data.  During paste,
        /// each separate item is displayed on a separate line (row).
        /// </summary>
        /// <param name="listViewBox">ListView display box to copy existing items from</param>
        private void CopyListViewToClipboard(ListView listViewBox, out string sbString)
        {
            int count = listViewBox.Items.Count;
            if (count > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < count; ++j)
                {
                    string temp = listViewBox.Items[j].Text;
                    //sb.Append(lvCompleted.Items[j].Text);
                    if (temp != null && temp != " " && temp != "")
                    {
                        sb.AppendLine(temp);
                    }

                }
                sbString = sb.ToString();
                Clipboard.SetData(System.Windows.Forms.DataFormats.Text, sbString);
            }
            else
            {
                sbString = null;
            }
            //else
            //{
            //    lblFeedbackMessage.Text = "There must be students contained in the group box\nin order to copy the items to clipboard.";
            //    lblFeedbackMessage.Visible = true;
            //}
        }

        #endregion

        #region Add New Objective
        /// <summary>
        /// Executes when the menuItem to add learning objective is clicked.  Group 'panel' with objective
        /// fields will display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiAddObjective_Click(object sender, EventArgs e)
        {
            grpLearnObj.Visible = true;
            lblObjAddConfirm.Visible = false;
            txtObjId.Focus();
            tsmiLearningObjectives.HideDropDown();
        }

        /// <summary>
        /// Executes when "Create Objective" button is clicked or simulated through enter press.
        /// If no description is given a "/" character will be used to store the description during objective
        /// object creation.  An objective ID is required for method to execute.  Error message will
        /// display if ID field is not entered.  This will not check for duplicate objectives!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddObjective_Click(object sender, EventArgs e)
        {
            string id = txtObjId.Text.Trim();
            string desc = txtObjDesc.Text.Trim();
            if (id != null)
            {
                if (desc == null || desc == "")
                {
                    desc = "/";
                }
                Objective userSelect = new Objective(id, desc);
                sto.AddObjectiveToFile(userSelect);
                sto.GetObjectivesFromFile();
                DisplayLearningObjectivesPanel();
                lblObjAddConfirm.Text = "You have successfully\nadded Learning\nObjective " + id + "!";
                txtObjId.Clear();
                txtObjDesc.Clear();
                ++addObjectiveCounter;
                txtObjId.Focus();
                lblObjAddConfirm.Visible = true;
            }
            else//if id == null (which needs to be required)
            {
                lblObjAddConfirm.Text = "You did not enter a unique identifier for the learning objective.  Please try again.";
                txtObjDesc.Clear();
                txtObjId.Focus();
            }
        }
        #endregion

        #region Hide Panels
        //Hides feedback message label when buttons are pressed or boxes are entered
        private void grpSelectClass_Enter(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
        }
        private void grpAddClass_Enter(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
        }
        private void grpAddStudent_Enter(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
        }
        private void grpLearnObj_Enter(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
        }
        private void scRosterDisplay_Panel1_Enter(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
        }
        private void scRosterDisplay_Panel2_Enter(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
        }
        private void pnlGenerateGroups_Enter(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
        }
        private void pnlGroups_MouseEnter(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
        }

        /// <summary>
        /// Clears all items from group boxes used to display class students in 
        /// "complete", "partial" and "not complete" sections
        /// </summary>
        private void ClearGroupBoxes()
        {
            lvCompleted.Items.Clear();
            lvPartial.Items.Clear();
            lvNot.Items.Clear();
            lblSortGroupList.Text = "";
        }

        /// <summary>
        /// Clears all items from student achievement list boxes
        /// </summary>
        private void ClearStuProgBoxes()
        {
            lbStuProgCom.Items.Clear();
            lbStuProgPart.Items.Clear();
            grpStuProgress.Text = "";
        }

        /// <summary>
        /// Clears all items from student achievement and class grouping list boxes.
        /// </summary>
        private void ClearAllGroupBoxes()
        {
            ClearGroupBoxes();
            ClearStuProgBoxes();
        }
        #endregion

        #region Menu Item Hover Display Controls

        bool obj1Drop = false, obj2Drop = false, obj3Drop = false, obj4Drop = false;
        
        private bool NO_CURSOR_IN_MENU()
        {
            bool returnVal;
            if (obj1Drop == false && obj2Drop == false && obj3Drop == false && obj4Drop == false)
            {
                returnVal = true;
            }
            else
            {
                returnVal = false;
            }
            return returnVal;
        }

        private void tsmiLearningObjectives_MouseEnter(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            tsmiClass.HideDropDown();
            tsmiStudents.HideDropDown();
            tsmiLearningObjectives.ShowDropDown();

        }

        private void msMainMenu_MouseLeave(object sender, EventArgs e)
        {
            //tsmiLearningObjectives.HideDropDown();
        }

        private void tsmiLearningObjectives_DropDownOpening(object sender, EventArgs e)
        {
            //obj1Drop = true;
        }

        private void tsmiLearningObjectives_MouseLeave(object sender, EventArgs e)
        {
                //if (obj1Drop == true && obj2Drop == false)
                //{
                //    tsmiLearningObjectives.HideDropDown();
                //}
        }

        private void tsmiAddObjective_MouseEnter(object sender, EventArgs e)
        {
            //obj2Drop = true;
            //if (NO_CURSOR_IN_MENU())
            //{
            //    tsmiLearningObjectives.HideDropDown();
            //}
        }

        private void tsmiAddObjective_MouseLeave(object sender, EventArgs e)
        {
            //if (NO_CURSOR_IN_MENU())
            //{
            //    tsmiLearningObjectives.HideDropDown();
            //}
            //obj2Drop = false;
        }

        private void tsmiGroups_MouseLeave(object sender, EventArgs e)
        {
            //if (NO_CURSOR_IN_MENU())
            //{
            //    tsmiLearningObjectives.HideDropDown();
            //}
            //obj3Drop = false;
        }


        private void tsmiGroups_DropDownOpening(object sender, EventArgs e)
        {
            //obj4Drop = true;
            //if (NO_CURSOR_IN_MENU())
            //{
            //    tsmiLearningObjectives.HideDropDown();
            //}
        }

        private void tsmiGroupByObj_MouseLeave(object sender, EventArgs e)
        {
            //if (NO_CURSOR_IN_MENU())
            //{
            //    tsmiLearningObjectives.HideDropDown();
            //}
            //obj4Drop = false;
        }

        private void tsmiGroups_MouseEnter(object sender, EventArgs e)
        {
            grpLearnObj.Visible = false; //Hides add learning objective panel group so the rest of the menu item can display cleanly.
            tsmiGroups.ShowDropDown();
            //obj3Drop = true;
        }

        private void tsmiGroupByObj_Click(object sender, EventArgs e)
        {
            tsmiGroups.HideDropDown();
            tsmiGroupByObj.HideDropDown();
            btnGoGroups_Click(sender, e);
        }

        private void tsmiStudents_MouseEnter(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            tsmiClass.HideDropDown();
            tsmiLearningObjectives.HideDropDown();
            tsmiStudents.ShowDropDown();
            grpLearnObj.Visible = false;
        }

        private void tsmiStudents_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            tsmiStudents.HideDropDown();
        }

        private void tsmiClass_MouseEnter(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            tsmiStudents.HideDropDown();
            tsmiLearningObjectives.HideDropDown();
            tsmiStudents.ShowDropDown();
            grpLearnObj.Visible = false;
        }

        private void tsmiClass_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            tsmiClass.HideDropDown();
        }
        #endregion

        #region ClearConfirmMessages
        private void txtBxObjId_KeyDown(object sender, KeyEventArgs e)
        {
            if (addObjectiveCounter > 0)
            {
                lblObjAddConfirm.Text = "";
            }
        }

        private void txtBxFName_KeyDown(object sender, KeyEventArgs e)
        {
            if (addStudentCounter > 0)
            {
                lblAddStudentConfirm.Text = "";
            }
        }

        //private void lvRoster_ItemChecked(object sender, ItemCheckedEventArgs e)
        //{
        //    lblUpdateComplete.Visible = false;
        //    lblPartialUpdate.Visible = false;
        //}

        //private void lvObjectives_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    lblUpdateComplete.Visible = false;
        //    lblPartialUpdate.Visible = false;
        //}
        #endregion
        
        #region Enter Key Handlers

        /// <summary>
        /// If enter is pressed while in the text box to type in the name
        /// of a new class to create, this will simulate as if the user 
        /// clicked the "create" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAddClass_KeyPress(object sender, KeyPressEventArgs e)
        {
                if (WAS_ENTER_PRESSED(e.KeyChar))
                {
                    btnAddClass_Click(sender, e);
                }
        }

        /// <summary>
        /// If enter is pressed while the tab stop is on the "create" class
        /// button, it will simulate a click event of that button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddClass_KeyPress(object sender, KeyPressEventArgs e)
        {
                if (WAS_ENTER_PRESSED(e.KeyChar))
                {
                    btnAddClass_Click(sender, e);
                }
        }


        private void txtBxFName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (WAS_ENTER_PRESSED(e.KeyChar))
            {
                btnAddStudent_Click_1(sender, e);
            }
        }
        private void txtBxLName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtFName.Text.Trim() != "" && txtFName.Text.Trim() != null)
            {
                if (WAS_ENTER_PRESSED(e.KeyChar))
                {
                    btnAddStudent_Click_1(sender, e);
                }
            }
        }
        private void btnAddStudent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (WAS_ENTER_PRESSED(e.KeyChar))
            {
                btnAddStudent_Click_1(sender, e);
            }
        }



        private void txtBxObjId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (WAS_ENTER_PRESSED(e.KeyChar))
            {
                btnAddObjective_Click(sender, e);
            }
        }



        private void txtBxObjDesc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtObjId.Text.Trim() != "" && txtObjId.Text.Trim() != null)
            {
                if (WAS_ENTER_PRESSED(e.KeyChar))
                {
                    btnAddObjective_Click(sender, e);
                }
            }

        }

        private void lbClasses_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (WAS_ENTER_PRESSED(e.KeyChar))
            {
                btnSelectClass_Click(sender, e);
            }
        }


        private void btnSelectClass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (WAS_ENTER_PRESSED(e.KeyChar))
            {
                btnSelectClass_Click(sender, e);
            }
        }








        /// <summary>
        /// Determines if Enter key was pressed by user
        /// </summary>
        /// <param name="input">key character generated by event</param>
        /// <returns>true if enter key was pressed, false if any other key</returns>
        private bool WAS_ENTER_PRESSED(char input)
        {
            bool value = false; 
            if (input == 13)
            {
                value = true;
            }
            return value;
        }
        #endregion

        #region UNUSED CODE
        //private void btnLoadClass_Click(object sender, EventArgs e)
        //{
        //    //what about loading a class if id has not been created yet?
        //    //Storage stor = new Storage();
        //    DetermineUserClassId();
        //    storageObject.GetNameRoster(storageObject.CurrentClassID);
        //}

        //private void DetermineUserClassId()
        //{
        //    lsBxMMSelClass.Focus();
        //    int index = lsBxMMSelClass.SelectedIndex;
        //    string selected = lsBxMMSelClass.SelectedText;
        //    if (index >= 0 && selected != "")
        //    {
        //        selected = lsBxMMSelClass.SelectedItem.ToString();
        //        storageObject.AddClassIdToFile(selected);
        //    }
        //    else if (index < 0)
        //    {
        //        storageObject.AddClassIdToFile(selected);
        //    }
        //    else if (selected == "")
        //    {
        //        selected = null;//do something about this
        //    }
        //    storageObject.CurrentClassID = selected;
        //    storageObject.AddClassIdToFile(selected);
        //}

        //private void btnAddStudent_Click(object sender, EventArgs e)
        //{
        //    DetermineUserClassId();
        //    string f = txtBxFName.Text;
        //    string l = txtBxLName.Text;
        //    bool ready = (f.Length > 0);//trying to prohibit empty strings
        //    if (ready)
        //    {
        //        Student tempStu = new Student(f, l, storageObject.CurrentClassID);
        //        storageObject.TempStudentObjectList.Add(tempStu);
        //        storageObject.AddStudentToFile(tempStu);
        //        lblConfirmAddStu.Text = "You've added " + f + " " + l + " to " + storageObject.CurrentClassID + "!";
        //    }
        //    txtBxFName.Text = "";
        //    txtBxLName.Text = "";
        //}
        //figure this shit out up here you have two add student clicks!

        //private void DisplayStudentsInRosterBox()
        //{
        //    lvRoster.Clear();
        //    foreach (Student stu in storageObject.TempStudentObjectList)
        //    {
        //        if (stu.SectionID == storageObject.CurrentClassID)
        //        {
        //            if (stu.LastName == "/")
        //            {
        //                lvRoster.Items.Add(stu.FirstName);
        //            }
        //            else
        //            {
        //                lvRoster.Items.Add(stu.FirstName + " " + stu.LastName);
        //            }
        //        }
        //    }
        //}

        //private void DisplayRosterBox()
        //{
        //    //if (selectClassButtonCounter == 0)
        //    {

        //        //storageObject.CreateStudentObjects();
        //    }
        //    //storageObject.CreateStudentObjects(storageObject.TempRosterList);
        //    DisplayStudentsInRosterBox();
        //}

        //private void btnMMAddToExisting_Click(object sender, EventArgs e)
        //{
        //    //btnMMSelectExClass.Visible = false;
        //    if (pnlMMSelectExClass.Visible == false)
        //    {
        //        DisplayExistingClasses();
        //    }
        //    pnlAddStudent.Visible = true;
        //}

        ///// <summary>
        ///// Sets visibility of main split panel container to true.
        ///// </summary>
        //private void SPLIT_CONTAINER_CLASS_INFO_ON()
        //{
        //    if (scRosterDisplay.Visible == false)
        //    {
        //        scRosterDisplay.Visible = true;
        //        pnlGenerateGroups.Visible = true;
        //    }
        //}
        #endregion

    }
}



