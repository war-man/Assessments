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
        /*this variable is designed to be changed when optimizing code to be run
         * on a mac via Mono framework or to be used as 'normal' on windows.net
         * framework.  Variable primarily generated to provide workaround for 
         * inability to use .net methods to copy to clipboard on mac os */
        private bool mac;
        public bool Mac
        {
            get { return mac; }
            set { mac = value; }
        }
        
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

        //constructor of windows form (GUI window)
        public LearningTrackerWindow()
        {
            InitializeComponent();//native method
            ToDoOnLoad();//own generated method
        }

        /// <summary>
        /// Contains sequence of methods to execute immediately upon generation of form.
        /// Items include loading existing data into newly generated instance of storage object
        /// to be used during existence of form.  
        /// Calls methods to determine if Mac options should 
        /// be "on" or "off", load existing class IDs, learning objectives, and to read each existing
        /// student data line and create an object for each individual student.
        /// </summary>
        private void ToDoOnLoad()
        {
            //determine if Optimize for Mac box is checked
            OPTIMIZE_FOR_MAC_OPTION();
            //load current class id list from file to storage object
            sto.GetClassIdsFromFile();
            //load current objective list from file to storage object
            sto.GetObjectivesFromFile();
            /*create student objects based on current data in .txt file upon load
            any new students created will be appended to end of tempstudentobjectlist in 
            storage object*/
            sto.CreateStudentObjects();
            //initialized form to display menu where user can select to load existing class (if any)
            DisplayExistingClasses();
        }

        /// <summary>
        /// Determines if "Optimize for Mac" checkbox option is selected.  If it is selected
        /// the learningWindow boolean 'Mac' is set to true, else it is set to false.
        /// Primarily designed to implement workabout of inability to copy to clipboard
        /// on Mac OS.
        /// </summary>
        private void OPTIMIZE_FOR_MAC_OPTION()
        {
            bool currentState = cbxMac.Checked; //current state of checkbox
            if (currentState == true)
            {
                Mac = true;
            }
            else
            {
                Mac = false;
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Executed when checkbox state of "Optimize for Mac" option is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxMac_CheckedChanged(object sender, EventArgs e)
        {
            OPTIMIZE_FOR_MAC_OPTION();
        }

        #region Load Class Roster & Objectives

        /// <summary>
        /// Executes when user clicks on "Class, LoadClass" submenu option.  Will display the 
        /// features to select an existing class from tempclassIdlist in storageobject.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //Class, "Load Existing Class" menu item click
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
            lbClasses.Items.Clear();//to prevent classes from being duplicated in listbox if method is called again.
            foreach (string s in sto.TempClassIDList) //classes are taken from tempClassIDList of storage object at this moment.
            {
                lbClasses.Items.Add(s);
            }
            //if menu to select class is not already being shown, show it.
            if (grpSelectClass.Visible == false)
            {
                grpSelectClass.Visible = true;
            }
            grpAddClass.Visible = false;//option to add new class will only be shown if user selects option from menu.
        }

        /// <summary>
        /// When user clicks on "Go" button!  If button is clicked the selected class will become the "active" class!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //Select Class "Go" button click
        private void btnSelectClass_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            if (lbClasses.SelectedIndex != -1) //if it is == -1 that means nothing in the box is selected and it won't do anything!
            {
                ClearAllGroupBoxes();
                string userSelect = lbClasses.SelectedItem.ToString();
                sto.CurrentClassID = userSelect;//establishes current selected item in class list box as currentclassID for storage object, which will drive everything with displaying students and updating data now!
                //NOT NEEDED//storageObject.GetNameRoster(storageObject.CurrentClassID);//will generate list of student names who are only in selected class and store them in tempstudentlist of storage object.
                DisplayClassRosterPanel();//displays roster of students in currently selected class
                scRosterDisplay.Visible = true;//set this line ahead of DisplayLearningObjectives due to bug in displaying in complete column.
                DisplayLearningObjectivesPanel();//displays learning objectives loaded into storage object from .txt data file
                pnlGenerateGroups.Visible = true;//shows menu option to generate groups below learning objectives
            }
            else//reaches this if no class in box is selected.  Simply displays error message to tell user they need to select a class if they want this button click to do anything.
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
            lvObjectives.Clear();//to avoid duplicates if method is called again
            lblFeedbackMessage.Visible = false;
            foreach (string o in sto.TempObjAndDescList)
            {
                lvObjectives.Items.Add(o);
            }
            //lvObjectives.Visible = true;//this should always be true after first class select call so this was commented out.
        }

        /// <summary>
        /// Determines which chapters user wants to display objectives from and calls method to store
        /// only specific objectives in active objective list.  Calls method to re-populate the learning
        /// objective panel to display objectives from only those chapters (units).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //"Show Selected" button click
        private void btnShowChaptObj_Click(object sender, EventArgs e)
        {
            ListView.CheckedListViewItemCollection checkedStuff = lvChapters.CheckedItems;//collection of all the items that are checked in chapter box
            int count = checkedStuff.Count;//how many things are checked
            if (count > 0)//at least one thing is checked
            {
                string[] chapters = new string[count];//will hold .text fields for each box that is checked
                int i = 0;
                foreach (ListViewItem item in checkedStuff)
                {
                    chapters[i] = item.Text.Trim();//places checked box's chapter number text string into array
                    ++i;
                }
                sto.DetermineActiveObjectiveIndicies(chapters);//to store absolute index values of objectives that will be relatively displayed.
                DisplaySpecificObjectives();//re-populates objective box with specific chapters' objectives.
            }
            else//nothing is checked in the listViewChapters box.
            {
                btnShowAllObj_Click(sender, e);//will show all objectives because no specific chapters are selected.
                lblFeedbackMessage.Text = "All objectives have been displayed.";//notifies user that all objectives have been displayed because they clicked to display specific ones but they didn't check any chapters.
                lblFeedbackMessage.Visible = true;
            }
        }

        /// <summary>
        /// Re-populates the objective panel with objectives that are "active" based upon user
        /// selection.
        /// </summary>
        private void DisplaySpecificObjectives()
        {
            lblFeedbackMessage.Visible = false;
            lvObjectives.Clear();
            foreach (int i in sto.ActiveObjectiveIndices)
            {
                string display = sto.TempObjAndDescList[i];
                lvObjectives.Items.Add(display);
            }
        }

        /// <summary>
        /// Displays all objectives in objective panel - regardless of if any specific chapters are selected
        /// in chapter select box.  Unchecks all chapters after display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //"Show All" objective button click
        private void btnShowAllObj_Click(object sender, EventArgs e)
        {
            sto.GetObjectivesFromFile();//re-establishes active objectives as all objectives from data file
            DisplayLearningObjectivesPanel();//displays all objectives in panel
            UnselectAllChapters();//ensures no specific chapters are selected
        }

        /// <summary>
        /// Iterates through Chapters listview box and un-checks all items.
        /// </summary>
        private void UnselectAllChapters()
        {
                lblFeedbackMessage.Visible = false;
                int count = lvChapters.Items.Count;//how many items are in the chapters listview box.
                for (int i = 0; i < count; ++i)
                {
                    lvChapters.Items[i].Focused = true;
                    lvChapters.Items[i].Checked = false;
                }
        }

        /// <summary>
        /// Iterates through all active items in learning objectives listbox and sets check property to true
        /// (checks all items).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //"Select All" Learning Objectives button click
        private void btnSelectAllObj_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            int count = lvObjectives.Items.Count;//how many items are in objective listbox
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    lvObjectives.Items[i].Focused = true;
                    lvObjectives.Items[i].Checked = true;
                }
            }
            else//gets here if there are no items in objective listbox
            //for example, if a user selects to display chapter 7 objectives, but there are no
            //chapter 7 objectives, the box will be empty.  If they then clicked select all
            //objectives, there will be nothing to select!
            {
                lblFeedbackMessage.Text = "There are no objectives in the list to select.";
                lblFeedbackMessage.Visible = true;
            }
        }

        /// <summary>
        /// Iterates through all active items in learning objectives listbox and sets
        /// check property to false (un-checks all items).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //"Unselect All" Learning Objectives button click
        private void btnUnSelectAllObj_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            int count = lvObjectives.Items.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    lvObjectives.Items[i].Focused = true;
                    lvObjectives.Items[i].Checked = false;
                }
            }
            else
            {
                lblFeedbackMessage.Text = "There are no objectives in the list to un-select.";
                lblFeedbackMessage.Visible = true;
            }
        }
        
        /// <summary>
        /// Method will generate list of students who are in current selected class using the temporary student object
        /// list in sto object.  Each time this method is called, the indices of the students in the active selected
        /// class are re-generated.
        /// </summary>
        private void DisplayClassRosterPanel()
        {
            lblFeedbackMessage.Visible = false;
            RePopulateActiveRoster();//tells storage object which student lines from data file will be displayed
            //in roster box and stores their absolute indices in a relative list.
            lvRoster.Clear();//clears roster listview box to prevent duplicates from displaying if classes are selected multiple times.
            foreach (int index in sto.ActiveStudentObjectIndices)/*ActiveStudentObjectIndices
                contains absolute index values of student objects that are displayed in roster box*/
            {
                string tempFirst = sto.TempStudentObjectList[index].FirstName;
                string tempLast = sto.TempStudentObjectList[index].LastName;
                if (tempLast == "/")//adds only first name if last name value is blank (by storage file 
                    //convention this is indicated by using the '/' character)
                {
                    lvRoster.Items.Add(tempFirst);
                }
                else//adds first and last name to listview box
                {
                    lvRoster.Items.Add(tempFirst + " " + tempLast);
                }
            }
            lblRosterClassHead.Text = sto.CurrentClassID + " Students";//changes header above roster box to class name + 'students'
        }

        /// <summary>
        /// Will set check mark value to "true" for each item in Roster listview box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //"Select All Students" button click
        private void btnSelectAllStudents_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            if (scRosterDisplay.Visible == true)//is this necessary?
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

        //private void lvRoster_ItemChecked(object sender, ItemCheckedEventArgs e)
        //{
            //lblFeedbackMessage.Visible = false;
            //bool go = false;
            //int count = lvRoster.Items.Count;
            //for (int i = 0; i < count; ++i)
            //{
            //    if (lvRoster.Items[i].Checked == true)
            //    {
            //        go = true;
            //        break;
            //    }
            //}
            //if (go == true)
            //{
            //    lblUpdateComplete.Visible = false;
            //}
        //}
        //

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
        //Class, "Create New" menu item click
        private void tsmiNewClass_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            grpAddClass.Visible = true;//displays add class menu options
            txtAddClass.Focus();//automatically places text cursor in classname textbox.
        }

        /// <summary>
        /// Executes when create class button is clicked or simulated by enter press.  Will execute only if 
        /// text exists in class name textbox.  Method updates data file to save new class name as well as updates
        /// 'select class' list box to reflect newly created class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //Create New Class "Create Class" button click
        private void btnAddClass_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            string userInput = txtAddClass.Text.Trim();
            if (userInput != "" && userInput != null)//verifies there was something in the textbox
            {
                sto.AddClassIdToFile(userInput);//adds input string to existing class id's in the file
                DisplayExistingClasses();//re-populates list of existing classes to choose from which
                //now includes newly added class name.
            }
            else//gets here if user clicks button to add class but text box is empty
            {
                lblFeedbackMessage.Text = "Please enter in a class name or identifier in the text box.";
                lblFeedbackMessage.Visible = true;
                txtAddClass.Focus();
            }
        }
        
        #endregion
        
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
        private void btnComplete_Click(object sender, EventArgs e)
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
                    int active = item.Index;//*********WHERE I CHANGED STUFF!!
                    int overall = sto.ActiveObjectiveIndices[active];
                    oIndicies[o] = overall;
                    ++o;
                }
                foreach (int sInd in sIndicies)
                {
                    int largeScopeIndex = sto.ActiveStudentObjectIndices[sInd];
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
                    int active = item.Index;//*********WHERE I CHANGED STUFF!!
                    int overall = sto.ActiveObjectiveIndices[active];
                    oIndicies[o] = overall;
                    ++o;
                }
                foreach (int sInd in sIndicies)
                {
                    int largeScopeIndex = sto.ActiveStudentObjectIndices[sInd];
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
                lblUpdateComplete.Text = "The following students\nhave been updated\nto reflect partial\nobjective(s) completion:\n";

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
        #endregion
        
        #region Add / Update / Group Students
           
        /// <summary>
        /// Executes when the menuItem to add new student is clicked.  Add student 'grp panel' with student
        /// fields will display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //Student, "Add New" menu item click
        private void tsmiAddStudent_Click(object sender, EventArgs e)
        {
            DisplayExistingClasses(); //essentially does the same as when load existing class is clicked.  
            //User must select class first then the option of adding a student will show!
            grpAddStudent.Visible = true;
            txtFName.Focus();//sets cursor in add class field after user selects class id.
        }

        /// <summary>
        /// New student object will be created using the first name, last name, and currently selected class section id.  
        /// If no last name is given, a "/" character is used during object creation but will not be 
        /// displayed in roster box.  This method does update the data file to append new student 
        /// information as well as adds new student object to temporary storage object's 'temp student 
        /// object list'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //Create New Student "Create Student" button click
        private void btnAddStudent_Click_1(object sender, EventArgs e)
        {
            string f = txtFName.Text;
            string l = txtLName.Text;
            string userSelect;
            if (l == "" || l == " " || l == null)//if last name field is empty
            {
                l = "/";//character used in order to indicate when reading from the file that last name is absent
            }
            if (lbClasses.SelectedIndex == -1)//if no classID is selected in listbox
            {
                userSelect = sto.CurrentClassID;//just use the most recent classId from storage object
            }
            else//if a classID from listbox is selected, use that as the new student's class!
            {
                userSelect = lbClasses.SelectedItem.ToString();
                sto.CurrentClassID = userSelect;
            }
            Student toAdd = new Student(f, l, userSelect);
            sto.AddStudentToFile(toAdd);
            txtFName.Clear();//clear in case they want to add another student
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
            txtFName.Focus();//sets cursor back in firstname box in case user want to add another student
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
        //"Show Student Progress" button click
        private void btnStuAchieve_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            ClearGroupBoxes();//clears anything in overall group boxes to minimize distractions to user.
            if (scRosterDisplay.Visible == true)//if roster is visible
            {
                if (lvRoster.CheckedItems.Count == 1)//one student is checked in the box
                {
                    lbStuProgCom.Items.Clear();//clears any existing data in the student progress boxes
                    lbStuProgPart.Items.Clear();
                    int index = lvRoster.CheckedIndices[0];
                    int actualIndex = sto.ActiveStudentObjectIndices[index];//retrieves absolute index value of student that is checked. 
                    Student selected = sto.TempStudentObjectList[actualIndex];//assigns 'selected' to existing student object reference in storage object.
                    grpStuProgress.Text = selected.ToStringFirstLast();
                    foreach (Objective comp in selected.Completed)
                    {
                        string id = comp.ToString();
                        lbStuProgCom.Items.Add(id);//adds each objective that is in student object's completed list to the listbox
                    }
                    foreach (Objective part in selected.Partial)
                    {
                        string id = part.ToString();
                        lbStuProgPart.Items.Add(id);//adds each objective that is in student object's partially completed list to the listbox
                    }
                    grpStuProgress.Visible = true;
                }
                else//either no student is selected or more than one student is selected
                {
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
        //"Generate Groups" button click
        private void btnGoGroups_Click(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            ClearGroupBoxes();
            ClearStuProgBoxes();
            if (scRosterDisplay.Visible == true)//prohibits code from running when menu item option is selected but before class has been selected.
            {
                int objecCount = lvObjectives.CheckedItems.Count;//how many objectives are checked
                if (objecCount > 0)//ensures that at least one objective to group students by is actually checked.
                {
                    int[] oIndices = new int[objecCount];//will hold the absolute index values of all currently checked objectives
                    List<string> currentSelectedObjectives = new List<string>();//will hold the string id's of currently checked objectives
                    ListView.CheckedListViewItemCollection objList = lvObjectives.CheckedItems;//items that are checked
                    int o = 0;//to start with first index in oIndices
                    foreach (ListViewItem item in objList)
                    {
                        int active = item.Index;//position index value of checked item in currently displayed objective box.
                        //this is not necessarily the same number as position in data file because user might have had objective
                        //list display customized by chapter
                        int overall = sto.ActiveObjectiveIndices[active];//this value should be the absolute index position of
                        //checked item as the ActiveObjectiveIndicies array holds this pointer.
                        oIndices[o] = overall;//stores the absolute index value of checked item into the oIndices array holder
                        currentSelectedObjectives.Add(sto.TempObjectivesList[overall]);//adds the string value of current objective to the temporary list that holds the id values of all currently selected objectives.
                        ++o;//increments counter in order to go to the next spot in the array if there is another objective checked.
                    }
                    string currentClass = sto.CurrentClassID;
                    foreach (Student stud in sto.TempStudentObjectList)/*iterates through all StudentObjects
                        this is probably not the most efficient way to do this but I don't know another solution yet.*/
                    {
                        if (stud.SectionID == sto.CurrentClassID)//checks student object to determine if this particular student belongs to the class the user is working with.
                        {
                            bool passAll = true;//when true, indicates the student has passed every objective that the user wants to group by.
                            bool partAll = true;//when true, indicates the student has passed OR partially passed every objective the user wants to group by.
                            for (int i = 0; i < o; ++i)// 'o' should be the number of objectives the user selected.
                            {
                                bool pass = false;//when true, indicates the student has passed THIS specific objective (iteration)
                                bool part = false;//when true, indicates the student partially passed THIS specific objective
                                string current = sto.TempObjectivesList[oIndices[i]];//represents the id value of the objective that is checked
                                foreach (Objective compObj in stud.Completed)//iterates through student object's completed objective list
                                {
                                    if (pass == false)//if pass is true this means that the student has completed the specified objective and there is no reason to keep searching.
                                    {//would putting a BREAK below work better?
                                        if (current == compObj.ToString())
                                        {
                                            pass = true;
                                            //objective found in student's completed list! would a BREAK work more efficiently?
                                        }
                                    }
                                }
                                foreach (Objective partObj in stud.Partial)//iterates through student object's partially completed objective list
                                {
                                    if (pass == false)/*if student has completely passed this objective we're not going to 
                                        see if they also partially passed it because the completion 'supercedes the partial*/
                                    {
                                        if (part == false)//if part value is true it means the current objective was found in 
                                            //student's partial list so we should move on.
                                        {
                                            if (current == partObj.ToString())
                                            {
                                                part = true;
                                                //would break be better here?
                                            }
                                        }
                                    }
                                }
                                //the sequence below should happen for each student after EACH iteration through the selected objectives that the user wants to compare to.
                                if ( (part == true && partAll == true) /* student has partial pass of current objective AND partial pass of all the previous ones compared to (if any)*/ 
                                    || (part == true && passAll == true) /*student has partial pass of current objective AND complete pass of all previous ones*/ 
                                    || (pass == true && partAll == true) ) /*student has partial pass of previous objective(s)  and complete pass of current objective*/
                                {
                                    partAll = true; /*set Partial pass of all to true 
                                    (Student has at LEAST PARTIALLY passed all objectives, 
                                    even if some of those include complete passes)*/
                                }
                                else
                                {
                                    partAll = false; //even if student has passed all the selected objectives, PARTall will be false
                                }
                                if (passAll == true && pass == true) //if the student has passedAll and passed the current one
                                {
                                    passAll = true; //set passAll to (student has passed All so far)
                                }
                                else
                                {
                                    passAll = false; //once a student hasn't passed one, passAll needs to be false
                                }
                                //end sequence
                            }

                            //this sequence will occur ONCE for each student after all the selected compare to objectives have been iterated through.
                            if (passAll == true)//if they passedAll the selected objectives, add their name to the passing listview box.
                            {
                                lvCompleted.Items.Add(stud.ToStringFirstLast());
                            }
                            else if (partAll == true)//if they partially passed all.  This would also include if they completely passed all except one that was partially passed.  Any combination of partial and complete passes will put them in the partially passed box.
                            {
                                lvPartial.Items.Add(stud.ToStringFirstLast());
                            }
                            else
                            {
                                lvNot.Items.Add(stud.ToStringFirstLast());
                            }
                        }
                    }
                    lblSortGroupList.Text = "The objective()s used\nas criteria for this\nsort was/were:";
                    int objCount = currentSelectedObjectives.Count();
                    if (objCount > 3)//just display three objectives and tell how many more there were (instead of having a huge list).
                    {
                        //lblSortGroupList.Text += "\n";//adds line break to message
                        for (int i = 0; i < 3; ++i)
                        {
                            lblSortGroupList.Text += "\n" + currentSelectedObjectives[i];
                        }
                        int diff = objCount - 3;
                        lblSortGroupList.Text += "\nAnd " + diff + " other(s).";
                    }
                    else
                    {
                        for (int i = 0; i < objCount; ++i)
                        {
                            lblSortGroupList.Text += "\n" + currentSelectedObjectives[i];
                        }
                    }
                    lblSortGroupList.Visible = true;
                    pnlGroups.Visible = true;
                }

                else//if objective box is displayed but nothing is checked.
                {
                    lblFeedbackMessage.Text = "You must select at least one objective to generate achievement groups for.";
                    lblFeedbackMessage.Visible = true;
                }
            }
            else//if user tries to generate groups but hasn't selected a class yet.  most likely would get to this point if user tried to use menuitems to generate groups before they selected a class.
            {
                lblFeedbackMessage.Text = "Please first select a class so you can select which\nobjective(s) you would like to generate groups from.";
                lblFeedbackMessage.Visible = true;
                tsmiLoadClass_Click(sender, e);//will display the menu to select a class from.
            }
        }

        /// <summary>
        /// Will generate list of all existing items in the group boxes with detailed headings and send to the clipboard
        /// to be used in an outside document.  Includes notation if boxes were empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //"Copy Groups to Clipboard" button click
        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            if (Mac == false)
            {
                StringBuilder sb = CopyAllGroupBoxesString();
                Clipboard.SetData(System.Windows.Forms.DataFormats.Text, sb);/*copies final stringBuilder object
            to clipboard*/
                lblFeedbackMessage.Text = "Group data has been copied to the clipboard.\nYou should be able to paste into a document.";
                lblFeedbackMessage.Visible = true;//message just to confirm to user that the material was copied.
            }
            else//if Mac boolean == true!
            {
                StringBuilder macSb = CopyAllGroupBoxesString();
                string macSbString = macSb.ToString();
                macSbString.Trim();
                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(@"../../Write-Read/GroupsClipboard.txt", false))
                {
                    file.WriteLine(macSbString);
                }
                lblFeedbackMessage.Text = "Group data has been copied to the GroupsClipboard.txt file.";
                lblFeedbackMessage.Visible = true;//message just to confirm to user that the material was copied.
            }
        }

        /// <summary>
        /// Generates stringBuilder object with current group data from "generate groups" panel
        /// includes descriptive headings and identifies objectives used for grouping.
        /// </summary>
        /// <returns>StringBuilder object</returns>
        private StringBuilder CopyAllGroupBoxesString()
        {
            lblFeedbackMessage.Visible = false;
            StringBuilder fullSb = new StringBuilder();//will hold the full strings element to copy to clipboard.
            string temp;//will hold the list of strings from each box
            temp = lblSortGroupList.Text;
            string barrier = "--------------------";
            fullSb.AppendLine(barrier);
            fullSb.AppendLine(temp);
            fullSb.AppendLine(barrier);
            fullSb.AppendLine();
            CopyListViewBox(lvCompleted, out temp);//generates strings for items in "Completed Box"
            fullSb.AppendLine("**COMPLETE**");
            if (temp == null)//nothing was in the "Completed" box
            {
                fullSb.AppendLine("//NONE//");
                fullSb.AppendLine();//skips a line between next group
            }
            else
            {
                fullSb.AppendLine(temp);//places temp strings under the heading **COMPLETE**
            }

            CopyListViewBox(lvPartial, out temp);/*generates strings for items in 
            "Partially Completed" box overwrites (or just changes pointer) the temp string used previously*/
            fullSb.AppendLine("**PARTIALLY**");
            if (temp == null)//nothing was in the partially completed box
            {
                fullSb.AppendLine("//NONE//");//tells user that there was no one in this group
                fullSb.AppendLine();
            }
            else
            {
                fullSb.AppendLine(temp);//places temp strings under the heading *Partially*
            }

            CopyListViewBox(lvNot, out temp);//generates strings for items in "Incomplete" box
            fullSb.AppendLine("**INCOMPLETE**");
            if (temp == null)//if no items were in the incomplete box
            {
                fullSb.AppendLine("//NONE//");
                //fullSb.AppendLine();//no need to skip another line because this is the last group
            }
            else
            {
                fullSb.AppendLine(temp);//places temp strings under the heading "Incomplete"
            }
            return fullSb;
        }

        /// <summary>
        /// Gets all items in passed listview box and stores them in a string.  
        /// Each item is contained on separate line.
        /// </summary>
        /// <param name="listViewBox">ListView display box to copy existing items from</param>
        private void CopyListViewBox(ListView listViewBox, out string sbString)
        {
            int count = listViewBox.Items.Count;//how many items are in the listview box
            if (count > 0)//will go in if the box is not empty
            {
                StringBuilder sb = new StringBuilder();//using stringbuilder to provide opportunity to continue to append.
                for (int j = 0; j < count; ++j)//iterates through all items in the listview box
                {
                    string temp = listViewBox.Items[j].Text;//stores current listview box item
                    if (temp != null && temp != " " && temp != "")
                    {
                        sb.AppendLine(temp);
                    }
                }
                sbString = sb.ToString();//converts stringbuilder object to string to return.
                //Clipboard.SetData(System.Windows.Forms.DataFormats.Text, sbString);
            }
            else//box is empty
            {
                sbString = null;//simply return a null string
            }
        }

        #endregion

        #region Add New Objective
        /// <summary>
        /// Executes when the menuItem to add learning objective is clicked.  Group 'panel' with objective
        /// fields will display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //Learning Objective, "Create New" menu item click
        private void tsmiAddObjective_Click(object sender, EventArgs e)
        {
            grpLearnObj.Visible = true;
            lblObjAddConfirm.Visible = false;
            txtObjId.Focus();//sets cursor objective id textbox position. Ready for user input!
            tsmiLearningObjectives.HideDropDown();//hides dropdown menu from item that was clicked.
        }

        /// <summary>
        /// Executes when "Create Objective" button is clicked or simulated through enter press.
        /// If no description is given a "/" character will be used to store the description during objective
        /// object creation.  An objective ID is required for method to execute.  Error message will
        /// display if ID field is not entered.  This will not check for duplicate objectives!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //Create New Learning Objective "Create Objective" button click
        private void btnAddObjective_Click(object sender, EventArgs e)
        {
            string id = txtObjId.Text.Trim();
            string desc = txtObjDesc.Text.Trim();
            if (id != null && id != " " && id != "")//if user did not put in anything in id field
            {
                if (desc == null || desc == "")//if user did not put in a description, which is permitted
                {
                    desc = "/";//using this character for file storage purposes to identify an objective that does not have a description
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
            else//user did not put anything in id field
            {
                lblFeedbackMessage.Text = "You must enter a unique identifier for the learning objective.  Please try again.";
                //lblObjAddConfirm.Text = "You must enter a unique identifier for the learning objective.  Please try again.";
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
            //lblFeedbackMessage.Visible = false;
        }
        private void pnlGroups_MouseEnter(object sender, EventArgs e)
        {
            //lblFeedbackMessage.Visible = false;
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
            lblSortGroupList.Text = "";//resets the text that explained which objectives were used to sort the groups
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
        /// Clears all items from student achievement AND class grouping list boxes.
        /// </summary>
        private void ClearAllGroupBoxes()
        {
            ClearGroupBoxes();
            ClearStuProgBoxes();
        }
        #endregion

        #region Menu Item Hover Display Controls
        //bool obj1Drop = false, obj2Drop = false, obj3Drop = false, obj4Drop = false;
        //private bool NO_CURSOR_IN_MENU()
        //{
        //    bool returnVal;
        //    if (obj1Drop == false && obj2Drop == false && obj3Drop == false && obj4Drop == false)
        //    {
        //        returnVal = true;
        //    }
        //    else
        //    {
        //        returnVal = false;
        //    }
        //    return returnVal;
        //}

        private void tsmiLearningObjectives_MouseEnter(object sender, EventArgs e)
        {
            lblFeedbackMessage.Visible = false;
            tsmiClass.HideDropDown();
            tsmiStudents.HideDropDown();
            tsmiLearningObjectives.ShowDropDown();
        }

        /// <summary>
        /// Executes when mouse enters "Group Students By" menu item box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiGroups_MouseEnter(object sender, EventArgs e)
        {
            grpLearnObj.Visible = false; //Hides add learning objective panel group so the rest of the menu item can display cleanly.
            tsmiGroups.ShowDropDown();
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
    }
}



