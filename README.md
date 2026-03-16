Habit Tracker Project
=====================

Console app to track habits using Sqlite CRUD functions and C# ADO.NET.

The initial thought was "I've been drinking a lot of coffee lately, bet it could be more if I put my mind to it." The concept then became that I wanted to track only coffee consumption, but begrudgingly added multiple habits to track for the challenge. I also wanted to make something that I could actually use and would enjoy using.

Requirements:
-------------
- This is an application where you’ll log occurrences of a habit.
- This habit can't be tracked by time (ex. hours of sleep), only by quantity (ex. number of water glasses a day)
- Users need to be able to input the date of the occurrence of the habit
- The application should store and retrieve data from a real database
- When the application starts, it should create a sqlite database, if one isn’t present.
- It should also create a table in the database, where the habit will be logged.
- The users should be able to insert, delete, update and view their logged habit.
- You should handle all possible errors so that the application never crashes.
- You can only interact with the database using ADO.NET. You can’t use mappers such as Entity Framework or Dapper.
- Follow the DRY Principle, and avoid code repetition.
- Your project needs to contain a Read Me file where you'll explain how your app works and tell a little bit about your thought progress. What was hard? What was easy? What have you learned? 

Features
--------
Sqlite db connection
- Creates 2 tables for habit instances and habit types
- CRUD habit instance records
- Auto-seeding optional on startup if habit record table is empty
- Adding habit types and unit to track them in

Console app
- Centered formatting
- Ability to generate record report for individual habit types or all habit types
- Habit type selector in main menu
- Input validation and easy input when inputting a record for today's date
- Coffee animation

Thought Process
---------------

I had to follow cap's tutorial to get my bearings on SQL at first. One piece of it that really stood out was the SELECT EXISTS operation 'SELECT EXISTS (SELECT 1 FROM drinking_wa...' I couldn't find specific documentation for it, but trying to understand what it returned and the syntax making it work was interesting.

Used primary constructor assignments in CoffeeRecord class for simplicity and omitted sets.

Tried to oversimplify get input into one method, for validation I decided this was the wrong move.

Found it helpful to think of what will make me enjoy using the app.

The program namespace is a little cluttered, so I moved the formatting methods to a library.

What Was Hard
-------------
- Viewing SQL db files
- Setting working directory
- Understanding how SQL syntax translates to literal function
- Understanding C# to SQL interaction
- Wrapping my head around SQL data structure
- SQL connection and commands

What Was Easy
-------------
- C# syntax and code structure
- Using methods
- Logical operation
- Thinking in terms of tables (col & row)
- Tracking down bugs

What Have I Learned
-------------------
- Using sqlite types and methods to interact with sql dbs
- Basic sql commands like INSERT, SELECT, REPLACE and DELETE
- Basic sql formatting and order of ops
- Creating and referencing a persistent local database
- How to escape safely from the middle of a method
- More about ConsoleKey and DateTime types
- Using the documentation and classes in Visual Studio to understand objects and their methods

Areas to Improve
----------------
- Understanding where to divide methods and how to implement seperation of concerns is a big struggle for me. I tried to divide methods this time more than in my previous project. Identifying patterns and repetition is something I could work on in future projects
- Managing the flow of a project. Generally, I felt comfortable in the order of steps, but by the end of the project it felt like navigating back and forth in the code was very inefficient. Focusing on one task at a time was also a struggle. Hopefully I can get a better understanding of project management in the next challenge.
