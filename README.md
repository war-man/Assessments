# Assessments
Student Learning Progress Windows Forms Project

This idea for this project was conceived after a conversation with a family member about the time she spent
as a teacher grouping students based upon their individual achievement.

The existing scenario was the following:
the teachers were recording whether or not a student learned a particular concept after
an assessment using a spreadsheet.  'X' if the student passed, '/' if they "kind of" got it, and the
box was left blank if the student did not yet demonstrate understanding.

After all students were assessed, the teachers would look through the spreadsheet, trying to determine
groups of students who were "ready" to move on, needed more help with a particular concept, or maybe
even needed a complete re-teaching of the concept.

The teachers were spending too much time simply trying to determine where the 'x's were and losing time
being able to teach the students.

I thought this was a great opportunity for me to try to complete a project outside of class time and also 
learn my way through Windows Forms and implement object oriented principles.

The basic premise of the solution I wanted to provide for the teachers was this:
Ability to 'input' students, learning objectives, and a way to assign 'completion/progress' to individual students
all while maintaining the ability to retrieve this data when a new instance of the program is run.  Prior to this,
all of my Windows Forms projects had been useful at the time of instatiation but none actually needed to 'load'
or save information from a previously saved session.

The solution I developed for the teachers was certainly a combination of many iterations of development, including
feedback after a demo with my family member (teacher).  In my admission, I know there are probably already existing
software solutions to the scenario the teachers were facing.  My solution is not the most efficient or 'fastest'
in terms of architecture or computation.  Yet I do believe it is a great showcase of what I was able to accomplish
after two months of working in a C#, Visual Studio environment.

In addition to the windows forms classes, the solution has three main classes:
Student class, in which each student is created as an object, with fields that 'hold' their achievement results
Learning objectives class, in which each learning objective is created with fields that uniquely identify it
Storage class, which was by far the most extensive and challenging class.  I essentially created one storage object
each time the application runs, which is used to retrieve, hold, and update data during the user interaction while
the windows form is active.  I had not learned database structure before beginning this project, so I actually used
.txt files to store the data.

This project continues to be a fluid, living application with updates or additions to be added as time permits.

Please feel free to contact me with questions.
